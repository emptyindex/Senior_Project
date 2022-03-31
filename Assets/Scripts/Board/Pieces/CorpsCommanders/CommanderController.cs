using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MoveToMake
{
    Delegate, 
    MoveSelf,
    MoveOther,
    RecallDelegate,
    None
}

public class CommanderController : MonoBehaviour, ICorpsCommander
{
    public string canvasName;

    //[HideInInspector]
    public List<GameObject> controlledPieces = new List<GameObject>();
    [HideInInspector]
    public PlayerManager player;

    protected new Camera camera;

    protected GameObject selectedPiece = null;

    private Cell previousCell = null;


    private static GameObject attackingPiece;

    private static Cell cellToAttack;

    private GameObject deadPile;

    public List<GameObject> highlightedCells = new List<GameObject>();
    public List<GameObject> attackCells = new List<GameObject>();

    private GameManager manager;

    protected bool isMoving = false;
    private bool isAttacking = false;

    private bool commandAuthorityTaken = false;
    private bool isFirstMove = true;

    public bool HasTakenCommand { get; set; } = false;
    public GameObject MenuCanvas { get; set; }

    public delegate void CommandEndedEvent();
    public event CommandEndedEvent OnCommandEnded;

    public MoveToMake CurrentMove { get; set; }

    public void SetToMoveSelf()
    {
        CurrentMove = MoveToMake.MoveSelf;
        HideMenuAndToolTip();
    }

    public void SetToMoveOther()
    {
        CurrentMove = MoveToMake.MoveOther;
        HideMenuAndToolTip();
    }

    protected void HideMenuAndToolTip()
    {
        MenuCanvas.SetActive(false);
        TooltipManager.Hide();
    }

    protected void Start()
    {
        camera = Camera.main;

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        manager.dice.OnDiceEnded += CheckAttackSuccessful;

        this.MenuCanvas = gameObject.transform.Find(canvasName).gameObject;

        this.MenuCanvas.SetActive(false);

        this.CurrentMove = MoveToMake.None;

        deadPile = GameObject.FindWithTag("Deadpile");
    }

    private void CheckAttackSuccessful()
    {
        if(attackingPiece != null && cellToAttack != null) 
        {
            bool result = attackingPiece.GetComponent<IPieceBase>().IsAttackSuccessful(cellToAttack.GetCurrentPiece.GetComponent<IPieceBase>().PieceID, DiceNumberTextScript.diceNumber);

            print(result);

            if (result)
            {
                FinishAttack();
            }
            else
            {
                cellToAttack.GetComponent<Cell>().IsAttackHighlighted = false;
                attackingPiece.GetComponent<BasePiece>().spotLight.enabled = false;

                this.ResetMove();
            }
        }
    }

    private void FinishAttack()
    {
        // Do attack
        cellToAttack.GetCurrentPiece.transform.SetParent(deadPile.transform, false);
        cellToAttack.GetCurrentPiece.transform.position = Vector3.zero;

        // Tell other player they lost a piece

        // Move attacking piece
        attackingPiece.GetComponent<BasePiece>().MovePiece(cellToAttack, manager);

        cellToAttack.GetComponent<Cell>().IsAttackHighlighted = false;
        attackingPiece.GetComponent<BasePiece>().spotLight.enabled = false;

        this.ResetMove();
    }

    protected void Update()
    {
        if(commandAuthorityTaken)
        {
            HasTakenCommand = false;
            commandAuthorityTaken = false;

            OnCommandEnded.Invoke();
        }

        if(HasTakenCommand)
        {
            if(!MenuCanvas.activeSelf && CurrentMove == MoveToMake.None)
            {
                MenuCanvas.SetActive(true);
            }

            if(CurrentMove != MoveToMake.None && !isMoving && !isAttacking)
            {
                switch (CurrentMove)
                {
                    case MoveToMake.MoveSelf:
                        var basePiece = gameObject.GetComponent<BasePiece>();
                        selectedPiece = gameObject;
                        (highlightedCells, attackCells) = basePiece.Highlight(GameManager.boardArr, basePiece.CurrRowPos, basePiece.CurrColPos);
                        isMoving = true;
                        break;
                    case MoveToMake.MoveOther:
                        HighlightAllExceptCommander(controlledPieces);
                        isMoving = SelectPiece(true, controlledPieces);
                        break;
                }
            }

            if(isMoving && selectedPiece)
            {
                MovePiece();
            }

            if(Input.GetKeyUp(KeyCode.C) && (isMoving || isAttacking))
            {
                if(selectedPiece.GetComponent<IRoyalty>() != null && selectedPiece.GetComponent<IRoyalty>().HasMoved)
                {
                    ResetMove();
                }
                else
                {
                    ResetMoveWithOutEnd();
                }
            }
        }
    }

    protected void HighlightAllExceptCommander(List<GameObject> listToHighlight)
    {
        listToHighlight.ForEach(p => p.GetComponent<BasePiece>().spotLight.enabled = true);
        gameObject.GetComponent<BasePiece>().spotLight.enabled = false;
    }

