using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base Piece that all Human Player pieces inherit from.
/// </summary>
public abstract class BasePiece : MonoBehaviour, IPieceBase
{
    public int MovementNum { get; set; }
    public int CurrRowPos { get; set; }
    public int CurrColPos { get; set; }
    public int PieceID { get; set; }
    public bool IsDead { get; set; }

    [HideInInspector]
    public List<int[]> validActions = new List<int[]>();
    [HideInInspector]
    public int dangerLevel;

    private int protectionLevel;
    public int ProtectionLevel { get => protectionLevel; set => protectionLevel = value; }

    public int[] GetNumberMoves(int x, int y)
    {
        return new int[] { Mathf.Abs(x - CurrRowPos), Mathf.Abs(y - CurrColPos) };
    }

    /// <summary>
    /// Moves the piece to the new cell location.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="obj"></param>
    public void MovePiece(Cell cell, GameManager manager)
    {
        int currX = CurrColPos;
        int currY = CurrRowPos;

        var newPos = manager.GetMovePosition(cell.gameObject, gameObject);

        int newX = CurrColPos;
        int newY = CurrRowPos;

        manager.UpdateIntBoard(currX, currY, newX, newY, gameObject.GetComponent<IPieceBase>().PieceID);

        Move(newPos);
    }


    /// <summary>
    /// The abstract method that GameManager calls to highlight the available cells on the board.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The current piece's X position on the board.</param>
    /// <param name="y">The current piece's Y position on the board.</param>
    /// <returns></returns>
    public abstract (List<GameObject> moves, List<GameObject> attacks) Highlight(GameObject[,] board, int x, int y);

    /// <summary>
    /// This method gets the all possible positions a piece can move - both forwards and backwards.
    /// It finds the available positions of right, left, forward, backwards, left diagonal, right diagonal,
    /// right backwards diagonal, and left backwards diagonal.
    /// Since some pieces can move in all directions (Queen) and others cannot (Pawn), the method will initally check if a piece can
    /// move in the direction based on the boolean parameter values.
    /// If a potentially valid cell is occupied, then the boolean will be switched to false and the algorithm will no longer
    /// check for addition cells in that direction. This is because pieces cannot jump over others.
    /// Pieces also have a limit on the number of cells they can move at a time, so the while loop terminates once that number is reached.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The current piece's X position on the board.</param>
    /// <param name="y">The current piece's Y position on the board.</param>
    /// <param name="maxTimes">The maximum cells a piece can move at once.</param>
    /// <param name="diaRight">Whether the piece can move right diagonally.</param>
    /// <param name="diaLeft">Whether the piece can move left diagonally.</param>
    /// <param name="diaRightUp">Whether the piece can move right, backwards diagonally.</param>
    /// <param name="diaLeftUp">Whether the piece can move left, backwards diagonally.</param>
    /// <param name="up">Whether the piece can move up/forwards.</param>
    /// <param name="down">Whether the piece can move down/backwards.</param>
    /// <param name="left">Whether the piece can move left.</param>
    /// <param name="right">Whether the piece can move right.</param>
    /// <returns></returns>
    public (List<GameObject>, List<GameObject>) HighlightCells(GameObject[,] board,
        int x, int y,
        bool diaRight = true,
        bool diaLeft = true,
        bool diaRightUp = true,
        bool diaLeftUp = true,
        bool up = true,
        bool down = true,
        bool left = true,
        bool right = true)
    {
        List<GameObject> inRange = new List<GameObject>();
        List<GameObject> inRangeToAttack = new List<GameObject>();

        var maxTimes = this.MovementNum;

        int timesMoved = 1;
        maxTimes++;

        int moveY;
        int moveX;

        while (timesMoved < maxTimes)
        {
            moveY = y + timesMoved;

            // get diagonal down left
            diaLeft = CheckGetMove(board, diaLeft, inRange, inRangeToAttack, moveY, x - timesMoved, timesMoved);

            moveX = x + timesMoved;

            // move right
            right = CheckGetMove(board, right, inRange, inRangeToAttack, y, moveX, timesMoved);

            // move forward
            up = CheckGetMove(board, up, inRange, inRangeToAttack, moveY, x, timesMoved);

            // move diagonal down right
            diaRight = CheckGetMove(board, diaRight, inRange, inRangeToAttack, moveY, moveX, timesMoved);

            moveY = y - timesMoved;

            // move diagonal up right
            diaRightUp = CheckGetMove(board, diaRightUp, inRange, inRangeToAttack, moveY, moveX, timesMoved);

            // move backwards
            down = CheckGetMove(board, down, inRange, inRangeToAttack, moveY, x, timesMoved);

            moveX = x - timesMoved;

            // move left
            left = CheckGetMove(board, left, inRange, inRangeToAttack, y, moveX, timesMoved);

            // move diagonal up left
            diaLeftUp = CheckGetMove(board, diaLeftUp, inRange, inRangeToAttack, moveY, moveX, timesMoved);

            timesMoved++;
        }

        return (inRange, inRangeToAttack);
    }

