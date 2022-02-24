using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Piece that all Human Player pieces inherit from.
/// </summary>
public abstract class BasePiece : MonoBehaviour
{
    public int MovementNum { get; set; }

    [HideInInspector]
    public int positionX;
    [HideInInspector]
    public int positionY;

    public int[] GetNumberMoves(int x, int y)
    {
        return new int[] { Mathf.Abs(x - positionX), Mathf.Abs(y - positionY) };
    }


    /// <summary>
    /// The abstract method that GameManager calls to highlight the available cells on the board.
    /// </summary>
    /// <param name="board">The 2D array representing the board.</param>
    /// <param name="x">The current piece's X position on the board.</param>
    /// <param name="y">The current piece's Y position on the board.</param>
    /// <returns></returns>
    public abstract List<GameObject> Highlight(GameObject[,] board, int x, int y);

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
    public List<GameObject> HighlightCells(GameObject[,] board,
        int x, int y,
        int maxTimes,
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

        int timesMoved = 1;
        maxTimes++;

        int moveY;
        int moveX;

        while (timesMoved < maxTimes)
        {
            moveY = y + timesMoved;

            // get diagonal down left
            if (diaLeft)
            {
                diaLeft = !IsPopulated(board, x - timesMoved, moveY);

                GetMove(board, x - timesMoved, moveY, inRange, diaLeft);
            }

            moveX = x + timesMoved;

            // move right
            if (right)
            {
                right = !IsPopulated(board, moveX, y);

                GetMove(board, moveX, y, inRange, right);
            }

            // move forward
            if(up)
            {
                up = !IsPopulated(board, x, moveY);

                GetMove(board, x, moveY, inRange, up);
            }

            // move diagonal down right
            if(diaRight)
            {
                diaRight = !IsPopulated(board, moveX, moveY);

                GetMove(board, moveX, moveY, inRange, diaRight);
            }

            moveY = y - timesMoved;

            // move diagonal up right
            if(diaRightUp)
            {
                diaRightUp = !IsPopulated(board, moveX, moveY);

                GetMove(board, moveX, moveY, inRange, diaRightUp);
            }

            // move backwards
            if(down)
            {
                down = !IsPopulated(board, x, moveY);

                GetMove(board, x, moveY, inRange, down);
            }

            moveX = x - timesMoved;

            // move left
            if(left)
            {
                left = !IsPopulated(board, moveX, y);

                GetMove(board, moveX, y, inRange, left);
            }

            // move diagonal up left
            if(diaLeftUp)
            {
                diaLeftUp = !IsPopulated(board, moveX, moveY);

                GetMove(board, moveX, moveY, inRange, diaLeftUp);
            }

            timesMoved++;
        }

        return inRange;
    }

    /// <summary>
    /// Changes the transform of the gameobject that this script is attached to.
    /// </summary>
    /// <param name="position">The new position to move.</param>
    public void Move(Vector3 position)
    {
        gameObject.transform.position = position + new Vector3(0, 0.02f, 0);
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
        if(!InBounds(board, indexX, indexY) || board[indexX, indexY].GetComponent<Cell>().GetCurrentPiece)
        {
            return true;
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
    private static void GetMove(GameObject[,] board, int coord, int offset, List<GameObject> inRange, bool canMove)
    {
        if (canMove && !IsPopulated(board, coord, offset) && !inRange.Contains(board[coord, offset]))
        {
            inRange.Add(board[coord, offset]);
            board[coord, offset].GetComponent<Cell>().IsHighlighted = true;
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
}
