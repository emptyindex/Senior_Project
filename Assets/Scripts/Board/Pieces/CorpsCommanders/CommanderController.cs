using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [HideInInspector]
    public List<GameObject> controlledPieces = new List<GameObject>();
    [HideInInspector]
    public PlayerManager player;

    protected new Camera camera;

    protected GameObject selectedPiece = null;
    private Cell previousCell = null;

    private List<GameObject> highlightedCells = new List<GameObject>();

    private GameManager manager;

    protected bool isMoving = false;

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

        this.MenuCanvas = gameObject.transform.Find(canvasName).gameObject;

        this.MenuCanvas.SetActive(false);

        this.CurrentMove = MoveToMake.None;
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

            if(CurrentMove != MoveToMake.None && !isMoving)
            {
                switch (CurrentMove)
                {
                    case MoveToMake.MoveSelf:
                        var basePiece = gameObject.GetComponent<BasePiece>();
                        selectedPiece = gameObject;
                        highlightedCells = basePiece.Highlight(GameManager.boardArr, basePiece.positionX, basePiece.positionY);
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
        }
    }

    protected void HighlightAllExceptCommander(List<GameObject> listToHighlight)
    {
        listToHighlight.ForEach(p => p.GetComponent<PieceColorManager>().SetHighlight(true));
        gameObject.GetComponent<PieceColorManager>().SetHighlight(false);
    }

    protected bool SelectPiece(bool toMove, List<GameObject> piecesToCheck)
    {
        if(Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            // if we clicked on a cell populated with one of this corps commander's controlled pieces
            if (cell && cell.GetCurrentPiece && piecesToCheck.Contains(cell.GetCurrentPiece))
            {
                if (!selectedPiece)
                {
                    previousCell = cell;
                    selectedPiece = cell.GetCurrentPiece;

                    if (toMove)
                    {
                        highlightedCells = manager.SetSelectedPiece(hit, cell);
                    }

                    piecesToCheck.ForEach(p =>
                    {
                        if (!p.Equals(selectedPiece))
                            p.GetComponent<PieceColorManager>().SetHighlight(false);
                    });

                    return true;
                }
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

            if (cell && selectedPiece && highlightedCells.Contains(cell.gameObject))
            {
                var royalty = selectedPiece.GetComponent<IRoyalty>();
                var basePiece = selectedPiece.GetComponent<BasePiece>();

                int startPosX = basePiece.positionX;
                int startPosY = basePiece.positionY;
                int endPosX;
                int endPosY;

                if (royalty != null)
                {
                    var indexes = Tools.FindIndex(GameManager.boardArr, hit.transform.gameObject);

                    if (royalty.CanMoveAgain(indexes))
                    {
                        basePiece.MovePiece(cell, manager);
                        previousCell.GetCurrentPiece = null;

                        royalty.UpdateMovementNum(indexes);
                        royalty.ResetPos(indexes);

                        ClearCells();
                        highlightedCells = manager.SetSelectedPiece(hit, cell);

                        if (!highlightedCells.Any())
                        {
                            royalty.ResetMovementNum();
                            endPosX = basePiece.positionX;
                            endPosY = basePiece.positionY;

                            EndMovement(startPosX, startPosY, endPosX, endPosY);
                        }
                    }
                }
                else
                {
                    selectedPiece.GetComponent<BasePiece>().MovePiece(cell, manager);
                    previousCell.GetCurrentPiece = null;

                    endPosX = basePiece.positionX;
                    endPosY = basePiece.positionY;

                    EndMovement(startPosX, startPosY, endPosX, endPosY);
                }
            }
        }
    }

    private void EndMovement(int startPosX, int startPosY, int endPosX, int endPosY)
    {
        if (CurrentMove == MoveToMake.MoveSelf && HasMovedOneSpot(startPosX, startPosY, endPosX, endPosY) && isFirstMove)
        {
            ResetMoveWithOutEnd();
        }
        else
        {
            ResetMove();
        }
    }

    private bool HasMovedOneSpot(int sX, int sY, int eX, int eY)
    {
        return Mathf.Abs(sX - eX) <= 1 && Mathf.Abs(sY - eY) <= 1;
    }

    private void ResetMoveWithOutEnd()
    {
        ClearCells();

        selectedPiece.GetComponent<PieceColorManager>().SetHighlight(false);
        selectedPiece = null;

        isMoving = false;
        isFirstMove = false;

        CurrentMove = MoveToMake.None;
    }

    protected void ResetMove()
    {
        ResetMoveWithOutEnd();

        commandAuthorityTaken = true;
        isFirstMove = true;
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
