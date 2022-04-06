using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingAI : BaseAI
{
    //public int PieceID { get; set; } = 16;

    private void Awake()
    {
        this.PieceID = 6;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.PieceID = 16;
        //BestMove();
        this.hasFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.hasFinished == false)
        {
            //BestMove();
            validActions.Clear();
            protectionLevel = 0;

            setValidActions();
            this.hasFinished = true;
        }
    }

    public void setValidActions()
    {
        int currCol = this.GetComponent<IPieceBase>().CurrRowPos;
        int currRow = this.GetComponent<IPieceBase>().CurrColPos;

        int[,] newBoard = new int[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                newBoard[i, j] = this.AIManager.Board[i, j];
            }
        }

        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[5];

        //add "no move" to the valid actions
        validAction = new int[] { 16, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 3); y <= Mathf.Min(currCol + 3, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);
                    if (moves <= 3 && newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { 26, currRow, currCol, x, y };
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    if ((newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6) &&
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { 26, currRow, currCol, x, y };
                        validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 23 || newBoard[x, y] == 24 || newBoard[x, y] == 25 || newBoard[x, y] == 26))
                    {
                        protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 21)
                    {
                        protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 22)
                    {
                        protectionLevel += 1;
                    }
                }
            }
        }
        AI.protectionBoard += protectionLevel;
        //print("King protection level: " + protectionLevel);
    }
}
