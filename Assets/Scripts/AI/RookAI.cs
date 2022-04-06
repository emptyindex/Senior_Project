using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookAI : BaseAI
{
    private void Awake()
    {
        this.PieceID = 2;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        validAction = new int[] { 12, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        //check moves
        //moves must be in a straight line
        for (int direc = 0; direc < 8; direc++)
        {
            for (int move = 1; move <= 2; move++)
            {
                //diagonal up left \
                if (direc == 0)
                {
                    int x = currRow - move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //straight up |
                if (direc == 1)
                {
                    int x = currRow;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal up right /
                if (direc == 2)
                {
                    int x = currRow + move;
                    int y = currCol + move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //straight left 
                if (direc == 3)
                {
                    int x = currRow - move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //straight right 
                if (direc == 4)
                {
                    int x = currRow + move;
                    int y = currCol;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down left /
                if (direc == 5)
                {
                    int x = currRow - move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //straight down 
                if (direc == 6)
                {
                    int x = currRow;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }

                //diagonal down right \  
                if (direc == 7)
                {
                    int x = currRow + move;
                    int y = currCol - move;

                    int isBlocked = CheckActions(x, y, move, newBoard);
                    if (isBlocked == -1)
                        break;
                }
            }
        }

        //search possible attacks
        //attacks can be up to 2 spaces away and do not need to be in a straight line
        for (int x = Mathf.Max(0, currRow - 2); x <= Mathf.Min(currRow + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, CurrColPos - 2); y <= Mathf.Min(CurrColPos + 2, column_limit); y++)
            {
                if (x != CurrRowPos || y != CurrColPos)
                {

                    //check possible attacks
                    //since rook has the same move speed as attack range we can just check if the spot we found has an enemy on it
                    if (newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6)
                    {
                        validAction = new int[] { 12, currRow, currCol, x, y };
                        validActions.Add(validAction);
                    }

                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 13 || newBoard[x, y] == 14 || newBoard[x, y] == 15 || newBoard[x, y] == 16))
                    {
                        protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= currRow + 1 && x > currRow && y <= currCol + 1 && y >= currCol - 1 && newBoard[x, y] == 11)
                    {
                        protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (x <= currRow + 2 && x >= currRow - 2 && y <= currCol + 2 && y >= currCol - 2 && newBoard[x, y] == 12)
                    {
                        protectionLevel += 1;
                    }
                }
            }
        }
        AI.protectionBoard += protectionLevel;
        //print("Rook protection level: " + protectionLevel);
    }

    int CheckActions(int x, int y, int move, int[,] newBoard)
    {
        int currCol = this.GetComponent<IPieceBase>().CurrRowPos;
        int currRow = this.GetComponent<IPieceBase>().CurrColPos;

        //check moves
        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] == 0)
        {
            int[] validAction = new int[] { 22, currRow, currCol, x, y };
            validActions.Add(validAction);
        }

        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] != 0)
            return -1;
        else
            return 1;
    }
}
