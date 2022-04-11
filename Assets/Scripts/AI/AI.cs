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
    public List<int[][]> PlayerMoveList;

    public static bool AiTurn;

    public static int protectionBoard;
    public static int dangerBoard;

    public static int kingRow = 7;
    public static int kingCol = 3;
    //public static int BishopRRow = 7;
    //public static int BishopRCol = 2;
    //public static int BishopLRow = 7;
    //public static int BishopLCol = 5;

    public NativeArray<int> result;

    // Start is called before the first frame update
    void Start()
    {
        doJob = false;

        AiTurn = true;

        AiMoveList = new List<int[][]>();
        PlayerMoveList = new List<int[][]>();

        StartCoroutine("AiPick");
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
                print("debug");
                //loops through all the pieces in each corp commander so we can score their moves together
                //moves with highest score will be stored
                foreach (GameObject x in BishopLPieces)
                {
                    foreach (GameObject y in BishopRPieces)
                    {
                        foreach (GameObject z in KingPieces)
                        {
                            //createMoveList(x, y, z, false);
                            yield return checkCombinations(x, y, z);
                        }
                    }
                }           

                //move pieces to best found move
                bestPieceOne.gameObject.transform.position = Manager.GetMovePosition(bestAction[0][1], bestAction[0][0]);
                bestPieceTwo.gameObject.transform.position = Manager.GetMovePosition(bestAction[1][1], bestAction[1][0]);
                bestPieceThree.gameObject.transform.position = Manager.GetMovePosition(bestAction[2][1], bestAction[2][0]);

                //update the integer board
                Manager.UpdateIntBoard(bestPieceOne.GetComponent<IPieceBase>().CurrColPos, bestPieceOne.GetComponent<IPieceBase>().CurrRowPos, bestAction[0][0], bestAction[0][1], bestPieceOne.GetComponent<IPieceBase>().PieceID);
                Manager.UpdateIntBoard(bestPieceTwo.GetComponent<IPieceBase>().CurrColPos, bestPieceTwo.GetComponent<IPieceBase>().CurrRowPos, bestAction[1][0], bestAction[1][1], bestPieceTwo.GetComponent<IPieceBase>().PieceID);
                Manager.UpdateIntBoard(bestPieceThree.GetComponent<IPieceBase>().CurrColPos, bestPieceThree.GetComponent<IPieceBase>().CurrRowPos, bestAction[2][0], bestAction[2][1], bestPieceThree.GetComponent<IPieceBase>().PieceID);

                //update the protection map
                bestPieceOne.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[0][0], bestAction[0][1], Board);
                bestPieceTwo.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[1][0], bestAction[1][1], Board);
                bestPieceThree.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[2][0], bestAction[2][1], Board);

                //update the moved pieces current row and column
                bestPieceOne.GetComponent<IPieceBase>().CurrColPos = bestAction[0][0];
                bestPieceOne.GetComponent<IPieceBase>().CurrRowPos = bestAction[0][1];

                bestPieceTwo.GetComponent<IPieceBase>().CurrColPos = bestAction[1][0];
                bestPieceTwo.GetComponent<IPieceBase>().CurrRowPos = bestAction[1][1];

                bestPieceThree.GetComponent<IPieceBase>().CurrColPos = bestAction[2][0];
                bestPieceThree.GetComponent<IPieceBase>().CurrRowPos = bestAction[2][1];

                //print to the consol the piece that moved and where it moved
                print("piece " + bestPieceOne.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[0][0] + ", " + bestAction[0][1] + " and has protection of: " + bestPieceOne.GetComponent<BaseAI>().protectionLevel);
                print("piece " + bestPieceTwo.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[1][0] + ", " + bestAction[1][1] + " and has protection of: " + bestPieceTwo.GetComponent<BaseAI>().protectionLevel);
                print("piece " + bestPieceThree.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[2][0] + ", " + bestAction[2][1] + " and has protection of: " + bestPieceThree.GetComponent<BaseAI>().protectionLevel);

                // TO DO: 
                IsTurn(false);
                Manager.ChangeTurn(this.gameObject);

                //showBoard();
                //print("pressed SPACE");
            }

            yield return null;
        }
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
                        int[] actionOne = new int[2];
                        int[] actionTwo = new int[2];
                        int[] actionThree = new int[2];

                        //move each piece and store the action in the temp arrays
                        MovePieceCheck(pieceOne, x, actionOne);
                        MovePieceCheck(pieceTwo, y, actionTwo);
                        MovePieceCheck(pieceThree, z, actionThree);

                        //update protection of pieces
                        UpdateProtectionMap(pieceOne, x);
                        UpdateProtectionMap(pieceTwo, y);
                        UpdateProtectionMap(pieceThree, z);

                        int newScore = minimax(Board, 0, false);
                        
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
    }

    //method that moves a piece to a new position and stores the action
    private void MovePieceCheck(GameObject piece, int x, int[] action)
    {
        Manager.UpdateIntBoard(piece.GetComponent<IPieceBase>().CurrColPos, piece.GetComponent<IPieceBase>().CurrRowPos, piece.GetComponent<BaseAI>().validActions[x][3],
                               piece.GetComponent<BaseAI>().validActions[x][4], piece.GetComponent<IPieceBase>().PieceID);

        action[0] = piece.GetComponent<BaseAI>().validActions[x][3];
        action[1] = piece.GetComponent<BaseAI>().validActions[x][4];
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

        if (depth == 1)
            return score;

        // If there are no more moves and no winner then
        // it is a tie
        //if (isMovesLeft(board) == false)
        //    return 0;

        // If this maximizer's move
        if (isMax)
        {
            int best = -10000000;
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
            int best = 10000000;

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

                    //makeMove
                    Manager.UpdateIntBoard(moveSet[0][1], moveSet[0][2], moveSet[0][3],
                                  moveSet[0][4], moveSet[0][0]);

                    Manager.UpdateIntBoard(moveSet[1][1], moveSet[1][2], moveSet[1][3],
                                  moveSet[1][4], moveSet[1][0]);

                    Manager.UpdateIntBoard(moveSet[2][1], moveSet[2][2], moveSet[2][3],
                                  moveSet[2][4], moveSet[2][0]);

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
