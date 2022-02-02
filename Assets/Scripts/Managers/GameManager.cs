using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameMode
{
    PvP,
    PvAI,
    AIvAI    
}

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ai;

    public GameObject board;

    public GameObject whiteCell, blackCell;

    public GameObject castle, knight, bishop, queen, king, pawn;

    private GameObject[] players = new GameObject[2];

    private readonly GameObject[] higherOrder = new GameObject[8];

    private readonly GameObject[,] boardArr = new GameObject[8,8];

    private readonly List<GameObject> player1Pieces = new List<GameObject>();
    private readonly List<GameObject> player2Pieces = new List<GameObject>();

    private GameMode currGameMode = GameMode.PvP;

    // Start is called before the first frame update
    void Start()
    {
        Object.Instantiate(board);

        var renderer = board.GetComponent<Renderer>();
        var startPos = renderer.bounds.min;

        higherOrder[0] = GameObject.Instantiate(castle);
        higherOrder[1] = GameObject.Instantiate(knight);
        higherOrder[2] = GameObject.Instantiate(bishop);
        higherOrder[3] = GameObject.Instantiate(queen);
        higherOrder[4] = GameObject.Instantiate(king);
        higherOrder[5] = GameObject.Instantiate(bishop);
        higherOrder[6] = GameObject.Instantiate(knight);
        higherOrder[7] = GameObject.Instantiate(castle);

        for (int i = 0; i < boardArr.GetLength(0); i++)
        {
            for (int j = 0; j < boardArr.GetLength(1); j++)
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
                    if (j == 0)
                    {
                        SetPosition(player1Pieces, i, j, higherOrder[i]);
                    }
                    if (j == 1)
                    {
                        PopulateCell(player1Pieces, pawn, i, j);
                    }
                    if (j == boardArr.GetLength(0) - 2)
                    {
                        PopulateCell(player2Pieces, pawn, i, j);
                    }
                    if (j == boardArr.GetLength(0) - 1)
                    {
                        PopulateCell(player2Pieces, higherOrder[i], i, j);
                    }
                }
            }
        }

        switch (currGameMode)
        {
            case GameMode.PvP:
                players[0] = CreatePlayer(player1Pieces);
                players[1] = CreatePlayer(player2Pieces);

                break;
            case GameMode.PvAI:
                players[0] = Object.Instantiate(ai);
                players[1] = CreatePlayer(player2Pieces);

                break;
            case GameMode.AIvAI:
                break;
        }

        players[0].GetComponent<BasePlayer>().IsTurn(true);
    }

    public void ChangeTurn(GameObject player)
    {
        int index = players.FindIndex(player).FirstOrDefault();

        switch(index)
        {
            case 0:
                players[1].GetComponent<BasePlayer>().IsTurn(true);
                break;
            case 1:
                players[0].GetComponent<BasePlayer>().IsTurn(true);
                break;
        }
    }

    public List<GameObject> SetSelectedPiece(RaycastHit hit, Cell cell)
    {
        var indexes = Tools.FindIndex(boardArr, hit.transform.gameObject);

        // highlight available slots
        var highlightedCells = cell.GetCurrentPiece.GetComponent<BasePiece>().Highlight(boardArr, indexes[0], indexes[1]);

        Debug.Log("I have a piece");

        return highlightedCells;
    }

    private GameObject CreatePlayer(List<GameObject> pieces)
    {
        var p = Object.Instantiate(player);

        p.GetComponent<PlayerManager>().SetPieces(pieces.ToArray());
        p.GetComponent<PlayerManager>().Manager = this;

        return p;
    }

    private void PopulateCell(List<GameObject> playerList, GameObject piece, int i, int j)
    {
        var newPiece = Instantiate(piece);

        if (newPiece.GetComponent<Pawn>())
        {
            if(j > 2)
            {
                newPiece.GetComponent<Pawn>().MoveUp = false;
            }
        }

        SetPosition(playerList, i, j, newPiece);
    }

    private void SetPosition(List<GameObject> playerList, int i, int j, GameObject newPiece)
    {
        playerList.Add(newPiece);

        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = newPiece;

        newPiece.transform.parent = boardArr[i, j].transform;
        newPiece.transform.localPosition = new Vector3(0, 0.02f, 0);
    }

    private GameObject CreateNewCell(GameObject cell, Vector3 startPos, int i, int j)
    {
        var newCell = Instantiate(cell);
        var cellRenderer = newCell.GetComponent<Renderer>();

        var offsetX = cellRenderer.bounds.size.x * i;
        var offsetZ = cellRenderer.bounds.size.z * j;

        newCell.transform.position = (startPos - cellRenderer.bounds.min) + (new Vector3(offsetX, 1.25f, offsetZ));
        return newCell;
    }
}