    protected bool SelectPiece(bool toMove, List<GameObject> piecesToCheck)
    {
        if(Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            // if we clicked on a cell populated with one of this corps commander's controlled pieces
            if(cell && cell.GetCurrentPiece && piecesToCheck.Contains(cell.GetCurrentPiece))
            {
                if(selectedPiece)
                {
                    ClearCells();
                    selectedPiece.GetComponent<BasePiece>().spotLight.enabled = false;
                }

                previousCell = cell;

                selectedPiece = cell.GetCurrentPiece;

                if (toMove)
                {
                    (highlightedCells, attackCells) = manager.SetSelectedPiece(hit, cell);
                }

                piecesToCheck.ForEach(p =>
                {
                    if (!p.Equals(selectedPiece))
                        p.GetComponent<BasePiece>().spotLight.enabled = false;
                });

                return true;
            }
        }

        return false;
    }

    private void MovePiece()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();
            //highlightedCells.Contains(cell.gameObject)
            if (cell && selectedPiece)
            {
                var royalty = selectedPiece.GetComponent<IRoyalty>();
                var basePiece = selectedPiece.GetComponent<BasePiece>();

                int startPosX = basePiece.CurrRowPos;
                int startPosY = basePiece.CurrColPos;

                int endPosX;
                int endPosY;

                if (highlightedCells.Contains(cell.gameObject))
                {
                    if (royalty != null)
                    {
                        var indexes = Tools.FindIndex(GameManager.boardArr, hit.transform.gameObject);

                        if (royalty.CanMoveAgain(indexes))
                        {
                            previousCell = GameManager.boardArr[basePiece.CurrRowPos, basePiece.CurrColPos].GetComponent<Cell>();

                            basePiece.MovePiece(cell, manager);
                            previousCell.GetCurrentPiece = null;

                            royalty.UpdateMovementNum(indexes);
                            royalty.ResetPos(indexes);

                            ClearCells();
                            (highlightedCells, attackCells) = manager.SetSelectedPiece(hit, cell);

                            endPosX = basePiece.CurrRowPos;
                            endPosY = basePiece.CurrColPos;

                            if (!highlightedCells.Any())
                            {
                                royalty.ResetMovementNum();

                                EndMovement(startPosX, startPosY, endPosX, endPosY);

                                return;
                            }

                            if (selectedPiece.GetComponent<King>() && isFirstMove)
                            {
                                EndMovement(startPosX, startPosY, endPosX, endPosY);

                                return;
                            }
                        }
                    }
                    else
                    {
                        selectedPiece.GetComponent<BasePiece>().MovePiece(cell, manager);
                        previousCell.GetCurrentPiece = null;

                        endPosX = basePiece.CurrRowPos;
                        endPosY = basePiece.CurrColPos;

                        EndMovement(startPosX, startPosY, endPosX, endPosY);
                    }
                }
                if (attackCells.Contains(cell.gameObject))
                {
                    isMoving = false;
                    isAttacking = true;

                    cellToAttack = cell;
                    attackingPiece = selectedPiece;

                    this.ClearCells();
                    cellToAttack.GetComponent<Cell>().IsAttackHighlighted = true;

                    gameObject.GetComponent<AttackManager>().InvokeAttackRoll();
                }
            }
        }
    }

    private void EndMovement(int startPosX, int startPosY, int endPosX, int endPosY)
    {
        if (CommanderMovedOneSpace(startPosX, startPosY, endPosX, endPosY))
        {
            isFirstMove = false;
            ResetMoveWithOutEnd();
        }
        else
        {
            ResetMove();
        }
    }

    private bool CommanderMovedOneSpace(int startPosX, int startPosY, int endPosX, int endPosY)
    {
        return CurrentMove == MoveToMake.MoveSelf && !HasMovedMoreThanOneSpot(startPosX, startPosY, endPosX, endPosY) && isFirstMove;
    }

    private bool HasMovedMoreThanOneSpot(int sX, int sY, int eX, int eY)
    {
        return Mathf.Abs(sX - eX) > 1 && Mathf.Abs(sY - eY) > 1;
    }

    private void ResetMoveWithOutEnd()
    {
        ClearCells();

        if (selectedPiece)
        {
            selectedPiece.GetComponent<BasePiece>().spotLight.enabled = false;
            selectedPiece = null;
        }

        if(attackingPiece)
        {
            attackingPiece = null;
        }

        if(cellToAttack)
        {
            cellToAttack = null;
        }

        isMoving = false;

        CurrentMove = MoveToMake.None;
    }

    protected void ResetMove()
    {
        ResetMoveWithOutEnd();

        commandAuthorityTaken = true;
        isFirstMove = true;
    }

    private void ClearCells()
    {
        if(highlightedCells.Any())
        {
            highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
        }
        if(attackCells.Any())
        {
            attackCells.ForEach(c => c.GetComponent<Cell>().IsAttackHighlighted = false);
        }

        highlightedCells.Clear();
        attackCells.Clear();
    }
}
