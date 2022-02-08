using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AI : BasePlayer
{
    //public GameObject rook, knight, bishop, queen, king, pawn;

    [HideInInspector]
    public int[,] Board;
    [HideInInspector]
    public GameObject[,] BoardState;
    //[HideInInspector]
    //public int[,] PieceInfo;
    public GameObject[] Pieces;

    public static bool AiTurn;

    // Start is called before the first frame update
    void Start()
    {
        //rows = 8;
        //columns = 8;
        //Board = new int[rows, columns];
        //BoardState = new GameObject[rows, columns];

        AiTurn = true;

        //PieceInfo = new int[16, 6];
        //Pieces = new GameObject[16];

        //initializeBoard();
        //showBoard();

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
    void initializeBoard()
    {
        //rooks
        //Board[0, 0] = 12; Board[0, 7] = 12; Board[7, 0] = 2; Board[7, 7] = 2;
        //GameObject rookClone = Instantiate(rook);
        //Pieces[8] = rookClone;
        //rookClone.transform.position = new Vector3(35 - 10 * 0, .75f, -35 + 10 * 0);
        //rookClone.GetComponent<RookAI>().pieceNum = 8;
        //BoardState[0, 0] = rookClone;
        //BoardState[0, 0].GetComponent<RookAI>().currRow = 0;
        //BoardState[0, 0].GetComponent<RookAI>().currCol = 0;

        //GameObject rookClone2 = Instantiate(rook);
        //Pieces[9] = rookClone2;
        //rookClone2.transform.position = new Vector3(35 - 10 * 7, .75f, -35 + 10 * 0);
        //rookClone2.GetComponent<RookAI>().pieceNum = 9;
        //BoardState[0, 7] = rookClone2;
        //BoardState[0, 7].GetComponent<RookAI>().currRow = 0;
        //BoardState[0, 7].GetComponent<RookAI>().currCol = 7;

        ////knights
        //Board[0, 1] = 13; Board[0, 6] = 13; Board[7, 1] = 3; Board[7, 6] = 3;
        //GameObject knightClone = Instantiate(knight);
        //Pieces[10] = knightClone;
        //knightClone.transform.position = new Vector3(35 - 10 * 1, .75f, -35 + 10 * 0);
        //knightClone.GetComponent<KnightAI>().pieceNum = 10;
        //BoardState[0, 1] = knightClone;
        //BoardState[0, 1].GetComponent<KnightAI>().currRow = 0;
        //BoardState[0, 1].GetComponent<KnightAI>().currCol = 1;

        //GameObject knightClone2 = Instantiate(knight);
        //Pieces[11] = knightClone2;
        //knightClone2.transform.position = new Vector3(35 - 10 * 6, .75f, -35 + 10 * 0);
        //knightClone2.GetComponent<KnightAI>().pieceNum = 11;
        //BoardState[0, 6] = knightClone2;
        //BoardState[0, 6].GetComponent<KnightAI>().currRow = 0;
        //BoardState[0, 6].GetComponent<KnightAI>().currCol = 6;

        ////bishops
        //Board[0, 2] = 14; Board[0, 5] = 14; Board[7, 2] = 4; Board[7, 5] = 4;
        //GameObject bishopClone = Instantiate(bishop);
        //Pieces[12] = bishopClone;
        //bishopClone.transform.position = new Vector3(35 - 10 * 2, .75f, -35 + 10 * 0);
        //bishopClone.GetComponent<BishopAI>().pieceNum = 12;
        //BoardState[0, 2] = bishopClone;
        //BoardState[0, 2].GetComponent<BishopAI>().currRow = 0;
        //BoardState[0, 2].GetComponent<BishopAI>().currCol = 1;

        //GameObject bishopClone2 = Instantiate(bishop);
        //Pieces[13] = bishopClone2;
        //bishopClone2.transform.position = new Vector3(35 - 10 * 5, .75f, -35 + 10 * 0);
        //bishopClone2.GetComponent<BishopAI>().pieceNum = 13;
        //BoardState[0, 5] = bishopClone2;
        //BoardState[0, 5].GetComponent<BishopAI>().currRow = 0;
        //BoardState[0, 5].GetComponent<BishopAI>().currCol = 6;

        ////queens
        //Board[0, 3] = 15; Board[7, 3] = 5;
        //GameObject queenClone = Instantiate(queen);
        //Pieces[14] = queenClone;
        //queenClone.transform.position = new Vector3(35 - 10 * 3, .75f, -35 + 10 * 0);
        //queenClone.GetComponent<QueenAI>().pieceNum = 14;
        //BoardState[0, 3] = queenClone;
        //BoardState[0, 3].GetComponent<QueenAI>().currRow = 0;
        //BoardState[0, 3].GetComponent<QueenAI>().currCol = 3;

        ////kings
        //Board[0, 4] = 16; Board[7, 4] = 6;
        //GameObject kingClone = Instantiate(king);
        //Pieces[15] = kingClone;
        //kingClone.transform.position = new Vector3(35 - 10 * 4, .75f, -35 + 10 * 0);
        //kingClone.GetComponent<KingAI>().pieceNum = 15;
        //BoardState[0, 3] = kingClone;
        //BoardState[0, 3].GetComponent<KingAI>().currRow = 0;
        //BoardState[0, 3].GetComponent<KingAI>().currCol = 4;

        ////pawns
        //for (int i = 0; i < 8; i++)
        //{
        //    Board[1, i] = 11;
        //    GameObject pawnClone = Instantiate(pawn);
        //    Pieces[i] = pawnClone;
        //    pawnClone.transform.position = new Vector3(35 - 10 * i, .75f, -25);
        //    pawnClone.GetComponent<PawnAI>().pieceNum = i;
        //    BoardState[1, i] = pawnClone;
        //    BoardState[1, i].GetComponent<PawnAI>().currRow = 1;
        //    BoardState[1, i].GetComponent<PawnAI>().currCol = i;

        //    Board[6, i] = 1;
        //}
        //Board[0, 2] = 1;
    }

    //helper method to print the current state of the board to the debug line
    //void showBoard()
    //{
    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < columns; j++)
    //        {
    //            print(Board[i, j]);
    //        }
    //    }

    //    //print(BoardState[1, 0].GetComponent<PawnAI>().currCol);
    //    //print(BoardState[1, 1].GetComponent<PawnAI>().currCol);
    //}

    //coroutine
    IEnumerator AiPick()
    {
        while (!isGameOver)
        {
            if (this.canMove)
            {
                Array.ForEach(Pieces, p => p.GetComponent<BaseAI>().hasFinished = false);

                //for (int i = 0; i < 8; i++)
                //{
                //    Pieces[i].GetComponent<PawnAI>().hasFinished = false;
                //}
                //Pieces[8].GetComponent<RookAI>().hasFinished = false;
                //Pieces[9].GetComponent<RookAI>().hasFinished = false;
                //Pieces[10].GetComponent<KnightAI>().hasFinished = false;
                //Pieces[11].GetComponent<KnightAI>().hasFinished = false;
                //Pieces[12].GetComponent<BishopAI>().hasFinished = false;
                //Pieces[13].GetComponent<BishopAI>().hasFinished = false;
                //Pieces[14].GetComponent<QueenAI>().hasFinished = false;
                //Pieces[15].GetComponent<KingAI>().hasFinished = false;

                yield return new WaitForSeconds(1);

                int bestScore = -99999;
                int[] moveToMake = new int[2];

                BaseAI pieceThatMoved = null;

                foreach(GameObject piece in Pieces)
                {
                    if(piece.GetComponent<BaseAI>().bestScore > bestScore)
                    {
                        pieceThatMoved = piece.GetComponent<BaseAI>();
                        bestScore = pieceThatMoved.bestScore;
                        moveToMake[0] = pieceThatMoved.bestMove[0];
                        moveToMake[1] = pieceThatMoved.bestMove[1];
                    }
                }

                if(pieceThatMoved == null)
                {
                    throw new Exception("No piece was found to move.");
                }

                //for (int i = 0; i < 16; i++)
                //{
                //    if (PieceInfo[i, 0] > bestScore)
                //    {
                //        bestScore = PieceInfo[i, 0];
                //        moveToMake[0] = PieceInfo[i, 1];
                //        moveToMake[1] = PieceInfo[i, 2];
                //        pieceThatMoved = i;
                //    }
                //}             

                print("piece " + pieceThatMoved.GetComponent<IPieceBase>().PieceID + " has moved to: " + moveToMake[0] + ", " + moveToMake[1]);
                //print(bestScore);
                //print(moveToMake[0]);
                //print(moveToMake[1]);

                //Pieces[pieceThatMoved].transform.position = new Vector3(35 - 10 * moveToMake[1], .75f, -35 + 10 * moveToMake[0]);

                pieceThatMoved.gameObject.transform.position = Manager.GetMovePosition(moveToMake[1], moveToMake[0]);
                Manager.UpdateIntBoard(pieceThatMoved.currRow, pieceThatMoved.currCol, moveToMake[0], moveToMake[1], pieceThatMoved.GetComponent<IPieceBase>().PieceID);

                //Manager.UpdateIntBoard(PieceInfo[pieceThatMoved, 3], PieceInfo[pieceThatMoved, 4], moveToMake[0], moveToMake[1], PieceInfo[pieceThatMoved, 5]);

                //Board[PieceInfo[pieceThatMoved, 3], PieceInfo[pieceThatMoved, 4]] = 0;
                //Board[moveToMake[0], moveToMake[1]] = PieceInfo[pieceThatMoved, 5];

                pieceThatMoved.currRow = moveToMake[0];
                pieceThatMoved.currCol = moveToMake[1];

                //Pieces[index].GetComponent<BaseAI>().currRow = moveToMake[0];


                //if (PieceInfo[pieceThatMoved, 5] == 11)
                //{
                //    Pieces[pieceThatMoved].GetComponent<PawnAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<PawnAI>().currCol = moveToMake[1];
                //}
                //if (PieceInfo[pieceThatMoved, 5] == 12)
                //{
                //    Pieces[pieceThatMoved].GetComponent<RookAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<RookAI>().currCol = moveToMake[1];
                //}
                //if (PieceInfo[pieceThatMoved, 5] == 13)
                //{
                //    Pieces[pieceThatMoved].GetComponent<KnightAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<KnightAI>().currCol = moveToMake[1];
                //}
                //if (PieceInfo[pieceThatMoved, 5] == 14)
                //{
                //    Pieces[pieceThatMoved].GetComponent<BishopAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<BishopAI>().currCol = moveToMake[1];
                //}
                //if (PieceInfo[pieceThatMoved, 5] == 15)
                //{
                //    Pieces[pieceThatMoved].GetComponent<QueenAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<QueenAI>().currCol = moveToMake[1];
                //}
                //if (PieceInfo[pieceThatMoved, 5] == 16)
                //{
                //    Pieces[pieceThatMoved].GetComponent<KingAI>().currRow = moveToMake[0];
                //    Pieces[pieceThatMoved].GetComponent<KingAI>().currCol = moveToMake[1];
                //}

                IsTurn(false);
                Manager.ChangeTurn(this.gameObject);

                //showBoard();
                //print("pressed SPACE");
            }

            yield return null;
        }
    }

    public override void SetPieces(List<GameObject> pieces)
    {
        this.Pieces = pieces.ToArray();
    }
}
