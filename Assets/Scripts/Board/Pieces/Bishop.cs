using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Bishop piece.
/// </summary>
public class Bishop : BasePiece, IPieceBase
{
    public int PieceID { get; set; } = 4;

    /// <summary>
    /// Highlights the valid moves for this piece.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The piece's X position on the board.</param>
    /// <param name="y">The piece's Y position on the board.</param>
    /// <returns>A list of all valid moves for this piece.</returns>
    public override List<GameObject> Highlight(GameObject[,] board, int x, int y)
    {
        return base.HighlightCells(board, x, y, 2);
    }
}
