using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MoveToMake
{
    Delegate, 
    MoveSelf,
    MoveOther,
    None
}

public class CommanderController : MonoBehaviour, ICorpsCommander
{
    [HideInInspector]
    public List<GameObject> controlledPieces = new List<GameObject>();
    [HideInInspector]
    public PlayerManager player;

    private new Camera camera;

    private GameObject selectedPiece = null;
    private Cell previousCell = null;

    private List<GameObject> highlightedCells = new List<GameObject>();

    private GameManager manager;

    private bool isMoving = false;

    private bool isDelegating = false;

    private bool commandAuthorityTaken = false;

    public bool HasTakenCommand { get; set; } = false;
    public GameObject MenuCanvas { get; set; }

    public delegate void CommandEndedEvent();
    public event CommandEndedEvent OnCommandEnded;

    public MoveToMake CurrentMove { get; set; }

    public void SetToDelegate()
    {
        CurrentMove = MoveToMake.Delegate;
        HideMenuAndToolTip();
    }

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

    private void HideMenuAndToolTip()
    {
        MenuCanvas.SetActive(false);
        TooltipManager.Hide();
    }

    private void Start()
    {
        camera = Camera.main;

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        this.MenuCanvas = gameObject.transform.Find("Canvas").gameObject;

        this.MenuCanvas.SetActive(false);

        this.CurrentMove = MoveToMake.None;
    }

    private void Update()
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

            if(CurrentMove != MoveToMake.None && !isMoving && !isDelegating)
            {
                switch (CurrentMove)
                {
                    case MoveToMake.Delegate:
                        // select piece to delegate
                        HighlightAllExceptCommander();
                        SelectPiece(false);

                        break;
                    case MoveToMake.MoveSelf:
                        var basePiece = gameObject.GetComponent<BasePiece>();
                        selectedPiece = gameObject;
                        highlightedCells = basePiece.Highlight(GameManager.boardArr, basePiece.positionX, basePiece.positionY);
                        isMoving = true;
                        break;
                    case MoveToMake.MoveOther:
                        HighlightAllExceptCommander();
                        SelectPiece(true);
                        break;
                }
            }

            if(isMoving && selectedPiece)
            {
                MovePiece();
            }

            if(isDelegating && selectedPiece)
            {
                DelegateToOther();
            }
        }
    }

    private void DelegateToOther()
    {
        var others = player.GetOtherCommanders(gameObject);
        others.ForEach(c => c.GetComponent<BasePiece>().spotLight.enabled = true);

        if(Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            // if we clicked on a cell populated with one of this corps commander's controlled pieces
            if (cell && cell.GetCurrentPiece && others.Contains(cell.GetCurrentPiece)) 
            { 
                cell.GetCurrentPiece.GetComponent<CommanderController>().controlledPieces.Add(selectedPiece);
                //others.ForEach(c => c.GetComponent<BasePiece>().spotLight.enabled = false); TODO: add back in later (add yield to announce commander done)
                ResetMove();
            }
        }
    }

    private void HighlightAllExceptCommander()
    {
        controlledPieces.ForEach(p => p.GetComponent<BasePiece>().spotLight.enabled = true);
        gameObject.GetComponent<BasePiece>().spotLight.enabled = false;
    }

    private void SelectPiece(bool toMove)
    {
        if(Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            // if we clicked on a cell populated with one of this corps commander's controlled pieces
            if(cell && cell.GetCurrentPiece && controlledPieces.Contains(cell.GetCurrentPiece))
            {
                if(!selectedPiece)
                {
                    previousCell = cell;
                    selectedPiece = cell.GetCurrentPiece;

                    if(toMove)
                    {
                        highlightedCells = manager.SetSelectedPiece(hit, cell);

                        isMoving = true;
                    }
                    else
                    {
                        isDelegating = true;
                    }

                    controlledPieces.ForEach(p =>
                    {
                        if (!p.Equals(selectedPiece))
                            p.GetComponent<BasePiece>().spotLight.enabled = false;
                    });
                }
            }
        }
    }

    private void MovePiece()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            if(cell && selectedPiece && highlightedCells.Contains(cell.gameObject))
            {
                var royalty = selectedPiece.GetComponent<IRoyalty>();

                if (royalty != null)
                {
                    var indexes = Tools.FindIndex(GameManager.boardArr, hit.transform.gameObject);

                    if (royalty.CanMoveAgain(indexes))
                    {
                        selectedPiece.GetComponent<BasePiece>().MovePiece(cell, manager);
                        previousCell.GetCurrentPiece = null;

                        royalty.UpdateMovementNum(indexes);
                        royalty.ResetPos(indexes);

                        ClearCells();
                        highlightedCells = manager.SetSelectedPiece(hit, cell);

                        if (!highlightedCells.Any())
                        {
                            royalty.ResetMovementNum();
                            ResetMove();
                        }
                    }
                }
                else
                {
                    selectedPiece.GetComponent<BasePiece>().MovePiece(cell, manager);
                    previousCell.GetCurrentPiece = null;
                    ResetMove();
                }
            }
        }
    }

    private void ResetMove()
    {
        ClearCells();

        selectedPiece.GetComponent<BasePiece>().spotLight.enabled = false;
        selectedPiece = null;

        isMoving = false;
        commandAuthorityTaken = true;

        CurrentMove = MoveToMake.None;
    }


    /// <summary>
    /// Clears and restores the highlighted cells after a piece has been moved.
    /// </summary>
    private void ClearSelected()
    {
        previousCell = null;

        ClearCells();
    }

    private void ClearCells()
    {
        if(highlightedCells.Any())
        {
            highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
            highlightedCells.Clear();
        }
    }
}
