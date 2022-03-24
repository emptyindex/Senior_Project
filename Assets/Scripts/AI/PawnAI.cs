using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAI : BaseAI, IPieceBase, IProtectionBoard
{
    public int PieceID { get; set; } = 11;

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
            //BestMove();
            validActions.Clear();
            protectionLevel = 0;

            setValidActions();
            this.hasFinished = true;
        }
    }

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

        int[] validAction = new int [5];

        //add "no move" to the valid actions
        validAction = new int[] { 11, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        for (int i = -1; i <= 1; i++)
        {
            if (currRow - 1 > -1 && currCol + i < 8 && currCol + i > -1 && (newBoard[currRow - 1, currCol + i] == 0 || newBoard[currRow - 1, currCol + i] == 1 || newBoard[currRow - 1, currCol + i] == 2 ||
            newBoard[currRow - 1, currCol + i] == 3 || newBoard[currRow - 1, currCol + i] == 4 || newBoard[currRow - 1, currCol + i] == 5 ||
            newBoard[currRow - 1, currCol + i] == 6))
            {
                validAction = new int[] { 11, currRow, currCol, currRow - 1, currCol + i };
                validActions.Add(validAction);
                //print("found action");
            }
        }


        updateProtectionMap(currRow, currCol, AIManager.Board);

        AI.protectionBoard += protectionLevel;
        //print("Pawn protection level: " + protectionLevel);
        //int coun = 0;
        //for (int x = 0; x < validActions.Count; x++)
        //{
        //    print("pawn move " + coun + ": " + validActions[x][3] + ", " + validActions[x][4]);
        //    coun++;
        //}
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
