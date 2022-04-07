using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAI : BaseAI
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

        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol };
        validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 4); x <= Mathf.Min(currRow + 4, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 4); y <= Mathf.Min(currCol + 4, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(this.AIManager.Board, currRow, currCol, x, y);

                    // check possible movies
                    if (moves <= 4 && this.AIManager.Board[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 0};
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    //since knight can move and attack in the same turn we can check through their whole move range
                    if (moves <= 4 && Mathf.Abs(this.AIManager.Board[x, y] - this.PieceID) >= 10)
                    {
                        moveFound = true;
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 1};
                        validActions.Add(validAction);
                    }

                    this.UpdateProtectionMap(currRow, currCol, this.AIManager.Board);
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("Knight protection level: " + protectionLevel);
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
