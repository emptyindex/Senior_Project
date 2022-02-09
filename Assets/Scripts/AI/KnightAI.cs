using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : BaseAI, IPieceBase
{
    public int PieceID { get; set; } = 13;
    // Start is called before the first frame update
    void Start()
    {
        //BestMove();
        this.hasFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.hasFinished == false)
        {
            BestMove();
            this.hasFinished = true;
        }
    }

    //method that loops through all possible moves a piece can make and scores them
    void BestMove()
    {
        bestScore = -999999999;
        //int nextRow = 0;
        //int nextCol = 0;

        int currScore;
        int[] currAttack = new int[2];
        bestMove = new int[2];

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

        //search possible moves
        for (int x = Mathf.Max(0, currRow - 4); x <= Mathf.Min(currRow + 4, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 4); y <= Mathf.Min(currCol + 4, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);
                    if (moves <= 4)
                    {
                        moveFound = true;
                        newBoard[x, y] = 13;
                        newBoard[currRow, currCol] = 0;
                        currAttack[0] = -1;
                        currAttack[1] = -1;
                        currScore = HeuristicScore(newBoard, currAttack);
                        if (currScore > bestScore)
                        {
                            bestScore = currScore;
                            bestMove[0] = x;
                            bestMove[1] = y;
                        }
                        newBoard[x, y] = 0;
                        newBoard[currRow, currCol] = 13;
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we have to do wierd stuff
                    //not implemented all the way yet, right now the knight attacks only spaces around it
                    //also knights can only attack adjacent pieces
                    if ((newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6) && 
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        currAttack[0] = x;
                        currAttack[1] = y;
                        currScore = HeuristicScore(newBoard, currAttack);
                        if (currScore > bestScore)
                        {
                            bestScore = currScore;
                            bestMove[0] = x;
                            bestMove[1] = y;
                        }
                        //print("Knight Attack");
                    }
                }
            }
        }

        //if (moveFound == false)
        //    print("knight no move");
    }
}
