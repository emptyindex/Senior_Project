using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a Queen piece.
/// </summary>
public class Queen : BasePiece, IPieceBase, IRoyalty
{
    public int PieceID { get; set; } = 5;
    public int[] InitialStartPos { get; private set; } = new int[2];

    public bool HasMoved { get; private set; } = false;

    private readonly int maxMoves = 3;

    /// <summary>
    /// Highlights the valid moves for this piece.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The piece's X position on the board.</param>
    /// <param name="y">The piece's Y position on the board.</param>
    /// <returns>A list of all valid moves for this piece.</returns>
    public override List<GameObject> Highlight(GameObject[,] board, int x, int y)
    {
        return base.HighlightCells(board, x, y, this.MovementNum);
    }

    public void ResetPos(int[] newPos)
    {
        InitialStartPos = newPos;
    }

    public bool CanMoveAgain(int[] newPos)
    {
        return GetNumMoved(newPos) <= this.MovementNum;
    }

    public void UpdateMovementNum(int[] newPos)
    {
        if (!HasMoved)
            HasMoved = true;

        this.MovementNum -= GetNumMoved(newPos);
    }

    public void ResetMovementNum()
    {
        this.MovementNum = maxMoves;
    }

    public int GetNumMoved(int[] newPos)
    {
        return new int[2] { Mathf.Abs(newPos[0] - InitialStartPos[0]), Mathf.Abs(newPos[1] - InitialStartPos[1]) }.Max();
    }

    private void Start()
    {
        base.Start();
        this.MovementNum = 3;
    }
}
