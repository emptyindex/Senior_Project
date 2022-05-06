using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.Jobs;
using Unity.Collections;

public class AI : BasePlayer
{
    //public GameObject rook, knight, bishop, queen, king, pawn;
    private bool doJob;

    [HideInInspector]
    public int[,] Board;
    [HideInInspector]
    public GameObject[,] BoardState;
    public GameObject[] Pieces;

    [HideInInspector]
    public float bestScore;
    [HideInInspector]
    public GameObject bestPieceOne;
    [HideInInspector]
    public GameObject bestPieceTwo;
    [HideInInspector]
    public GameObject bestPieceThree;
    [HideInInspector]
    public int[][] bestAction = new int[3][];
    [HideInInspector]
    public int[][] newBestAction = new int[3][];

    public static List<GameObject> BishopLPieces = new List<GameObject>();
    public static List<GameObject> BishopRPieces = new List<GameObject>();
    public static List<GameObject> KingPieces = new List<GameObject>();

    [HideInInspector]
    public List<int[][]> AiMoveList;

    public static List<GameObject> PlayerBishopLPieces = new List<GameObject>();
    public static List<GameObject> PlayerBishopRPieces = new List<GameObject>();
    public static List<GameObject> PlayerKingPieces = new List<GameObject>();

    [HideInInspector]
    public List<int[][]> PlayerMoveList;

    private int VariableDepth;

    public static bool AiTurn;

    private AttackManager attackManager;

    public static int protectionBoard;
    public static int dangerBoard;

    [HideInInspector]
    public int[] AssumeOverwritten1;
    [HideInInspector]
    public int[] AssumeOverwritten2;
    [HideInInspector]
    public int[] AssumeOverwritten3;

    [HideInInspector]
    public float certaintyOne;
    [HideInInspector]
    public float attackScoreOne;
    [HideInInspector]
    public float certaintyTwo;
    [HideInInspector]
    public float attackScoreTwo;
    [HideInInspector]
    public float certaintyThree;
    [HideInInspector]
    public float attackScoreThree;

    public static int knightOneRow = 0;
    public static int knightOneCol = 1;

    public static int knightTwoRow = 0;
    public static int knightTwoCol = 6;

    public NativeArray<int> result;
    private static GameObject attackingPiece;
    private static GameObject cellToAttack;
    private static int[][] storeBestAction = new int[3][];
    private static List<int> indexesDone = new List<int>();

    private static bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        doJob = false;

        AiTurn = true;

        AiMoveList = new List<int[][]>();
        PlayerMoveList = new List<int[][]>();

        newBestAction[0] = new int[] {0, 0, 0 };
        newBestAction[1] = new int[] { 0, 0, 0 };
        newBestAction[2] = new int[] { 0, 0, 0 };

        attackManager = gameObject.GetComponent<AttackManager>();

        attackManager.AttackRollNeeded += Manager.dice.Roll;
        Manager.dice.OnDiceEnded += CheckAttackSuccessful;

