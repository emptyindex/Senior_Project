using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    PvP,
    PvAI,
    AIvAI    
}

public class GameManager : MonoBehaviour
{
    public Camera camera;

    public GameObject player;
    public GameObject ai;

    public GameObject board;

    public GameObject whiteCell;
    public GameObject blackCell;

    public GameObject castle, knight, rook, queen, king, pawn;

    private readonly GameObject[] higherOrder = new GameObject[8];

    private readonly GameObject[,] boardArr = new GameObject[8,8];

    private readonly List<GameObject> player1Pieces = new List<GameObject>();
    private readonly List<GameObject> player2Pieces = new List<GameObject>();

    private GameMode currGameMode = GameMode.PvAI;

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

                        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = higherOrder[i];

                        player1Pieces.Add(higherOrder[i]);
                    }
                    if(j == 1)
                    {
                        PopulateCell(player1Pieces, pawn, i, j);
                    }
                    if (j == boardArr.GetLength(0) - 2)
                    {
                        PopulateCell(player2Pieces, pawn, i, j);
                    }
                    if(j == boardArr.GetLength(0) - 1)
                    {
                        PopulateCell(player2Pieces, higherOrder[i], i, j);
                    }
                }
            }
        }

        switch(currGameMode)
        {
            case GameMode.PvP:
                break;
            case GameMode.PvAI:
                Object.Instantiate(player);
                Object.Instantiate(ai);

                player.GetComponent<PlayerManager>().SetPieces(player1Pieces.ToArray());
                break;
            case GameMode.AIvAI:
                break;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Debug.Log("position: " + camera.ScreenPointToRay(Input.mousePosition));
        }

    }

    private void PopulateCell(List<GameObject> playerList, GameObject piece, int i, int j)
    {
        var newPiece = CreateNewPiece(piece, i, j);

        playerList.Add(newPiece);

        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = newPiece;
    }

    private void SetPosition(GameObject piece, int i, int j)
    {
        piece.transform.SetPositionAndRotation(boardArr[i, j].transform.position + new Vector3(0, 0.01f, 0), Quaternion.identity);
        piece.transform.localScale *= 0.75f;
    }

    private GameObject CreateNewPiece(GameObject piece, int i, int j)
    {
        var newPawn = Object.Instantiate(piece);
        SetPosition(newPawn, i, j);

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
