using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : BaseAI, IPieceBase, IProtectionBoard
{
    public int PieceID { get; set; } = 13;

    public List<int[,]> protMaps = new List<int[,]>();
    public int[,] protectionMapKnight;

    // Start is called before the first frame update
    void Start()
    {
        this.PieceID = 13;

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

    //method that loops through all possible moves a piece can make and scores them
    /*
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
        for (int x = Mathf.Max(0, CurrRowPos - 4); x <= Mathf.Min(CurrRowPos + 4, row_limit); x++)
        {
            for (int y = Mathf.Max(0, CurrColPos - 4); y <= Mathf.Min(CurrColPos + 4, column_limit); y++)
            {
                if (x != CurrRowPos || y != CurrColPos)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, CurrRowPos, CurrColPos, x, y);
                    if (moves <= 4)
                    {
                        moveFound = true;
                        newBoard[x, y] = 13;
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
                        newBoard[CurrRowPos, CurrColPos] = 13;
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we have to do wierd stuff
                    //not implemented all the way yet, right now the knight attacks only spaces around it
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
    }
    */
    public void setValidActions()
    {
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
        validAction = new int[] { 13, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 4); x <= Mathf.Min(currRow + 4, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 4); y <= Mathf.Min(currCol + 4, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);

                    if (moves <= 4 && newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { 13, currRow, currCol, x, y };
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we can check through their whole move range
                    if (moves <= 4 && (newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        moveFound = true;
                        validAction = new int[] { 13, currRow, currCol, x, y };
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
        //print("Knight protection level: " + protectionLevel);
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
