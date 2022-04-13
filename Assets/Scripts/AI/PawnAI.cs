using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAI : BaseAI
{
    private void Awake()
    {
        this.PieceID = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.PieceID = 11;
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
            dangerLevel = 0;

            setValidActions();
            this.hasFinished = true;
        }
    }

    public void setValidActions()
    {
        int currCol = this.GetComponent<IPieceBase>().CurrRowPos;
        int currRow = this.GetComponent<IPieceBase>().CurrColPos;

        int[] validAction = new int[] { this.PieceID, currRow, currCol, currRow, currCol, 0};

        //add "no move" to the valid actions
        //validAction = new int[] { 21, currRow, currCol, currRow, currCol };
        //validActions.Add(validAction);

        for (int i = -1; i <= 1; i++)
        {
            if(isValid(currRow - 1, currCol + i))
            {
                var pieceDifference = AIManager.Board[currRow - 1, currCol + i] > 0 ? Mathf.Abs(AIManager.Board[currRow - 1, currCol + i] - this.PieceID) : -1;

                //can move
                if (AIManager.Board[currRow - 1, currCol + i] == 0)
                {
                    validAction = new int[] { this.PieceID, currRow, currCol, currRow - 1, currCol + i, 0};
                    validActions.Add(validAction);
                }
                
                if(this.AIManager.Board[currRow - 1, currCol + i] > 0 && pieceDifference >= 10)
                {
                    validAction = new int[] { this.PieceID, currRow, currCol, currRow - 1, currCol + i, 1};
                    validActions.Add(validAction);
                }
            }
        }

        UpdateProtectionMap(currRow, currCol, AIManager.Board);
        UpdateDangerMap(currRow, currCol, AIManager.Board);

        //AI.protectionBoard += protectionLevel;
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
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 5:
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (roll >= 6)
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
                if (roll >= 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 1:
                if (roll >= 4)
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
