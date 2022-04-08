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

        int row_limit = 7;
        int column_limit = 7;

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol, 0};
        validActions.Add(validAction);

        //search possible actions
        for (int x = Mathf.Max(0, currRow - 3); x <= Mathf.Min(currRow + 3, row_limit); x++)
        {
            for (int y = Mathf.Max(0, currCol - 3); y <= Mathf.Min(currCol + 3, column_limit); y++)
            {
                if (x != currRow || y != currCol)
                {
                    //check if move to spot is valid given movespeed of piece
                    int moves = isMoveValid(this.AIManager.Board, currRow, currCol, x, y);

                    // check possible moves
                    if (moves <= 3 && this.AIManager.Board[x, y] == 0)
                    {
                        moveFound = true;
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 0};
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    if (this.AIManager.Board[x, y] > 0 && Mathf.Abs(this.AIManager.Board[x, y] - this.PieceID) >= 10 &&
                        (x <= currRow + 1) && (y <= currCol + 1) && (x >= currRow - 1) && (y >= currCol - 1))
                    {
                        moveFound = true;
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 1 };
                        validActions.Add(validAction);
                    }

                    //UpdateProtectionMap
                   UpdateProtectionMap(currRow, currCol, this.AIManager.Board);
                }
            }
        }
        //AI.protectionBoard += protectionLevel;
        //print("King protection level: " + protectionLevel);
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
                return true;
        }
        return false;
    }
}
