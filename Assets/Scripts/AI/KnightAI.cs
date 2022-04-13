using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : BaseAI, IPieceBase, IProtectionBoard
{
    //public int PieceID { get; set; } = 13;

    public List<int[,]> protMaps = new List<int[,]>();
    public int[,] protectionMapKnight;

    private void Awake()
    {
        this.PieceID = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.PieceID = 13;

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
        validAction = new int[] { 23, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 2); x <= Mathf.Min(currRow + 2, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 2); y <= Mathf.Min(currCol + 2, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(newBoard, currRow, currCol, x, y);

                    if (moves <= 4 && newBoard[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { 23, currRow, currCol, x, y };
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we can check through their whole move range
                    if (moves <= 4 && (newBoard[x, y] == 1 || newBoard[x, y] == 2 || newBoard[x, y] == 3 ||
                        newBoard[x, y] == 4 || newBoard[x, y] == 5 || newBoard[x, y] == 6))
                    {
                        moveFound = true;
                        validAction = new int[] { 23, currRow, currCol, x, y };
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
        //print("Knight protection level: " + protectionLevel);
    }

    public void UpdateProtectionMap(int row, int col, int[,] board)
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
                        (board[x, y] == 23 || board[x, y] == 24 || board[x, y] == 25 || board[x, y] == 26))
                    {
                        protectionLevel += 1;
                    }

                    //check protection by pawn since they can only protect from behind
                    if (x <= row + 1 && x > row && y <= col + 1 && y >= col - 1 && board[x, y] == 21)
                    {
                        protectionLevel += 1;
                    }

                    //check protection by rook since they have a range of 2
                    if (board[x, y] == 22)
                    {
                        protectionLevel += 1;
                    }
                }
            }
        }
        AI.protectionBoard += protectionLevel;
    }

    public void revertProtectionMap()
    {
        throw new System.NotImplementedException();
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
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 5:
                if (roll >= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (roll >= 5)
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
}
