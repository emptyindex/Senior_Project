using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Public Enum to represent the 3 different types of game modes.
/// </summary>
public enum GameMode
{
    PvP,
    PvAI,
    AIvAI    
}

/// <summary>
/// The Game manager simply manages non-player activities. 
/// It creates the board and manages the turns.
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ai;

    public GameObject board;

    public GameObject whiteCell, blackCell;

    public GameObject castle, knight, bishop, queen, king, pawn;

    private GameObject[] players = new GameObject[2];

    private readonly GameObject[] higherOrder = new GameObject[8];

    public static GameObject[,] boardArr = new GameObject[8,8];

    private readonly List<GameObject> player1Pieces = new List<GameObject>();
    private readonly List<GameObject> player2Pieces = new List<GameObject>();

    private GameMode currGameMode = GameMode.PvP;

    /// <summary>
    /// Start is called before the first frame update when the script is loaded in.
    /// Instantiates the chess board and populates the squares with pieces.
    /// It then instantiates the players depending on the game mode -
    /// Two human players for PvP, One human and One AI player for PvAI, 
    /// and Two AI players for AIvAI.
    /// After creating the players, it assigns the appropriate collection of pieces to each.
    /// </summary>
    void Start()
    {
        Object.Instantiate(board);

        var renderer = board.GetComponent<Renderer>();
        var startPos = renderer.bounds.min;

        // Populates the higherOrder array with the correct order to place non-pawn pieces.
        // The array is populated by Instantiating the Prefabs in the scene.
        higherOrder[0] = GameObject.Instantiate(castle);
        higherOrder[1] = GameObject.Instantiate(knight);
        higherOrder[2] = GameObject.Instantiate(bishop);
        higherOrder[3] = GameObject.Instantiate(queen);
        higherOrder[4] = GameObject.Instantiate(king);
        higherOrder[5] = GameObject.Instantiate(bishop);
        higherOrder[6] = GameObject.Instantiate(knight);
        higherOrder[7] = GameObject.Instantiate(castle);

        // Nested for loop to create the chess board
        for (int i = 0; i < boardArr.GetLength(0); i++)
        {
            for (int j = 0; j < boardArr.GetLength(1); j++)
            {
                // If the width index and the height index are either both even or both odd, then the cell/square is white,
                // otherwise, the cell/square is black.
                if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                {
                    boardArr[i, j] = CreateNewCell(whiteCell, startPos, i, j);
                }
                else
                {
                    boardArr[i, j] = CreateNewCell(blackCell, startPos, i, j);
                }

                // If the position on the board is in the first or last two rows, then a piece needs to be placed.
                if (j < 2 || j > boardArr.GetLength(0) - 3)
                {
                    // if it's the first row of the board, we need to place a higher order (non-pawn) piece.
                    if (j == 0)
                    {
                        SetPosition(player1Pieces, i, j, higherOrder[i]);
                    }
                    // if it's in the second row of the board, we need to place a row of pawns.
                    if (j == 1)
                    {
                        PopulateCell(player1Pieces, pawn, i, j, false);
                    }
                    // if it's the second to last row on the board, we need to place the higher order piecs for the second player.
                    if (j == boardArr.GetLength(0) - 2)
                    {
                        PopulateCell(player2Pieces, pawn, i, j, true);
                    }
                    // if it's in the second row of the board, we need to place a row of pawns for the second player.
                    if (j == boardArr.GetLength(0) - 1)
                    {
                        PopulateCell(player2Pieces, higherOrder[i], i, j, true);
                    }
                }
            }
        }

        // depending on the chosen game mode, instantiate the correct players and assign the 
        // set of pieces to them.
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

        // Tell player one that it's their turn. 
        players[0].GetComponent<BasePlayer>().IsTurn(true);
    }

    /// <summary>
    /// Gets the position of a cell in the board array and gives
    /// the coordinates to the game piece.
    /// </summary>
    /// <param name="moveCell">The cell whose position needs to be found in the array.</param>
    /// <param name="piece">The piece that needs to be moved to the cell postion.</param>
    /// <returns>Returns the coordinates of the cell in the board.</returns>
    public Vector3 GetMovePosition(GameObject moveCell, GameObject piece)
    {
        var newIndexes = Tools.FindIndex(boardArr, moveCell);

        piece.GetComponent<BasePiece>().positionX = newIndexes[0];
        piece.GetComponent<BasePiece>().positionY = newIndexes[1];

        return boardArr[newIndexes[0], newIndexes[1]].transform.position;
    }

    /// <summary>
    /// When a player finishes a move, tell the other player that it's their turn
    /// using the abstract IsTurn method in BasePlayer.
    /// BasePlayer is the class that all players inherit from.
    /// </summary>
    /// <param name="player">The player that just finished their turn.</param>
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

    /// <summary>
    /// Helper method for highlighting the available moves when a player has selected a piece to move.
    /// This method is exlusively used for Human players.
    /// It exists in this class because it needs to access the board array.
    /// </summary>
    /// <param name="hit">The RaycastHit that represents the location the player has clicked.</param>
    /// <param name="cell">The Cell object to get the current piece that needs to be moved.</param>
    /// <returns>Returns all the highlighted cells where a piece can move.</returns>
    public List<GameObject> SetSelectedPiece(RaycastHit hit, Cell cell)
    {
        var indexes = Tools.FindIndex(boardArr, hit.transform.gameObject);

        // highlight available slots
        var highlightedCells = cell.GetCurrentPiece.GetComponent<BasePiece>().Highlight(boardArr, indexes[0], indexes[1]);

        Debug.Log("I have a piece");

        return highlightedCells;
    }

    /// <summary>
    /// Helper method that's called in Start() when a Human player is created.
    /// The player is given their list of pieces and also an instance of GameManager to call
    /// when they've completed a turn or to access the SetSelectedPiece method.
    /// </summary>
    /// <param name="pieces">The list of game pieces.</param>
    /// <returns>Returns a completed Human player.</returns>
    private GameObject CreatePlayer(List<GameObject> pieces)
    {
        var p = Object.Instantiate(player);

        p.GetComponent<PlayerManager>().SetPieces(pieces.ToArray());
        p.GetComponent<PlayerManager>().Manager = this;

        return p;
    }

    /// <summary>
    /// Helper method called in Start() to place a piece on a board square/cell.
    /// It instantiates the piece 
    /// </summary>
    /// <param name="playerList"></param>
    /// <param name="piece"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="isBlack"></param>
    private void PopulateCell(List<GameObject> playerList, GameObject piece, int i, int j, bool isBlack)
    {
        var newPiece = Instantiate(piece);

        if (newPiece.GetComponent<Pawn>())
        {
            if(j > 2)
            {
                newPiece.GetComponent<Pawn>().MoveUp = false;
            }
        }

        if(isBlack)
        {
            newPiece.GetComponent<Renderer>().material.color *= 0.5f;
        }

        newPiece.GetComponent<BasePiece>().positionX = i;
        newPiece.GetComponent<BasePiece>().positionY = j;

        SetPosition(playerList, i, j, newPiece);
    }

    /// <summary>
    /// Sets the position of a piece based on the position in the board array at
    /// the provided indexes. Used in init.
    /// </summary>
    /// <param name="playerList">The player list for whom the piece belongs.</param>
    /// <param name="i">The x index in the 2D array</param>
    /// <param name="j">The y index in the 2D array</param>
    /// <param name="newPiece">The piece that is being positioned.</param>
    private void SetPosition(List<GameObject> playerList, int i, int j, GameObject newPiece)
    {
        playerList.Add(newPiece);

        boardArr[i, j].GetComponent<Cell>().GetCurrentPiece = newPiece;

        newPiece.transform.position = boardArr[i, j].transform.position + new Vector3(0, 0.02f, 0);
    }

    /// <summary>
    /// Creates a new cell object based on the cell prefab. Used in init.
    /// </summary>
    /// <param name="cell">The Gameobject prefab to instantiate.</param>
    /// <param name="startPos">The starting position of the board.</param>
    /// <param name="i">The x index in the 2D array.</param>
    /// <param name="j">The y index in the 2D array.</param>
    /// <returns>Returns the completed cell object in the correct position on the board.</returns>
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