    private bool CheckGetMove(GameObject[,] board, bool canContinueDirection, List<GameObject> inRange, List<GameObject> inRangeToAttack, int moveY, int moveX, int timesMoved)
    {
        if (canContinueDirection && IsValid(board, moveX, moveY))
        {
            if (IsPopulated(board, moveX, moveY))
            {
                if (HasEnemyPiece(board, moveX, moveY) && IsAbleToAttack(timesMoved))
                {
                    GetMove(board, moveX, moveY, inRangeToAttack, true);

                    return false;
                }
            }
            else
            {
                GetMove(board, moveX, moveY, inRange, false);

                return true;
            }
        }

        return false;
    }

    private bool IsAbleToAttack(int timesMoved)
    {
        var adjPieceID = this.PieceID > 20 ? this.PieceID - 20 : this.PieceID;

        if(timesMoved > 1)
        {
            if(adjPieceID == 3 || adjPieceID == 2)
            {
                //this.gameObject.GetComponent<IRoyalty>().HasMoved = true;
                return true;
            }

            return false;
        }

        if (this.gameObject.GetComponent<IRoyalty>() != null && this.gameObject.GetComponent<IRoyalty>().HasMoved)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Changes the transform of the gameobject that this script is attached to.
    /// </summary>
    /// <param name="position">The new position to move.</param>
    private void Move(Vector3 position)
    {
        gameObject.transform.position = position + new Vector3(0, 0.01f, 0);
    }

    private bool IsValid(GameObject[,] board, int indexX, int indexY)
    {
        if (!InBounds(board, indexX, indexY)) 
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether a cell already contains a piece.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="indexX">The current piece's X position on the board.</param>
    /// <param name="indexY">The current piece's Y position on the board.</param>
    /// <returns></returns>
    private static bool IsPopulated(GameObject[,] board, int indexX, int indexY)
    {
        if(board[indexX, indexY].GetComponent<Cell>().GetCurrentPiece)
        {
            return true;
        }

        return false;
    }

    private bool HasEnemyPiece(GameObject[,] board, int indexX, int indexY)
    {
        var piece = board[indexX, indexY].GetComponent<Cell>().GetCurrentPiece.GetComponent<IPieceBase>();

        if (piece != null)
        {
            return Mathf.Abs(piece.PieceID - PieceID) >= 15;
        }

        return false;
    }

    /// <summary>
    /// Determines if a cell is valid, aka it's within the bounds of the board, it's not already
    /// populated, and the piece can move in that direction.
    /// If the move if valid, the cell is added to a list of highlighted cells (or other valid moves).
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="coord">The x position that we want to check.</param>
    /// <param name="offset">The y position that we want to check.</param>
    /// <param name="inRange">The list of valid cells/moves.</param>
    /// <param name="canMove">The boolean to indicate whether a piece can move in the direction being checked.</param>
    private static void GetMove(GameObject[,] board, int coord, int offset, List<GameObject> inRange, bool checkAttack)
    {
        if (!inRange.Contains(board[coord, offset]))
        {
            inRange.Add(board[coord, offset]);

            if (checkAttack)
            {
                board[coord, offset].GetComponent<Cell>().IsAttackHighlighted = true;
            }
            else
            {
                board[coord, offset].GetComponent<Cell>().IsHighlighted = true;
            }
        }
    }

    /// <summary>
    /// Checks whether the x, y pair exist in the bounds of the board.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="indexX">The current piece's X position on the board.</param>
    /// <param name="indexY">The current piece's Y position on the board.</param>
    /// <returns></returns>
    protected static bool InBounds(GameObject[,] board, int indexX, int indexY)
    {
        bool temp = indexX > -1 && indexX < board.GetLength(0);

        return temp && indexY < board.GetLength(1) && indexY > -1;
    }

    public abstract bool IsAttackSuccessful(int PieceToAttack, int numberRolled);
}
