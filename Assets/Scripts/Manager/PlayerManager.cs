using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the human player.
/// </summary>
public class PlayerManager : BasePlayer
{
    private new Camera camera;

    public GameObject[] pieces = new GameObject[16];

    private List<GameObject> highlightedCells = new List<GameObject>();

    private Cell previousCell;

    private bool hasSelectedAPiece = false;

    private GameObject selectedPiece = null;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// If it's this player's turn and the user has pressed the left mouse button,
    /// then check if the player is selecting a piece to move or a place to move the piece.
    /// If the player is chosing a piece to move, highlight all the valid moves. 
    /// Otherwise, if the player has already selected a piece and has clicked on a valid 
    /// square, move the piece and tell the GameManager that our turn has ended.
    /// </summary>
    void Update()
    {
        if (this.canMove && Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();
            var obj = hit.transform.gameObject;

            if (cell)
            {
                if (cell.GetCurrentPiece && pieces.Contains(cell.GetCurrentPiece))
                {
                    if (!hasSelectedAPiece)
                    {
                        previousCell = cell;
                        hasSelectedAPiece = true;

                        selectedPiece = previousCell.GetCurrentPiece;

                        highlightedCells = Manager.SetSelectedPiece(hit, cell);
                    }            
                    else
                    {
                        if(selectedPiece.GetComponent<IRoyalty>() == null || !selectedPiece.GetComponent<IRoyalty>().HasMoved)
                        {
                            ClearSelected();

                            previousCell = cell;
                            hasSelectedAPiece = true;

                            selectedPiece = previousCell.GetCurrentPiece;

                            highlightedCells = Manager.SetSelectedPiece(hit, cell);
                        }
                        else
                        {
                            ChangeTurn();
                        }
                    }
                }              
                else if(!cell.GetCurrentPiece && hasSelectedAPiece)
                {
                    if (highlightedCells.Contains(obj))
                    {
                        var royalty = selectedPiece.GetComponent<IRoyalty>();

                        if(royalty != null)
                        {
                            var indexes = Tools.FindIndex(GameManager.boardArr, hit.transform.gameObject);

                            if (royalty.CanMoveAgain(indexes))
                            {
                                MovePiece(cell);

                                royalty.UpdateMovementNum(indexes);
                                royalty.ResetPos(indexes);

                                ClearCells();
                                highlightedCells = Manager.SetSelectedPiece(hit, cell);

                                if(!highlightedCells.Any())
                                {
                                    royalty.ResetMovementNum();
                                    ChangeTurn();
                                }
                            }
                        }
                        else
                        {
                            MovePiece(cell);
                            ChangeTurn();
                        }
                    }
                } 
            }
        }
    }

    private void ChangeTurn()
    {
        selectedPiece = null;

        IsTurn(false);
        Manager.ChangeTurn(this.gameObject);

        ClearSelected();
    }

    /// <summary>
    /// Clears and restores the highlighted cells after a piece has been moved.
    /// </summary>
    private void ClearSelected()
    {
        previousCell = null;

        hasSelectedAPiece = false;

        ClearCells();
    }

    private void ClearCells()
    {
        highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
        highlightedCells.Clear();
    }

    /// <summary>
    /// Moves the piece to the new cell location.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="obj"></param>
    private void MovePiece(Cell cell)
    {
        var currPiece = selectedPiece;

        int currX = currPiece.GetComponent<BasePiece>().positionY;
        int currY = currPiece.GetComponent<BasePiece>().positionX;

        var newPos = Manager.GetMovePosition(cell.gameObject, selectedPiece);

        int newX = currPiece.GetComponent<BasePiece>().positionY;
        int newY = currPiece.GetComponent<BasePiece>().positionX;

        Manager.UpdateIntBoard(currX, currY, newX, newY, currPiece.GetComponent<IPieceBase>().PieceID);

        currPiece.GetComponent<BasePiece>().Move(newPos);

        if(currPiece.GetComponent<Pawn>())
        {
            currPiece.GetComponent<Pawn>().UpdateMoved();
        }

        cell.GetCurrentPiece = currPiece;
        previousCell.GetCurrentPiece = null;
    }

    public override void SetPieces(List<GameObject> pieces)
    {
        this.pieces = pieces.ToArray();
    }
}