        StartCoroutine("AiPick");
    }

    private void CheckAttackSuccessful()
    {
        // Verify this is the correct AI instance to check
        if (attackingPiece != null && cellToAttack != null && Pieces.Contains(attackingPiece))
        {
            // get the piece being attacked
            var attackedPiece = cellToAttack.GetComponent<Cell>().GetCurrentPiece;

            // check whether the attack was successful on the attacked piece
            bool result = attackingPiece.GetComponent<IPieceBase>().IsAttackSuccessful(attackedPiece.GetComponent<IPieceBase>().PieceID, DiceNumberTextScript.diceNumber);    

            print(result);

            // if the attack was successful, remove the piece from the other player and move it into the deadpile
            // otherwise, end attacking
            if (result)
            {
                if(attackedPiece.GetComponent<IPieceBase>().PieceID == 26 || attackedPiece.GetComponent<IPieceBase>().PieceID == 6)
                {
                    Manager.EndGame(this.gameObject);
                    goto BREAK;
                }

                if(attackedPiece.GetComponent<Rigidbody>() == null)
                {
                    var rb = attackedPiece.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    rb.useGravity = true;
                }

                if(attackedPiece.GetComponent<BoxCollider>() == null)
                {
                    var _ = attackedPiece.AddComponent(typeof(BoxCollider)) as BoxCollider;
                }

                Manager.RemoveKilledPieceFromPlayer(this.gameObject, attackedPiece);

                MovePiece(attackingPiece, attackedPiece.GetComponent<IPieceBase>().CurrColPos, attackedPiece.GetComponent<IPieceBase>().CurrRowPos);

                // reset static variables
                attackingPiece = null;
                cellToAttack = null;
                storeBestAction = new int[3][];

                isAttacking = false;
            }
            else
            {
                // reset static variables
                attackingPiece = null;
                cellToAttack = null;
                storeBestAction = new int[3][];

                isAttacking = false;
            }
        }

    BREAK:;
    }

    public override void RemovePiece(GameObject pieceToRemove) 
    {
        List<GameObject> tempList = new List<GameObject>(Pieces);
        tempList.Remove(pieceToRemove);
        Pieces = tempList.ToArray();

        var commanderPiece = BishopLPieces.Contains(pieceToRemove);
        if (commanderPiece)
        {
            BishopLPieces.Remove(pieceToRemove);
            goto END;
        }

        commanderPiece = BishopRPieces.Contains(pieceToRemove);
        if (commanderPiece)
        {
            BishopRPieces.Remove(pieceToRemove);
            goto END;
        }

        commanderPiece = KingPieces.Contains(pieceToRemove);
        if (commanderPiece)
        {
            KingPieces.Remove(pieceToRemove);
        }

    END:;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //pawns are 1
    //rooks are 2
    //knights are 3
    //bishops are 4
    //queen is 5
    //king is 6
    //AI pieces have +10


    //coroutine
    IEnumerator AiPick()
    {
        while (!isGameOver)
        {
            if (this.canMove)
            {
                AiMoveList.Clear();
                PlayerMoveList.Clear();


                PlayerMoveFinder.findPlayerMoves = true;
                /*
                print("Left Bishop Corp:");
                foreach (GameObject thing in PlayerBishopLPieces)
                {
                    //    int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                    //    int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                    //    thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                    //    thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrColPos + ", " + thing.GetComponent<IPieceBase>().CurrRowPos);
                }
                print("Right Bishop Corp:");
                foreach (GameObject thing in PlayerBishopRPieces)
                {
                    //int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                    //int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                    //thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                    //thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrColPos + ", " + thing.GetComponent<IPieceBase>().CurrRowPos);
                }
                print("King Corp:");
                foreach (GameObject thing in PlayerKingPieces)
                {
                    //int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                    //int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                    //thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                    //thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrColPos + ", " + thing.GetComponent<IPieceBase>().CurrRowPos);
                }
                */
                protectionBoard = 0;
                dangerBoard = 0;
                Array.ForEach(Pieces, p => p.GetComponent<BaseAI>().hasFinished = false);

                //print("Current Danger = " + dangerBoard);

                yield return new WaitForSeconds(.15f);

                //print(protectionBoard);

                bestScore = -99999;
                int[] moveToMake = new int[2];

                //BaseAI pieceThatMoved = null;

                foreach (GameObject x in PlayerBishopLPieces)
                {
                    foreach (GameObject y in PlayerBishopRPieces)
                    {
                        foreach (GameObject z in PlayerKingPieces)
                        {
                            createMoveList(x, y, z, true);
                        }
                    }
                }
                foreach (GameObject x in BishopLPieces)
                {
                    foreach (GameObject y in BishopRPieces)
                    {
                        foreach (GameObject z in KingPieces)
                        {
                            createMoveList(x, y, z, false);
                        }

                    }
                }

                if (PlayerMoveList.Count + AiMoveList.Count > 20000)
                {
                    VariableDepth = 0;
                }

                if (PlayerMoveList.Count + AiMoveList.Count <= 20000)
                {
                    VariableDepth = 1;
                }

                print("debug");
                //loops through all the pieces in each corp commander so we can score their moves together
                //moves with highest score will be stored
                foreach (GameObject x in BishopLPieces)
                {
                    foreach (GameObject y in BishopRPieces)
                    {
                        foreach (GameObject z in KingPieces)
                        {
                            yield return checkCombinations(x, y, z);
                        }
                    }
                }

                //storeBestAction = bestAction;
                List<GameObject> pieces = new List<GameObject>() { bestPieceOne, bestPieceTwo, bestPieceThree };

                for(int i = 0; i < bestAction.GetLength(0); i++)
                {
                    if(!indexesDone.Contains(i))
                    {
                        GameManager.boardArr[bestAction[i][1], bestAction[i][0]].GetComponent<Cell>().IsAIHighlighted = true;
                        GameManager.boardArr[bestAction[i][1], bestAction[i][0]].GetComponent<Cell>().ChangeAIBorder();

                        yield return new WaitForSeconds(3f);

                        if (bestAction[i][2] == 0)
                        {
                            indexesDone.Add(i);
                            MovePiece(pieces[i], bestAction[i][0], bestAction[i][1]);
                        }
                        else
                        {
                            //int[][] newBestAction = new int[3][];

                            newBestAction[i][0] = bestAction[i][1];
                            newBestAction[i][1] = bestAction[i][0];
                            newBestAction[i][2] = bestAction[i][2];

                            indexesDone.Add(i);
                            BeginAttack(newBestAction, i, pieces[i]);

                            yield return new WaitUntil(() => !isAttacking);
                            //yield return new WaitForSeconds(10);
                        }
                    }
                }

                yield return new WaitForSeconds(3f);

                for (int i = 0; i < bestAction.GetLength(0); i++)
                {
                    GameManager.boardArr[bestAction[i][1], bestAction[i][0]].GetComponent<Cell>().IsAIHighlighted = false;
                }

                IsTurn(false);
                Manager.ChangeTurn(this.gameObject);

                indexesDone.Clear();
            }

            yield return null;
        }
    }

    private void BeginAttack(int[][] bestAction, int id, GameObject piece)
    {
        var cell = GameManager.boardArr[bestAction[id][0], bestAction[id][1]];

        if(cell.GetComponent<Cell>() && cell.GetComponent<Cell>().containsPiece)
        {
            cellToAttack = cell;
        }

        attackingPiece = piece;

        isAttacking = true;
        attackManager.InvokeAttackRoll();
    }

    private void MovePiece(GameObject piece, int x, int y)
    {
        var pieceBase = piece.GetComponent<IPieceBase>();

        piece.transform.position = Manager.GetMovePosition(y, x);

        Manager.UpdateIntBoard(pieceBase.CurrColPos, pieceBase.CurrRowPos, x, y, pieceBase.PieceID);

        piece.GetComponent<IProtectionBoard>().UpdateProtectionMap(x, y, Board);
        piece.GetComponent<IProtectionBoard>().UpdateDangerMap(x, y, Board);

        pieceBase.CurrColPos = x;
        pieceBase.CurrRowPos = y;

        print("piece " + pieceBase.PieceID + " has moved to: " + x + ", " + y + " and has danger of: " + piece.GetComponent<BaseAI>().dangerLevel);
    }

    //this method takes in 3 pieces (1 from each corp commander)
    //it will then loop through the valid actions that each piece can make and score it on a heuristic
    IEnumerator checkCombinations(GameObject pieceOne, GameObject pieceTwo, GameObject pieceThree)
    {
        float startTime;
        float endTime;
        float elapsedTime = 0;
        float desiredTime = 1f;

        for (int x = 0; x < pieceOne.GetComponent<BaseAI>().validActions.Count; x++)
        {

            for (int y = 0; y < pieceTwo.GetComponent<BaseAI>().validActions.Count; y++)
            {

                for (int z = 0; z < pieceThree.GetComponent<BaseAI>().validActions.Count; z++)
                {
                    startTime = Time.realtimeSinceStartup;

                    if (((pieceOne.GetComponent<BaseAI>().validActions[x][3] != pieceTwo.GetComponent<BaseAI>().validActions[y][3]) && (pieceOne.GetComponent<BaseAI>().validActions[x][4] != pieceTwo.GetComponent<BaseAI>().validActions[y][4])) ||
                        ((pieceOne.GetComponent<BaseAI>().validActions[x][3] != pieceThree.GetComponent<BaseAI>().validActions[z][3]) && (pieceOne.GetComponent<BaseAI>().validActions[x][4] != pieceThree.GetComponent<BaseAI>().validActions[z][4])) ||
                        ((pieceTwo.GetComponent<BaseAI>().validActions[y][3] != pieceThree.GetComponent<BaseAI>().validActions[z][3]) && (pieceTwo.GetComponent<BaseAI>().validActions[y][4] != pieceThree.GetComponent<BaseAI>().validActions[z][4])))
                    {
                        //temporary arrays to store actions
                        int[] actionOne = new int[3];
                        int[] actionTwo = new int[3];
                        int[] actionThree = new int[3];

                        //move each piece and store the action in the temp arrays
                        if (pieceOne.GetComponent<BaseAI>().validActions[x][5] == 0) 
                        {
                            MovePieceCheck(pieceOne, x, actionOne);
                            attackScoreOne = 0;
                            certaintyOne = 1;
                            //AssumeOverwritten1 = new int[] { pieceOne.GetComponent<BaseAI>().validActions[x][3], pieceOne.GetComponent<BaseAI>().validActions[x][4] };
                        }
                        if (pieceTwo.GetComponent<BaseAI>().validActions[y][5] == 0)
                        {
                            MovePieceCheck(pieceTwo, y, actionTwo);
                            attackScoreTwo = 0;
                            certaintyTwo = 1;
                            //AssumeOverwritten2 = new int[] { pieceTwo.GetComponent<BaseAI>().validActions[y][3], pieceTwo.GetComponent<BaseAI>().validActions[y][4] };
                        }
                        if (pieceThree.GetComponent<BaseAI>().validActions[z][5] == 0)
                        {
                            MovePieceCheck(pieceThree, z, actionThree);
                            attackScoreThree = 0;
                            certaintyThree = 1;
                            //AssumeOverwritten3 = new int[] { pieceThree.GetComponent<BaseAI>().validActions[z][3], pieceThree.GetComponent<BaseAI>().validActions[z][4] };
                        }

                        //attack with each piece and store the action in the temp arrays
                        if (pieceOne.GetComponent<BaseAI>().validActions[x][5] == 1)
                        {
                            float[] temp1 = AttackPieceCheck(pieceOne, x, actionOne, Board);
                            attackScoreOne = temp1[0];
                            certaintyOne = temp1[1];
                            //AssumeOverwritten1 = new int[] { pieceOne.GetComponent<BaseAI>().validActions[x][3], pieceOne.GetComponent<BaseAI>().validActions[x][4] };
                        }
                        if (pieceTwo.GetComponent<BaseAI>().validActions[y][5] == 1)
                        {
                            float[] temp2 = AttackPieceCheck(pieceTwo, y, actionTwo, Board);
                            attackScoreOne = temp2[0];
                            certaintyOne = temp2[1];
                            //AssumeOverwritten2 = new int[] { pieceTwo.GetComponent<BaseAI>().validActions[y][3], pieceTwo.GetComponent<BaseAI>().validActions[y][4] };
                        }
                        if (pieceThree.GetComponent<BaseAI>().validActions[z][5] == 1)
                        {
                            float[] temp3 = AttackPieceCheck(pieceThree, z, actionThree, Board);
                            attackScoreOne = temp3[0];
                            certaintyOne = temp3[1];
                            //AssumeOverwritten3 = new int[] { pieceThree.GetComponent<BaseAI>().validActions[z][3], pieceThree.GetComponent<BaseAI>().validActions[z][4] };
                        }

                        //update protection of pieces
                        UpdateProtectionMap(pieceOne, x);
                        UpdateProtectionMap(pieceTwo, y);
                        UpdateProtectionMap(pieceThree, z);

                        float newScore = minimax(Board, 0, false);
                        
                        if (newScore > bestScore)
                        {
                            bestScore = newScore;
                            bestPieceOne = pieceOne;
                            bestPieceTwo = pieceTwo;
                            bestPieceThree = pieceThree;
                            bestAction[0] = actionOne;
                            bestAction[1] = actionTwo;
                            bestAction[2] = actionThree;
                        }

                        //revert piece movement for all pieces
                        MovePieceBack(pieceOne, x);
                        MovePieceBack(pieceTwo, y);
                        MovePieceBack(pieceThree, z);

                        endTime = Time.realtimeSinceStartup;
                        elapsedTime += endTime - startTime;

                        if (elapsedTime >= desiredTime)
                        {
                            elapsedTime = 0;
                            yield return null;
                        }
                    }
                }
            }
        }

        yield return null;
    }

    //method that reverts a pieces movement back to its original position
    private void MovePieceBack(GameObject piece, int x)
    {
        Board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] = 0;
        Board[piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos] = piece.GetComponent<IPieceBase>().PieceID;
        piece.GetComponent<IProtectionBoard>().UpdateProtectionMap(piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos, Board);
    }

    //method to update protection
    //used for the heuristic
    private void UpdateProtectionMap(GameObject piece, int x)
    {
        piece.GetComponent<IProtectionBoard>().UpdateProtectionMap(piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4], Board);

        piece.GetComponent<IProtectionBoard>().UpdateDangerMap(piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4], Board);
    }

    //method that moves a piece to a new position and stores the action
    private void MovePieceCheck(GameObject piece, int x, int[] action)
    {
        Manager.UpdateIntBoard(piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos, piece.GetComponent<BaseAI>().validActions[x][3],
                               piece.GetComponent<BaseAI>().validActions[x][4], piece.GetComponent<IPieceBase>().PieceID);

        action[0] = piece.GetComponent<BaseAI>().validActions[x][3];
        action[1] = piece.GetComponent<BaseAI>().validActions[x][4];
        action[2] = piece.GetComponent<BaseAI>().validActions[x][5];
    }

    //method that moves a piece to a new position and stores the action
    private float[] AttackPieceCheck(GameObject piece, int x, int[] action, int[,] board)
    {
        //Manager.UpdateIntBoard(piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos, piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4], piece.GetComponent<IPieceBase>().PieceID);
        float tempCert = 1;
        float tempAttScore = 0;
        //check pawn
        if(piece.GetComponent<IPieceBase>().PieceID == 21)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 2;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .2f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .2f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .35f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .2f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .2f;
            }
        }

        //check rooks
        if (piece.GetComponent<IPieceBase>().PieceID == 22)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 1;
                tempCert = .35f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .35f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .35f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }

        //check knights
        if (piece.GetComponent<IPieceBase>().PieceID == 23)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 5;
                tempCert = 1f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .45f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .45f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .45f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .45f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .45f;
            }
        }

        //check bishops
        if (piece.GetComponent<IPieceBase>().PieceID == 24)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 2;
                tempCert = .1f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .1f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .1f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .1f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .35f;
            }
        }

        //check queen
        if (piece.GetComponent<IPieceBase>().PieceID == 25)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 2;
                tempCert = .9f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .35f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .5f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }

        //check king
        if (piece.GetComponent<IPieceBase>().PieceID == 26)
        {
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 1)
            {
                tempAttScore = 2;
                tempCert = .01f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 2)
            {
                tempAttScore = 10;
                tempCert = .01f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 3)
            {
                tempAttScore = 10;
                tempCert = .01f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 4)
            {
                tempAttScore = 50;
                tempCert = .01f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 5)
            {
                tempAttScore = 15;
                tempCert = .01f;
            }
            if (board[piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4]] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }
        float[] temp = new float[2];
        temp[0] = tempAttScore;
        temp[1] = tempCert;

        action[0] = piece.GetComponent<BaseAI>().validActions[x][3];
        action[1] = piece.GetComponent<BaseAI>().validActions[x][4];
        action[2] = piece.GetComponent<BaseAI>().validActions[x][5];

        return temp;
    }

    //method that moves a piece to a new position and stores the action
    private float[] AttackPieceCheckPlayer(int pieceID, int x, int y, int[,] board)
    {
        //Manager.UpdateIntBoard(piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos, piece.GetComponent<BaseAI>().validActions[x][3], piece.GetComponent<BaseAI>().validActions[x][4], piece.GetComponent<IPieceBase>().PieceID);
        float tempCert = 1;
        float tempAttScore = 0;
        //check pawn
        if (pieceID == 21)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 5;
                tempCert = .5f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 15;
                tempCert = .2f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 15;
                tempCert = .2f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .35f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .2f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .2f;
            }
        }

        //check rooks
        if (pieceID == 22)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 1;
                tempCert = .35f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 10;
                tempCert = .35f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 10;
                tempCert = .5f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .35f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .5f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }

        //check knights
        if (pieceID == 23)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 10;
                tempCert = 1f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 10;
                tempCert = .45f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 10;
                tempCert = .45f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .45f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .45f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .45f;
            }
        }

        //check bishops
        if (pieceID == 24)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 2;
                tempCert = .1f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 10;
                tempCert = .1f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 10;
                tempCert = .1f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .5f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .1f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .35f;
            }
        }

        //check queen
        if (pieceID == 25)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 2;
                tempCert = .9f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 10;
                tempCert = .35f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 10;
                tempCert = .5f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .5f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .5f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }

        //check king
        if (pieceID == 26)
        {
            if (board[x, y] == 1)
            {
                tempAttScore = 2;
                tempCert = .01f;
            }
            if (board[x, y] == 2)
            {
                tempAttScore = 10;
                tempCert = .01f;
            }
            if (board[x, y] == 3)
            {
                tempAttScore = 10;
                tempCert = .01f;
            }
            if (board[x, y] == 4)
            {
                tempAttScore = 50;
                tempCert = .01f;
            }
            if (board[x, y] == 5)
            {
                tempAttScore = 15;
                tempCert = .01f;
            }
            if (board[x, y] == 6)
            {
                tempAttScore = 15000;
                tempCert = .5f;
            }
        }
        float[] temp = new float[2];
        temp[0] = tempAttScore;
        temp[1] = tempCert;

        //action[0] = piece.GetComponent<BaseAI>().validActions[x][3];
        //action[1] = piece.GetComponent<BaseAI>().validActions[x][4];
        //action[2] = piece.GetComponent<BaseAI>().validActions[x][5];

        return temp;
    }

    //method to find the score of a given board
    //right now it only cares for how protected the board pieces are
    public int HeuristicScore(int[,] theBoard, int protection)
    {
        int score = protection;
        return score;
    }

    public float HeuristicScore(int[,] theBoard)
    {
        float score = protectionBoard - dangerBoard;
        score = (score + attackScoreOne) * certaintyOne;
        score = (score + attackScoreTwo) * certaintyTwo;
        score = (score + attackScoreThree) * certaintyThree;
        return score;
    }

    public override void SetPieces(List<GameObject> pieces)
    {
        this.Pieces = pieces.ToArray();
    }

    private void createMoveList(GameObject pieceOne, GameObject pieceTwo, GameObject pieceThree, bool isPlayer)
    {
        if (!isPlayer)
        {
            foreach (int[] x in pieceOne.GetComponent<BaseAI>().validActions)
            {
                foreach (int[] y in pieceTwo.GetComponent<BaseAI>().validActions)
                {
                    foreach (int[] z in pieceThree.GetComponent<BaseAI>().validActions)
                    {

                        int[][] moveToAdd = new int[3][];
                        moveToAdd[0] = x;
                        moveToAdd[1] = y;
                        moveToAdd[2] = z;

                        AiMoveList.Add(moveToAdd);
                    }
                }
            }
        }
        else
        {
            foreach (int[] x in pieceOne.GetComponent<BasePiece>().validActions)
            {
                foreach (int[] y in pieceTwo.GetComponent<BasePiece>().validActions)
                {
                    foreach (int[] z in pieceThree.GetComponent<BasePiece>().validActions)
                    {

                        int[][] moveToAdd = new int[3][];
                        moveToAdd[0] = x;
                        moveToAdd[1] = y;
                        moveToAdd[2] = z;

                        PlayerMoveList.Add(moveToAdd);
                    }
                }
            }
        }
    }


    float minimax(int[,] board, int depth, bool isMax)
    {
        //score the board state
        float score = HeuristicScore(board);

        // If Maximizer has won the game return his/her
        // evaluated score
        if (score >= 1000)
            return score;

        // If Minimizer has won the game return his/her
        // evaluated score
        if (score <= -1000)
            return score;

        if (depth == VariableDepth)
            return score;

        // If there are no more moves and no winner then
        // it is a tie
        //if (isMovesLeft(board) == false)
        //    return 0;

        // If this maximizer's move
        if (isMax)
        {
            float best = -10000000;
            //print("error");
            //traverse all possible moves
            foreach (int[][] moveSet in AiMoveList)
            {
                //makeMove

                //move 1
                if (board[moveSet[0][3], moveSet[0][4]] == 0)
                {
                    Manager.UpdateIntBoard(moveSet[0][2], moveSet[0][1], moveSet[0][3],
                                  moveSet[0][4], moveSet[0][0]);
                }
                else
                {
                    Manager.UpdateIntBoard(moveSet[0][2], moveSet[0][1], moveSet[0][3],
                                  moveSet[0][4], moveSet[0][0]);
                }

                //move 2
                if (board[moveSet[1][3], moveSet[1][4]] == 0)
                {
                    Manager.UpdateIntBoard(moveSet[1][2], moveSet[1][1], moveSet[1][3],
                              moveSet[1][4], moveSet[1][0]);
                }
                else
                {
                    Manager.UpdateIntBoard(moveSet[1][2], moveSet[1][1], moveSet[1][3],
                              moveSet[1][4], moveSet[1][0]);
                }

                //move 3
                if (board[moveSet[2][3], moveSet[2][4]] == 0)
                {
                    Manager.UpdateIntBoard(moveSet[2][2], moveSet[2][1], moveSet[2][3],
                              moveSet[2][4], moveSet[2][0]);
                }
                else
                {
                    Manager.UpdateIntBoard(moveSet[2][2], moveSet[2][1], moveSet[2][3],
                              moveSet[2][4], moveSet[2][0]);
                }

                //call minimax recursively
                best = Math.Max(best, minimax(board, depth + 1, !isMax));

                //undo the move


            }
            return best;
        }
        // If this minimizer's move
        else
        {
            float best = 10000000;

            if (doJob)
            {
                NativeArray<int> result = new NativeArray<int>(1, Allocator.TempJob);
                //NativeArray<int[]> moves = new NativeArray<int[]>(1, Allocator.TempJob);

                AiJob job = new AiJob
                {
                    //board = board,
                    result = result,
                };
                JobHandle JH = job.Schedule();

                JH.Complete();

                best = job.result[0];

                result.Dispose();

                return best;
            }
            else
            {
                foreach (int[][] moveSet in PlayerMoveList)
                {
                    //&& moveSet[0][1] != AssumeOverwritten1[0] && moveSet[0][2] != AssumeOverwritten1[1]
                    //makeMove
                    if (board[moveSet[0][3], moveSet[0][4]] == 0) 
                    { 
                        Manager.UpdateIntBoard(moveSet[0][1], moveSet[0][2], moveSet[0][3],
                                  moveSet[0][4], moveSet[0][0]);
                    }

                    if (board[moveSet[1][3], moveSet[1][4]] == 0 )
                    {
                        Manager.UpdateIntBoard(moveSet[1][1], moveSet[1][2], moveSet[1][3],
                                  moveSet[1][4], moveSet[1][0]);
                    }

                    if (board[moveSet[2][3], moveSet[2][4]] == 0)
                    {
                        Manager.UpdateIntBoard(moveSet[2][1], moveSet[2][2], moveSet[2][3],
                                  moveSet[2][4], moveSet[2][0]);
                    }

                    //or make attack
                    if (board[moveSet[0][3], moveSet[0][4]] != 0)
                    {
                        float[] temp1 = AttackPieceCheckPlayer(moveSet[0][0], moveSet[0][3], moveSet[0][4], board);
                        attackScoreOne = temp1[0];
                        certaintyOne = temp1[1];
                    }
                    if (board[moveSet[0][3], moveSet[0][4]] != 0)
                    {
                        float[] temp2 = AttackPieceCheckPlayer(moveSet[1][0], moveSet[1][3], moveSet[1][4], board);
                        attackScoreTwo = temp2[0];
                        certaintyTwo = temp2[1];
                    }
                    if (board[moveSet[0][3], moveSet[0][4]] != 0)
                    {
                        float[] temp3 = AttackPieceCheckPlayer(moveSet[2][0], moveSet[2][3], moveSet[2][4], board);
                        attackScoreThree = temp3[0];
                        certaintyThree = temp3[1];
                    }

                    //call minimax recursively
                    best = Math.Min(best, minimax(board, depth + 1, isMax));

                    //undo the move
                    Manager.UpdateIntBoard(moveSet[0][3], moveSet[0][4], moveSet[0][1],
                                  moveSet[0][2], moveSet[0][0]);

                    Manager.UpdateIntBoard(moveSet[1][3], moveSet[1][4], moveSet[1][1],
                                  moveSet[1][2], moveSet[1][0]);

                    Manager.UpdateIntBoard(moveSet[2][3], moveSet[2][4], moveSet[2][1],
                                  moveSet[2][2], moveSet[2][0]);

                }
            }
            return best;
        }
    }

    private JobHandle AiTaskJob()
    {
        AiJob job = new AiJob
        {
            //Manage = Manager,
            //PlayerMoves = PlayerMoveList,
            //board = Board,
            result = result,
        };
        return job.Schedule();
    }

    public override List<IPieceBase> GetPieces()
    {
        var bishopLPieces = BishopLPieces.ConvertAll(p => p.GetComponent<IPieceBase>());
        var bishopRPieces = BishopRPieces.ConvertAll(p => p.GetComponent<IPieceBase>());
        var kingPieces = KingPieces.ConvertAll(p => p.GetComponent<IPieceBase>());

        return new List<IPieceBase>(bishopLPieces.Concat(bishopRPieces).Concat(kingPieces));
    }
}

