using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAI : BaseAI
{
    // Start is called before the first frame update
    void Start()
    {
        this.PieceID = 11;
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

        for (int x = Mathf.Max(0, CurrRowPos - 1); x <= Mathf.Min(CurrRowPos + 1, row_limit); x++)
        {
            for (int y = Mathf.Max(0, CurrColPos - 1); y <= Mathf.Min(CurrColPos + 1, column_limit); y++)
            {
                if (x != CurrRowPos || y != CurrColPos)
                {
                    //print(x + " " + y);

                    //move is into an empty space
                    if (newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        newBoard[x, y] = 11;
                        newBoard[CurrRowPos, CurrColPos] = 0;
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
                        newBoard[CurrRowPos, CurrColPos] = 11;
                    }

                    //move is into enemy space
                    if ((newBoard[x,y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 || 
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6) && x > CurrRowPos)
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
                    }
                }
            }
        }
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
