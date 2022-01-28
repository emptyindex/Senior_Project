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

    public List<GameObject> HighlightCells(GameObject[,] board, int x, int y, int maxTimes)
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
            GetMove(board, x - timesMoved, moveY, inRange);

            moveX = x + timesMoved;

            // move right
            GetMove(board, moveX, y, inRange);

            // move forward
            GetMove(board, x, moveY, inRange);

            // move diagonal down right
            GetMove(board, moveX, moveY, inRange);

            moveY = y - timesMoved;

            // move diagonal up right
            GetMove(board, moveX, moveY, inRange);

            // move backwards
            GetMove(board, x, moveY, inRange);

            moveX = x - timesMoved;

            // move left
            GetMove(board, moveX, y, inRange);

            // move diagonal up left
            GetMove(board, moveX, moveY, inRange);

            timesMoved++;
        }

        return inRange;
    }

    protected static void GetMove(GameObject[,] board, int coord, int offset, List<GameObject> inRange)
    {
        if (InBounds(board, coord, offset) && !inRange.Contains(board[coord, offset]) && !board[coord, offset].GetComponent<Cell>().GetCurrentPiece)
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
