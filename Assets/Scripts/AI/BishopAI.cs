using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopAI : BaseAI, IPieceBase, IProtectionBoard
{
    //public int PieceID { get; set; } = 14;

    int currScore;
    int[] currAttack = new int[2];

    private void Awake()
    {
        this.PieceID = 4;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.PieceID = 14;

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
        int currCol = this.GetComponent<IPieceBase>().CurrColPos;
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
        validAction = new int[] { 15, currRow, currCol, currRow, currCol };
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
        for (int x = Mathf.Max(0, currRow - 2); x <= Mathf.Min(currRow + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, CurrColPos - 2); y <= Mathf.Min(CurrColPos + 2, column_limit); y++)
            {
                if (x != CurrRowPos || y != CurrColPos)
                {
                    //check possible attacks
                    if (x <= currRow + 1 && x >= currRow - 1 && y <= currCol + 1 && y >= currCol - 1 &&
                        (newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6) &&
                        (x <= CurrRowPos + 1) && (y <= CurrColPos + 1) && (x >= CurrRowPos - 1) && (y >= CurrColPos - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { 14, currRow, currCol, x, y };
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
        //print("Bishop protection level: " + protectionLevel);
    }

    int CheckActions(int x, int y, int move, int[,] newBoard)
    {
        int currCol = this.GetComponent<IPieceBase>().CurrColPos;
        int currRow = this.GetComponent<IPieceBase>().CurrColPos;

        //check moves
        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] == 0)
        {
            int[] validAction = new int[] { 14, currRow, currCol, x, y };
            validActions.Add(validAction);
        }

        if (x < 8 && y < 8 && x > -1 && y > -1 && newBoard[x, y] != 0)
            return -1;
        else
            return 1;
    }

    public void updateProtectionMap(int row, int col, int[,] board)
    {
        int row_limit = 7;
        int column_limit = 7;

        AI.protectionBoard -= protectionLevel;
        protectionLevel = 0;

        for (int x = Mathf.Max(0, row - 2); x <= Mathf.Min(row + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, col - 2); y <= Mathf.Min(col + 2, column_limit); y++)
            {
                if (board[x, y] != 0 && x != row || y != col)
                {
                    //check protection by bishop, queen, and king since they all have the same attack range
                    if (x <= row + 1 && x >= row - 1 && y <= col + 1 && y >= col - 1 &&
                        (board[x, y] == 13 || board[x, y] == 14 || board[x, y] == 15 || board[x, y] == 16))
                    {
                        protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= row + 1 && x > row && y <= col + 1 && y >= col - 1 && board[x, y] == 11)
                    {
                        protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (board[x, y] == 12)
                    {
                        protectionLevel += 1;
                    }
                }
            }
        }
        AI.protectionBoard += protectionLevel;
    }
}
