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
    public override (List<GameObject>, List<GameObject>) Highlight(GameObject[,] board, int x, int y)
    {
        switch (MoveUp)
        {
            case true:
                return base.HighlightCells(board, x, y, true, true, false, false, true, false, false, false);
            case false:
                return base.HighlightCells(board, x, y, false, false, true, true, false, true, false, false);
        }
    }

    private void Awake()
    {
        this.MovementNum = 1;
        this.PieceID = 1;
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
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 5:
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (roll >= 6)
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
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 1:
                if (roll >= 4)
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
