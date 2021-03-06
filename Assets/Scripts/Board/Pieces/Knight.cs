using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a Knight piece.
/// </summary>
public class Knight : BasePiece, IRoyalty
{
    public int[] InitialStartPos { get; private set; } = new int[2];

    public bool HasMoved { get; set; } = false;

    private readonly int maxMoves = 4;

    public bool CanMoveAgain(int[] newPos)
    {
        return GetNumMoved(newPos) <= this.MovementNum;
    }

    /// <summary>
    /// Highlights the valid moves for this piece.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The piece's X position on the board.</param>
    /// <param name="y">The piece's Y position on the board.</param>
    /// <returns>A list of all valid moves for this piece.</returns>
    public override (List<GameObject>, List<GameObject>) Highlight(GameObject[,] board, int x, int y)
    {
        return base.HighlightCells(board, x, y);
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
        this.HasMoved = false;
    }

    public void ResetPos(int[] newPos)
    {
        InitialStartPos = newPos;
    }

    private void Awake()
    {
        this.MovementNum = 4;
        this.PieceID = 3;
    }

    public int GetNumMoved(int[] newPos)
    {
        return new int[2] { Mathf.Abs(newPos[0] - InitialStartPos[0]), Mathf.Abs(newPos[1] - InitialStartPos[1]) }.Max();
    }

    public override bool IsAttackSuccessful(int PieceToAttack, int roll)
    {
        if (PieceToAttack > 20)
        {
            PieceToAttack -= 20;
        }
        switch (PieceToAttack)
        {
            case 6:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 5:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 4:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 2:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 1:
                if (roll >= 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }
}
