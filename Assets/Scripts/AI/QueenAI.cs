using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAI : BaseAI
{
    private void Awake()
    {
        this.PieceID = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.PieceID = 15;

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

        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol, 0};

        //add "no move" to the valid actions
        //validAction = new int[] { 15, currRow, currCol, currRow, currCol };
        //validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, CurrColPos - 3); y <= Mathf.Min(CurrColPos + 3, column_limit); y++)
            {
                if (x != CurrRowPos || y != CurrColPos)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(this.AIManager.Board, currRow, currCol, x, y);

                    if (moves <= 3 && this.AIManager.Board[x, y] == 0)
                    {
                        moveFound = true;
                        float howGood = AssumeGoodness(currRow, currCol, this.AIManager.Board);
                        if (howGood > -5)
                        {
                            validAction = new int[] { this.PieceID, currRow, currCol, x, y, 0};
                            validActions.Add(validAction);
                        }
                    }

                    //check possible attacks
                    if (this.AIManager.Board[x, y] > 0 && Mathf.Abs(this.AIManager.Board[x, y] - this.PieceID) >= 15 &&
                        (x <= CurrRowPos + 1) && (y <= CurrColPos + 1) && (x >= CurrRowPos - 1) && (y >= CurrColPos - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 1};
                        validActions.Add(validAction);
                    }

                    //update protection map
                    this.UpdateProtectionMap(currRow, currCol, this.AIManager.Board);
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Queen protection level: " + protectionLevel);
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
                if (roll >= 4)
                {
                    return true;
                    //game over
                }
                else
                {
                    return false;
                }
            case 5:
                if (roll >= 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (roll >= 4)
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
                    //bishop's pieces relegated to King, King's command authority lost
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
                if (roll >= 2)
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

    private float AssumeGoodness(int row, int col, int[,] board)
    {
        int row_limit = 7;
        int column_limit = 7;

        float goodness = 0;
        float badness = 0;

        for (int x = Mathf.Max(0, row - 3); x <= Mathf.Min(row + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, col - 3); y <= Mathf.Min(col + 3, column_limit); y++)
            {
                if (board[x, y] == 21 || board[x, y] == 22 || board[x, y] == 23 || board[x, y] == 24 || board[x, y] == 25)
                {
                    goodness += 1;
                }

                if (board[x, y] == 24)
                {
                    goodness += 2;
                }

                if (board[x, y] == 26)
                {
                    goodness += 4;
                }

                if (x <= row + 2 && x >= row - 2 && y <= col + 2 && y >= col - 2 && (board[x, y] == 1 || board[x, y] == 3 || board[x, y] == 4))
                {
                    badness += 1;
                }

                if (x <= row + 2 && x >= row - 2 && y <= col + 2 && y >= col - 2 && (board[x, y] == 2 || board[x, y] == 5))
                {
                    badness += 2;
                }
            }
        }

        return goodness - badness;
    }
}
