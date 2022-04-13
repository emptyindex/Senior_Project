using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Bishop piece.
/// </summary>
public class Bishop : BasePiece
{
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

    private void Awake()
    {
        this.MovementNum = 2;
        this.PieceID = 4;
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
                if (roll >= 4)
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
                if (roll >= 3)
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
