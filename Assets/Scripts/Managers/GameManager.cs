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
    public Camera camera;

    public GameObject player;
    public GameObject ai;

    public GameObject board;

    public GameObject whiteCell, blackCell;

    public GameObject castle, knight, bishop, queen, king, pawn;

    private readonly GameObject[] higherOrder = new GameObject[8];

    private readonly GameObject[,] boardArr = new GameObject[8,8];

    private readonly List<GameObject> player1Pieces = new List<GameObject>();
    private readonly List<GameObject> player2Pieces = new List<GameObject>();

    private List<GameObject> highlightedCells = new List<GameObject>();

    private bool hasSelectedAPiece = false;
    private Cell previousCell;

    private GameMode currGameMode = GameMode.PvAI;

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
                    if (j == 0)
                    {
                        SetPosition(higherOrder[i], i, j);

                        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = higherOrder[i];

                        player1Pieces.Add(higherOrder[i]);
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

            RaycastHit hit;
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            Debug.Log(hit.transform.gameObject.name);

            if (cell)
            {
                if(cell.GetCurrentPiece)
                {
                    if(!hasSelectedAPiece)
                    {
                        SetSelectedPiece(hit, cell);
                    }
                    else
                    {
                        ClearSelected();
                        SetSelectedPiece(hit, cell);
                    }
                }
                else
                {
                    if(hasSelectedAPiece && highlightedCells.Contains(hit.transform.gameObject))
                    {
                        SetPositionFromCell(previousCell.GetCurrentPiece, hit.transform.gameObject);

                        cell.GetCurrentPiece = previousCell.GetCurrentPiece;
                        previousCell.GetCurrentPiece = null;

                        ClearSelected();
                    }
                }
            }
        }
    }

    private void SetSelectedPiece(RaycastHit hit, Cell cell)
    {
        previousCell = cell;
        var indexes = Tools.FindIndex(boardArr, hit.transform.gameObject);

        // highlight available slots
        highlightedCells = cell.GetCurrentPiece.GetComponent<BasePiece>().Highlight(boardArr, indexes[0], indexes[1]);
        Debug.Log("I have a piece");
    }

    private void ClearSelected()
    {
        previousCell = null;

        hasSelectedAPiece = false;

        highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
        highlightedCells.Clear();
    }

    private void PopulateCell(List<GameObject> playerList, GameObject piece, int i, int j)
    {
        var newPiece = CreateNewPiece(piece, i, j);

        playerList.Add(newPiece);

        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = piece;
    }

    private void SetPositionFromCell(GameObject piece, GameObject cell)
    {
        piece.transform.SetPositionAndRotation(cell.transform.position + new Vector3(0, 0.02f, 0), Quaternion.identity);
    }

    private void SetPosition(GameObject piece, int i, int j)
    {
        piece.transform.SetPositionAndRotation(boardArr[i, j].transform.position + new Vector3(0, 0.02f, 0), Quaternion.identity);
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

        newCell.transform.position = (startPos - cellRenderer.bounds.min) + (new Vector3(offsetX, 1.25f, offsetZ));
        return newCell;
    }
}
