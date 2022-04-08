using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class AI : BasePlayer
{
    //public GameObject rook, knight, bishop, queen, king, pawn;

    [HideInInspector]
    public int[,] Board;
    [HideInInspector]
    public GameObject[,] BoardState;
    public GameObject[] Pieces;

    public int bestScore;
    public GameObject bestPieceOne;
    public GameObject bestPieceTwo;
    public GameObject bestPieceThree;
    public int[][] bestAction = new int[3][];

    public static List<GameObject> BishopLPieces = new List<GameObject>();
    public static List<GameObject> BishopRPieces = new List<GameObject>();
    public static List<GameObject> KingPieces = new List<GameObject>();
    public List<int[][]> AiMoveList;

    public static List<GameObject> PlayerBishopLPieces = new List<GameObject>();
    public static List<GameObject> PlayerBishopRPieces = new List<GameObject>();
    public static List<GameObject> PlayerKingPieces = new List<GameObject>();
    public List<int[,]> PlayerMoveList;

    public static bool AiTurn;

    private AttackManager attackManager;

    private GameObject deadPile;

    public static int protectionBoard;
    public static int dangerBoard;

    private static GameObject attackingPiece;
    private static GameObject cellToAttack;
    private static int[][] storeBestAction = new int[3][];
    private static List<int> indexesDone = new List<int>();

    private static bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        AiTurn = true;

        AiMoveList = new List<int[][]>();

        attackManager = gameObject.GetComponent<AttackManager>();

        attackManager.AttackRollNeeded += Manager.dice.Roll;
        Manager.dice.OnDiceEnded += CheckAttackSuccessful;

        deadPile = GameObject.FindWithTag("Deadpile");

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
                attackedPiece.transform.SetParent(deadPile.transform);
                attackedPiece.transform.position = deadPile.transform.position + new Vector3(0, 3, 0);

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
                PlayerMoveFinder.findPlayerMoves = true;

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


                protectionBoard = 0;
                Array.ForEach(Pieces, p => p.GetComponent<BaseAI>().hasFinished = false);

                //PlayerMoveFinder.findPlayerMoves = true;

                yield return new WaitForSeconds(.5f);

                //print(protectionBoard);

                bestScore = -99999;
                int[] moveToMake = new int[2];

                //BaseAI pieceThatMoved = null;

                //loops through all the pieces in each corp commander so we can score their moves together
                //moves with highest score will be stored
                foreach (GameObject x in BishopLPieces)
                {
                    foreach (GameObject y in BishopRPieces)
                    {
                        foreach (GameObject z in KingPieces)
                        {
                            createMoveList(x, y, z, true);
                            checkCombinations(x, y, z);
                        }
                    }
                }

                storeBestAction = bestAction;
                List<GameObject> pieces = new List<GameObject>() { bestPieceOne, bestPieceTwo, bestPieceThree };

                for(int i = 0; i < storeBestAction.GetLength(0); i++)
                {
                    if(!indexesDone.Contains(i))
                    {
                        if (storeBestAction[i][2] == 0)
                        {
                            indexesDone.Add(i);
                            MovePiece(pieces[i], storeBestAction[i][0], storeBestAction[i][1]);
                        }
                        else
                        {
                            indexesDone.Add(i);
                            BeginAttack(storeBestAction, i, pieces[i]);

                            yield return new WaitUntil(() => !isAttacking);
                        }
                    }
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

        var previousCell = GameManager.boardArr[pieceBase.CurrRowPos, pieceBase.CurrColPos].GetComponent<Cell>();
        previousCell.GetCurrentPiece = null;

        var cell = GameManager.boardArr[y, x].GetComponent<Cell>();
        cell.GetCurrentPiece = piece;

        piece.GetComponent<IProtectionBoard>().UpdateProtectionMap(x, y, Board);

        pieceBase.CurrColPos = x;
        pieceBase.CurrRowPos = y;

        print("piece " + pieceBase.PieceID + " has moved to: " + x + ", " + y + " and has protection of: " + piece.GetComponent<BaseAI>().protectionLevel);
    }

    //this method takes in 3 pieces (1 from each corp commander)
    //it will then loop through the valid actions that each piece can make and score it on a heuristic
    public void checkCombinations(GameObject pieceOne, GameObject pieceTwo, GameObject pieceThree)
    {
        for (int x = 0; x < pieceOne.GetComponent<BaseAI>().validActions.Count; x++)
        {

            for (int y = 0; y < pieceTwo.GetComponent<BaseAI>().validActions.Count; y++)
            {

                for (int z = 0; z < pieceThree.GetComponent<BaseAI>().validActions.Count; z++)
                {
                    if (((pieceOne.GetComponent<BaseAI>().validActions[x][3] != pieceTwo.GetComponent<BaseAI>().validActions[y][3]) && (pieceOne.GetComponent<BaseAI>().validActions[x][4] != pieceTwo.GetComponent<BaseAI>().validActions[y][4])) ||
                        ((pieceOne.GetComponent<BaseAI>().validActions[x][3] != pieceThree.GetComponent<BaseAI>().validActions[z][3]) && (pieceOne.GetComponent<BaseAI>().validActions[x][4] != pieceThree.GetComponent<BaseAI>().validActions[z][4])) ||
                        ((pieceTwo.GetComponent<BaseAI>().validActions[y][3] != pieceThree.GetComponent<BaseAI>().validActions[z][3]) && (pieceTwo.GetComponent<BaseAI>().validActions[y][4] != pieceThree.GetComponent<BaseAI>().validActions[z][4])))
                    {
                        //temporary arrays to store actions
                        int[] actionOne = new int[3];
                        int[] actionTwo = new int[3];
                        int[] actionThree = new int[3];

                        //move each piece and store the action in the temp arrays
                        MovePieceCheck(pieceOne, x, actionOne);
                        MovePieceCheck(pieceTwo, y, actionTwo);
                        MovePieceCheck(pieceThree, z, actionThree);

                        //update protection of pieces
                        UpdateProtectionMap(pieceOne, x);
                        UpdateProtectionMap(pieceTwo, y);
                        UpdateProtectionMap(pieceThree, z);

                        int newScore = HeuristicScore(Board, protectionBoard);

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
                    }
                }
            }
        }
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

    //method to find the score of a given board
    //right now it only cares for how protected the board pieces are
    public int HeuristicScore(int[,] theBoard, int protection)
    {
        int score = protection;
        return score;
    }

    public int HeuristicScore(int[,] theBoard)
    {
        int score = protectionBoard;
        return score;
    }

    public override void SetPieces(List<GameObject> pieces)
    {
        this.Pieces = pieces.ToArray();
    }

    private void createMoveList(GameObject pieceOne, GameObject pieceTwo, GameObject pieceThree, bool isPlayer)
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


    int minimax(int[,] board, int depth, bool isMax)
    {
        //score the board state
        int score = HeuristicScore(board);

        // If Maximizer has won the game return his/her
        // evaluated score
        if (score >= 10000)
            return score;

        // If Minimizer has won the game return his/her
        // evaluated score
        if (score <= -10000)
            return score;

        if (depth == 2)
            return score;

        // If there are no more moves and no winner then
        // it is a tie
        //if (isMovesLeft(board) == false)
        //    return 0;

        // If this maximizer's move
        if (isMax)
        {
            int best = -10000000;

            //traverse all possible moves
            foreach (GameObject pieceOne in BishopLPieces)
            {
                foreach (GameObject pieceTwo in BishopRPieces)
                {
                    foreach (GameObject pieceThree in KingPieces)
                    {

                        for (int x = 0; x < pieceOne.GetComponent<BaseAI>().validActions.Count; x++)
                        {

                            for (int y = 0; y < pieceTwo.GetComponent<BaseAI>().validActions.Count; y++)
                            {

                                for (int z = 0; z < pieceThree.GetComponent<BaseAI>().validActions.Count; z++)
                                {
                                    //makeMove

                                    //call minimax recursively
                                    best = Math.Max(best, minimax(board, depth + 1, !isMax));

                                    //undo the move

                                }
                            }
                        }
                    }
                }
            }
            return best;
        }

        // If this minimizer's move
        else
        {
            int best = 10000000;

            //traverse all possible moves
            foreach (GameObject pieceOne in BishopLPieces)
            {
                foreach (GameObject pieceTwo in BishopRPieces)
                {
                    foreach (GameObject pieceThree in KingPieces)
                    {

                        for (int x = 0; x < pieceOne.GetComponent<BaseAI>().validActions.Count; x++)
                        {

                            for (int y = 0; y < pieceTwo.GetComponent<BaseAI>().validActions.Count; y++)
                            {

                                for (int z = 0; z < pieceThree.GetComponent<BaseAI>().validActions.Count; z++)
                                {
                                    //makeMove

                                    //call minimax recursively
                                    best = Math.Min(best, minimax(board, depth + 1, !isMax));

                                    //undo the move

                                }
                            }
                        }
                    }
                }
            }
            return best;
        }
    }

    public override void RemovePiece(GameObject pieceToRemove)
    {
        throw new NotImplementedException();
    }
}