public struct AiJob : IJob
{
    //public GameManager Manage;
    //public List<int[][]> PlayerMoves;
    //public int[,] board;
    public NativeArray<int> result;

    public void Execute()
    {
        /*
        //makeMove
        Manage.UpdateIntBoard(moveSet[0][1], moveSet[0][2], moveSet[0][3],
                      moveSet[0][4], moveSet[0][0]);

        Manage.UpdateIntBoard(moveSet[1][1], moveSet[1][2], moveSet[1][3],
                      moveSet[1][4], moveSet[1][0]);

        Manage.UpdateIntBoard(moveSet[2][1], moveSet[2][2], moveSet[2][3],
                      moveSet[2][4], moveSet[2][0]);

        //call minimax recursively
        result[0] = HeuristicScore(board, 5);

        //undo the move
        Manage.UpdateIntBoard(moveSet[0][3], moveSet[0][4], moveSet[0][1],
                      moveSet[0][2], moveSet[0][0]);

        Manage.UpdateIntBoard(moveSet[1][3], moveSet[1][4], moveSet[1][1],
                      moveSet[1][2], moveSet[1][0]);

        Manage.UpdateIntBoard(moveSet[2][3], moveSet[2][4], moveSet[2][1],
                      moveSet[2][2], moveSet[2][0]);
        */
        //BaseAI.MethodForTest();
        result[0] = HeuristicScore(5);
    }

    public int HeuristicScore(int protection)
    {
        int score = protection;
        return score;
    }
}
