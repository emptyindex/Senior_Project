using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Pawn piece.
/// </summary>
public class Pawn : BasePiece
{
    /// <summary>
    /// Pawns cannot move backwards, depending on their orientation on the board,
    /// they can move forwards or "backwards" towards the center of the board.
    /// </summary>
    public bool MoveUp { get; set; } = true;

    /// <summary>
    /// Highlights the valid moves for this piece.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The piece's X position on the board.</param>
    /// <param name="y">The piece's Y position on the board.</param>
    /// <returns>A list of all valid moves for this piece.</returns>
    public override List<GameObject> Highlight(GameObject[,] board, int x, int y)
    {
        switch (MoveUp)
        {
            case true:
                return base.HighlightCells(board, x, y, 1, true, true, false, false, true, false, false, false);
            case false:
                return base.HighlightCells(board, x, y, 1, false, false, true, true, false, true, false, false);
        }
    }
}
