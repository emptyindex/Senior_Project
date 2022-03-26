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

    public static bool AiTurn;

    public static int protectionBoard;
    public static int dangerBoard;

    // Start is called before the first frame update
    void Start()
    {
        AiTurn = true;

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
                
                print("Left Bishop Corp:");
                foreach (GameObject thing in BishopLPieces)
                {
                //    int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                //    int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                //    thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                //    thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrRowPos + ", " + thing.GetComponent<IPieceBase>().CurrColPos);
                }
                print("Right Bishop Corp:");
                foreach (GameObject thing in BishopRPieces)
                {
                    //int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                    //int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                    //thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                    //thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrRowPos + ", " + thing.GetComponent<IPieceBase>().CurrColPos);
                }
                print("King Corp:");
                foreach (GameObject thing in KingPieces)
                {
                    //int tempRow = thing.GetComponent<IPieceBase>().CurrRowPos;
                    //int tempCol = thing.GetComponent<IPieceBase>().CurrColPos;
                    //thing.GetComponent<IPieceBase>().CurrColPos = tempRow;
                    //thing.GetComponent<IPieceBase>().CurrRowPos = tempCol;
                    print(thing.GetComponent<IPieceBase>().PieceID + ", at pos: " + thing.GetComponent<IPieceBase>().CurrRowPos + ", " + thing.GetComponent<IPieceBase>().CurrColPos);
                }
                
                protectionBoard = 0;
                Array.ForEach(Pieces, p => p.GetComponent<BaseAI>().hasFinished = false);

                yield return new WaitForSeconds(1);

                //print(protectionBoard);

                bestScore = -99999;
                int[] moveToMake = new int[2];

                BaseAI pieceThatMoved = null;

                foreach (GameObject x in BishopLPieces)
                {
                    foreach (GameObject y in BishopRPieces)
                    {
                        foreach (GameObject z in KingPieces)
                        {
                            checkCombinations(x, y, z);
                        }
                    }
                }           
                print("yo debug");


                bestPieceOne.gameObject.transform.position = Manager.GetMovePosition(bestAction[0][1], bestAction[0][0]);
                bestPieceTwo.gameObject.transform.position = Manager.GetMovePosition(bestAction[1][1], bestAction[1][0]);
                bestPieceThree.gameObject.transform.position = Manager.GetMovePosition(bestAction[2][1], bestAction[2][0]);

                Manager.UpdateIntBoard(bestPieceOne.GetComponent<IPieceBase>().CurrRowPos, bestPieceOne.GetComponent<IPieceBase>().CurrColPos, bestAction[0][0], bestAction[0][1], bestPieceOne.GetComponent<IPieceBase>().PieceID);
                Manager.UpdateIntBoard(bestPieceTwo.GetComponent<IPieceBase>().CurrRowPos, bestPieceTwo.GetComponent<IPieceBase>().CurrColPos, bestAction[1][0], bestAction[1][1], bestPieceTwo.GetComponent<IPieceBase>().PieceID);
                Manager.UpdateIntBoard(bestPieceThree.GetComponent<IPieceBase>().CurrRowPos, bestPieceThree.GetComponent<IPieceBase>().CurrColPos, bestAction[2][0], bestAction[2][1], bestPieceThree.GetComponent<IPieceBase>().PieceID);

                bestPieceOne.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[0][0], bestAction[0][1], Board);
                bestPieceTwo.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[1][0], bestAction[1][1], Board);
                bestPieceThree.GetComponent<IProtectionBoard>().UpdateProtectionMap(bestAction[2][0], bestAction[2][1], Board);

                bestPieceOne.GetComponent<IPieceBase>().CurrRowPos = bestAction[0][0];
                bestPieceOne.GetComponent<IPieceBase>().CurrColPos = bestAction[0][1];

                bestPieceTwo.GetComponent<IPieceBase>().CurrRowPos = bestAction[1][0];
                bestPieceTwo.GetComponent<IPieceBase>().CurrColPos = bestAction[1][1];

                bestPieceThree.GetComponent<IPieceBase>().CurrRowPos = bestAction[2][0];
                bestPieceThree.GetComponent<IPieceBase>().CurrColPos = bestAction[2][1];

                print("piece " + bestPieceOne.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[0][0] + ", " + bestAction[0][1] + " and has protection of: " + bestPieceOne.GetComponent<BaseAI>().protectionLevel);
                print("piece " + bestPieceTwo.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[1][0] + ", " + bestAction[1][1] + " and has protection of: " + bestPieceTwo.GetComponent<BaseAI>().protectionLevel);
                print("piece " + bestPieceThree.GetComponent<IPieceBase>().PieceID + " has moved to: " + bestAction[2][0] + ", " + bestAction[2][1] + " and has protection of: " + bestPieceThree.GetComponent<BaseAI>().protectionLevel);
                print(bestScore);

                // TO DO: 
                IsTurn(false);
                Manager.ChangeTurn(this.gameObject);

                //showBoard();
                //print("pressed SPACE");
            }

            yield return null;
        }
    }

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
                        int[] actionOne = new int[2];
                        int[] actionTwo = new int[2];
                        int[] actionThree = new int[2];

                        MovePieceCheck(pieceOne, x, actionOne);
                        MovePieceCheck(pieceTwo, y, actionTwo);
                        MovePieceCheck(pieceThree, z, actionThree);

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

                        MovePieceBack(pieceOne, x);
                        MovePieceBack(pieceTwo, y);
                        MovePieceBack(pieceThree, z);
                    }
                }
            }
        }
    }

    private void MovePieceBack(GameObject pieceOne, int x)
    {
        Board[pieceOne.GetComponent<BaseAI>().validActions[x][3], pieceOne.GetComponent<BaseAI>().validActions[x][4]] = 0;
        Board[pieceOne.GetComponent<IPieceBase>().CurrRowPos, pieceOne.GetComponent<IPieceBase>().CurrColPos] = pieceOne.GetComponent<IPieceBase>().PieceID;
        pieceOne.GetComponent<IProtectionBoard>().UpdateProtectionMap(pieceOne.GetComponent<IPieceBase>().CurrRowPos, pieceOne.GetComponent<IPieceBase>().CurrColPos, Board);
    }

    private void UpdateProtectionMap(GameObject pieceOne, int x)
    {
        pieceOne.GetComponent<IProtectionBoard>().UpdateProtectionMap(pieceOne.GetComponent<BaseAI>().validActions[x][3], pieceOne.GetComponent<BaseAI>().validActions[x][4], Board);
    }

    private void MovePieceCheck(GameObject pieceTwo, int y, int[] actionTwo)
    {
        Manager.UpdateIntBoard(pieceTwo.GetComponent<IPieceBase>().CurrRowPos, pieceTwo.GetComponent<IPieceBase>().CurrColPos, pieceTwo.GetComponent<BaseAI>().validActions[y][3],
                               pieceTwo.GetComponent<BaseAI>().validActions[y][4], pieceTwo.GetComponent<IPieceBase>().PieceID);

        actionTwo[0] = pieceTwo.GetComponent<BaseAI>().validActions[y][3];
        actionTwo[1] = pieceTwo.GetComponent<BaseAI>().validActions[y][4];
    }

    //method to find the score of a given board
    //since attacks are not guarenteed the current attack is also passed if a piece is attacking another
    //right now just returns a random value
    public int HeuristicScore(int[,] theBoard, int protection)
    {
        int score = protection;
        return score;
    }

    public override void SetPieces(List<GameObject> pieces)
    {
        this.Pieces = pieces.ToArray();
    }
}
