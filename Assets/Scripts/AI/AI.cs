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

    public static int protectionBoard;
    public static int dangerBoard;

    // Start is called before the first frame update
    void Start()
    {
        AiTurn = true;

        AiMoveList = new List<int[][]>();

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

                if (bestAction[0][bestAction.Length - 1] == 0)
                {
                    MovePiece(bestPieceOne, 0);
                }
                else
                {
                    // attack
                }

                if (bestAction[1][bestAction.Length - 1] == 0)
                {
                    MovePiece(bestPieceTwo, 1);
                }
                else
                {
                    // attack
                }

                if (bestAction[2][bestAction.Length - 1] == 0)
                {
                    MovePiece(bestPieceThree, 2);
                }
                else
                {
                    // attack
                }

                // TO DO: 
                IsTurn(false);
                Manager.ChangeTurn(this.gameObject);

                //showBoard();
                //print("pressed SPACE");
            }

            yield return null;
        }
    }

    private void MovePiece(GameObject piece, int x)
    {
        var pieceBase = piece.GetComponent<IPieceBase>();

        piece.transform.position = Manager.GetMovePosition(bestAction[x][1], bestAction[x][0]);

        Manager.UpdateIntBoard(pieceBase.CurrColPos, pieceBase.CurrRowPos, bestAction[x][0], bestAction[x][1], pieceBase.PieceID);

        piece.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[x][0], bestAction[x][1], Board);

        pieceBase.CurrColPos = bestAction[x][0];
        pieceBase.CurrRowPos = bestAction[x][1];

        print("piece " + pieceBase.PieceID + " has moved to: " + bestAction[x][0] + ", " + bestAction[x][1] + " and has protection of: " + piece.GetComponent<BaseAI>().protectionLevel);
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
}
