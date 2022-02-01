using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePiece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(GameObject[,] board, int i, int j)
    {
        
    }

    public abstract List<GameObject> Highlight(GameObject[,] board, int x, int y);

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

    private static bool IsPopulated(GameObject[,] board, int indexX, int indexY)
    {
        if(!InBounds(board, indexX, indexY) || board[indexX, indexY].GetComponent<Cell>().GetCurrentPiece)
        {
            return true;
        }

        return false;
    }

    private static void GetMove(GameObject[,] board, int coord, int offset, List<GameObject> inRange, bool canMove)
    {
        if (canMove && !IsPopulated(board, coord, offset) && !inRange.Contains(board[coord, offset]))
        {
            inRange.Add(board[coord, offset]);
            board[coord, offset].GetComponent<Cell>().IsHighlighted = true;
        }
    }

    protected static bool InBounds(GameObject[,] board, int indexX, int indexY)
    {
        bool temp = indexX > -1 && indexX < board.GetLength(0);

        return temp && indexY < board.GetLength(1) && indexY > -1;
    }
}
