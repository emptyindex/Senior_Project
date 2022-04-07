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
        if (!this.hasFinished)
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

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol };
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

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //straight up |
                if (direc == 1)
                {
                    int x = currRow;
                    int y = currCol + move;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //diagonal up right /
                if (direc == 2)
                {
                    int x = currRow + move;
                    int y = currCol + move;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //straight left 
                if (direc == 3)
                {
                    int x = currRow - move;
                    int y = currCol;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //straight right 
                if (direc == 4)
                {
                    int x = currRow + move;
                    int y = currCol;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //diagonal down left /
                if (direc == 5)
                {
                    int x = currRow - move;
                    int y = currCol - move;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //straight down 
                if (direc == 6)
                {
                    int x = currRow;
                    int y = currCol - move;

                    if (CheckActions(x, y) == -1)
                        break;
                }

                //diagonal down right \  
                if (direc == 7)
                {
                    int x = currRow + move;
                    int y = currCol - move;

                    if (CheckActions(x, y) == -1)
                        break;
                }
            }

            this.UpdateProtectionMap(currRow, currCol, this.AIManager.Board);
        }

        //AI.protectionBoard += protectionLevel;
        //print("Rook protection level: " + protectionLevel);
    }

    int CheckActions(int x, int y)
    {
        int currCol = this.GetComponent<IPieceBase>().CurrRowPos;
        int currRow = this.GetComponent<IPieceBase>().CurrColPos;

        //check moves
        if (isValid(x, y))
        {  
            if (this.AIManager.Board[x, y] == 0)
            {
                int[] validAction = new int[] { this.PieceID, currRow, currCol, x, y, 0};
                validActions.Add(validAction);

                return 1;
            }

            if (Mathf.Abs(AIManager.Board[x, y] - this.PieceID) >= 10)
            {
                moveFound = true;
                int[] validAction = new int[] { this.PieceID, currRow, currCol, x, y, 1};
                validActions.Add(validAction);

                return -1;
            }
        }

        return -1;
    }
}
