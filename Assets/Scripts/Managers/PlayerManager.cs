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

    private GameObject[] pieces = new GameObject[16];

    private List<GameObject> highlightedCells = new List<GameObject>();

    private Cell previousCell;

    private bool hasSelectedAPiece = false;

    public bool canMove = false;

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
        if (canMove && Input.GetMouseButtonUp(0))
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

                        highlightedCells = Manager.SetSelectedPiece(hit, cell);
                    }
                    else
                    {
                        ClearSelected();

                        previousCell = cell;
                        hasSelectedAPiece = true;

                        highlightedCells = Manager.SetSelectedPiece(hit, cell);
                    }
                }              
                else if(!cell.GetCurrentPiece && hasSelectedAPiece)
                {
                    if (highlightedCells.Contains(obj))
                    {
                        MovePiece(cell, obj);

                        IsTurn(false);
                        Manager.ChangeTurn(this.gameObject);

                        ClearSelected();
                    }
                } 
            }
        }
    }

    /// <summary>
    /// Overrided method from the base class to change the canMove boolean.
    /// This determines whether it is this player's turn to move.
    /// </summary>
    /// <param name="newVal">The turn status.</param>
    public override void IsTurn(bool newVal)
    {
        canMove = newVal;
    }

    /// <summary>
    /// Clears and restores the highlighted cells after a piece has been moved.
    /// </summary>
    private void ClearSelected()
    {
        previousCell = null;

        hasSelectedAPiece = false;

        highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
        highlightedCells.Clear();
    }

    /// <summary>
    /// Moves the piece to the new cell location.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="obj"></param>
    private void MovePiece(Cell cell, GameObject obj)
    {
        var newPos = Manager.GetMovePosition(cell.gameObject, previousCell.GetCurrentPiece);

        previousCell.GetCurrentPiece.GetComponent<BasePiece>().Move(newPos);

        cell.GetCurrentPiece = previousCell.GetCurrentPiece;
        previousCell.GetCurrentPiece = null;
    }

    /// <summary>
    /// Gets and sets the GameManager instance to inform it of turn status.
    /// </summary>
    public GameManager Manager { get; set; }

    /// <summary>
    /// Sets the player's pieces at the start of the game.
    /// </summary>
    /// <param name="setPieces">The pieces that this player can manipulate.</param>
    public void SetPieces(GameObject[] setPieces)
    {
        pieces = setPieces;
    }
}
