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

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol };

        validActions.Add(validAction);

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
                        validAction = new int[] { this.PieceID, currRow, currCol, x, y, 0};
                        validActions.Add(validAction);
                    }

                    //check possible attacks
                    if (Mathf.Abs(this.AIManager.Board[x, y] - this.PieceID) >= 10 &&
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
}
