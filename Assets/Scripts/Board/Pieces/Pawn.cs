using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : BasePiece
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public List<GameObject> HighlightCells(GameObject[,] board, int x, int y)
    {
        List<GameObject> inRange = new List<GameObject>();

        int timesMoved = 1;
        int maxTimes = 2;

        int moveY;
        int moveX;

        while (timesMoved < maxTimes)
        {
            moveY = y + timesMoved;

            // get diagonal down left
            GetMove(board, x - timesMoved, moveY, inRange);

            moveX = x + timesMoved;

            // move forward
            GetMove(board, x, moveY, inRange);

            // move diagonal down right
            GetMove(board, moveX, moveY, inRange);

            timesMoved++;
        }

        return inRange;
    }
}
