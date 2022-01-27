using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject board;

    public GameObject whiteCell;
    public GameObject blackCell;

    public GameObject castle, knight, rook, queen, king, pawn;
    public readonly GameObject[] higherOrder = new GameObject[8];

    private readonly GameObject[,] boardArr = new GameObject[8,8];
    
    private readonly ArrayList player1Pieces = new ArrayList();
    private readonly ArrayList player2Pieces = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        Object.Instantiate(board);

        var renderer = board.GetComponent<Renderer>();
        var startPos = renderer.bounds.min;

        higherOrder[0] = GameObject.Instantiate(castle);
        higherOrder[1] = GameObject.Instantiate(knight);
        higherOrder[2] = GameObject.Instantiate(rook);
        higherOrder[3] = GameObject.Instantiate(queen);
        higherOrder[4] = GameObject.Instantiate(king);
        higherOrder[5] = GameObject.Instantiate(rook);
        higherOrder[6] = GameObject.Instantiate(knight);
        higherOrder[7] = GameObject.Instantiate(castle);

        for (int i = 0; i < boardArr.GetLength(0); i++)
        {
            for(int j = 0; j < boardArr.GetLength(1); j++)
            {
                if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                {
                    boardArr[i, j] = CreateNewCell(whiteCell, startPos, i, j);
                }
                else
                {
                    boardArr[i, j] = CreateNewCell(blackCell, startPos, i, j);
                }

                if (j < 2 || j > boardArr.GetLength(0) - 3)
                {
                    if(j == 0)
                    {
                        SetPosition(higherOrder[i], i, j);
                        player1Pieces.Add(higherOrder[i]);
                    }
                    if(j == 1)
                    {
                        player1Pieces.Add(CreateNewPiece(pawn, i, j, false));
                    }
                    if (j == boardArr.GetLength(0) - 2)
                    {
                        player2Pieces.Add(CreateNewPiece(pawn, i, j, true));
                    }
                    if(j == boardArr.GetLength(0) - 1)
                    {
                        player2Pieces.Add(CreateNewPiece(higherOrder[i], i, j, true));
                    }
                }
            }
        }
    }

    private void SetPosition(GameObject piece, int i, int j)
    {
        piece.transform.SetPositionAndRotation(boardArr[i, j].transform.position + new Vector3(0, 0.01f, 0), Quaternion.identity);

        piece.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    private GameObject CreateNewPiece(GameObject piece, int i, int j, bool isBlack)
    {
        var newPawn = Object.Instantiate(piece);
        SetPosition(newPawn, i, j);

        if (isBlack)
        {
            newPawn.GetComponent<Renderer>().material.color *= 0.5f;
        }

        return newPawn;
    }

    private GameObject CreateNewCell(GameObject cell, Vector3 startPos, int i, int j)
    {
        var newCell = Object.Instantiate(cell);
        var cellRenderer = newCell.GetComponent<Renderer>();

        var offsetX = cellRenderer.bounds.size.x * i;
        var offsetZ = cellRenderer.bounds.size.z * j;

        newCell.transform.position = (startPos - cellRenderer.bounds.min) + (new Vector3(offsetX, 1.2f, offsetZ));
        return newCell;
    }
}
