using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

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
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI checkNotifyText;
    public GameObject player;
    public GameObject ai;

    public Dice dice;

    public GameObject boardPrefab;

    public GameObject whiteCell, blackCell;

    public GameObject[] humanPieces;
    public GameObject[] aiPieces;

    private DeadPile deadPile;
    private GameObject[] players = new GameObject[2];

    private int indexer = 0;

    public static GameObject[,] boardArr = new GameObject[8,8];

    public int[,] board = new int[8, 8];

    private readonly List<GameObject> player1Pieces = new List<GameObject>();
    private readonly List<GameObject> player2Pieces = new List<GameObject>();

    [HideInInspector]
    public GameMode currGameMode;

    public BasePlayer[] GetBasePlayers()
    {
        return new BasePlayer[2] { players[0].GetComponent<BasePlayer>(), players[1].GetComponent<BasePlayer>() };
    }

    private void Awake()
    {
        endGamePanel.SetActive(false);
    }

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
        checkNotifyText.text = string.Empty;
        //Instantiate(boardPrefab);

        deadPile = GameObject.FindWithTag("Deadpile").GetComponent<DeadPile>();

        var renderer = boardPrefab.GetComponent<Renderer>();
        var startPos = renderer.bounds.min + new Vector3(0.04f, 0, 0.04f);

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
                        PopulateCellForGameMode(player1Pieces, i, j, false, true);
                    }
                    // if it's in the second row of the board, we need to place a row of pawns.
                    if (j == 1)
                    {
                        PopulateCellForGameMode(player1Pieces, i, j, false);
                    }
                    // if it's the second to last row on the board, we need to place the higher order piecs for the second player.
                    if (j == boardArr.GetLength(0) - 2)
                    {
                        PopulateCellForGameMode(player2Pieces, i, j, true);
                    }
                    // if it's in the second row of the board, we need to place a row of pawns for the second player.
                    if (j == boardArr.GetLength(0) - 1)
                    {
                        PopulateCellForGameMode(player2Pieces, i, j, true, true);
                    }
                }

                indexer++;
            }
        }

        player2Pieces.ForEach(p => p.GetComponent<IPieceBase>().PieceID += 20);

        UpdateIntBoard(player1Pieces);
        UpdateIntBoard(player2Pieces);

        // depending on the chosen game mode, instantiate the correct players and assign the 
        // set of pieces to them.
        switch (currGameMode)
        {
            case GameMode.PvP:
                players[0] = CreatePlayer(player, player1Pieces);
                players[1] = CreatePlayer(player, player2Pieces);

                break;
            case GameMode.PvAI:
                players[0] = CreatePlayer(player, player1Pieces);
                players[1] = CreatePlayer(ai, player2Pieces);

                break;
            case GameMode.AIvAI:
                players[0] = CreatePlayer(ai, player1Pieces);
                players[1] = CreatePlayer(ai, player2Pieces);

                break;
        }

        // Tell player one that it's their turn. 
        players[0].GetComponent<BasePlayer>().IsTurn(true);
    }

    private void PopulateCellForGameMode(List<GameObject> pieces, int i, int j, bool isBlack, bool isHigherOrder = false)
    {
        bool isPlayer = true;

        if(currGameMode == GameMode.PvAI && j > 1)
        {
            isPlayer = false;
        }

        int index = 0;

        if(isHigherOrder)
        {
            index = i + 1;
        }

        switch (currGameMode)
        {
            case GameMode.PvP:
                PopulateHumanCell(pieces, Instantiate(humanPieces[index]), i, j, isBlack);
                break;
            case GameMode.PvAI:
                if(isPlayer)
                {
                    PopulateHumanCell(pieces, Instantiate(humanPieces[index]), i, j, isBlack);
                }
                else
                {
                    PopulateCell(pieces, i, j, isBlack, Instantiate(aiPieces[index]));
                }
                break;
            case GameMode.AIvAI:
                PopulateCell(pieces, i, j, isBlack, Instantiate(aiPieces[index]));
                break;
        }
    }

    private void NotifiyCheck(int playerIndex)
    {
        checkNotifyText.text = $"Player {playerIndex + 1}'s King is Checked.";
    }

    public void NotifiyCheck(GameObject safePlayer)
    {
        var playerIndex = players.FindIndex(safePlayer).First() == 0 ? 1 : 0;

        checkNotifyText.text = $"Player {playerIndex + 1}'s King is Checked.";
    }

    public void EndGame(GameObject winner)
    {
        foreach(var player in players)
        {
            player.GetComponent<BasePlayer>().isGameOver = true;
        }

        var playerIndex = players.FindIndex(winner).First();

        Debug.Log($"Player {playerIndex + 1} won. END GAME.");

        endGameText.text = $"Player {playerIndex + 1} won!";
        endGamePanel.SetActive(true);

        //Time.timeScale = 0f;
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

        piece.GetComponent<BasePiece>().CurrRowPos = newIndexes[0];
        piece.GetComponent<BasePiece>().CurrColPos = newIndexes[1];

        return boardArr[newIndexes[0], newIndexes[1]].transform.position;
    }

    public Vector3 GetMovePosition(int i, int j)
    {
        return boardArr[i, j].transform.position;
    }

    private void UpdateIntBoard(List<GameObject> pieces)
    {
        pieces.ForEach(p => { board[p.GetComponent<IPieceBase>().CurrColPos, p.GetComponent<IPieceBase>().CurrRowPos] = p.GetComponent<IPieceBase>().PieceID; });
    }

    public void UpdateIntBoard(int i, int j, int newi, int newj, int pieceID)
    {
        board[i, j] = 0;
        board[newi, newj] = pieceID;
    }

    /// <summary>
    /// When a player finishes a move, tell the other player that it's their turn
    /// using the abstract IsTurn method in BasePlayer.
    /// BasePlayer is the class that all players inherit from.
    /// </summary>
    /// <param name="player">The player that just finished their turn.</param>
    public void ChangeTurn(GameObject player)
    {
        checkNotifyText.text = string.Empty;
        int index = players.FindIndex(player).FirstOrDefault();

        switch(index)
        {
            case 0:
                players[1].GetComponent<BasePlayer>().IsTurn(true);
                Debug.Log("Player 2's turn.");
                break;
            case 1:
                players[0].GetComponent<BasePlayer>().IsTurn(true);
                Debug.Log("Player 1's turn.");
                break;
        }
    }

    public void RemoveKilledPieceFromPlayer(GameObject player, GameObject piece)
    {
        piece.layer = LayerMask.NameToLayer("Ignore Raycast");

        deadPile.deadPieces.Add(piece.GetComponent<IPieceBase>());
        piece.transform.SetParent(deadPile.transform);
        piece.transform.position = deadPile.transform.position + new Vector3(0, 0.5f, 0);

        int index = players.FindIndex(player).FirstOrDefault();

        switch (index)
        {
            case 0:
                players[1].GetComponent<BasePlayer>().RemovePiece(piece);
                break;
            case 1:
                players[0].GetComponent<BasePlayer>().RemovePiece(piece);
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
    public (List<GameObject>, List<GameObject>) SetSelectedPiece(RaycastHit hit, Cell cell, GameObject pieceToMove)
    {
        var indexes = Tools.FindIndex(boardArr, hit.transform.gameObject);

        var highlightedCells = pieceToMove.GetComponent<BasePiece>().Highlight(boardArr, indexes[0], indexes[1]);

        var containsKing = highlightedCells.attacks.Where(c => c.GetComponent<Cell>().GetCurrentPiece.GetComponent<KingAI>() || c.GetComponent<Cell>().GetCurrentPiece.GetComponent<King>());

        if(containsKing.Any())
        {
            NotifiyCheck(containsKing.First().GetComponent<Cell>().GetCurrentPiece.GetComponent<IPieceBase>().PieceID == 6 ? 0 : 1);
        }
        else
        {
            checkNotifyText.text = string.Empty;
        }

        // highlight available slots
        return highlightedCells;
    }

    /// <summary>
    /// Helper method that's called in Start() when a Human player is created.
    /// The player is given their list of pieces and also an instance of GameManager to call
    /// when they've completed a turn or to access the SetSelectedPiece method.
    /// </summary>
    /// <param name="pieces">The list of game pieces.</param>
    /// <returns>Returns a completed Human player.</returns>
    private GameObject CreatePlayer(GameObject playerToCreate, List<GameObject> pieces)
    {
        var p = Instantiate(playerToCreate);

        if (p.GetComponent<AI>())
        {
            var aiClass = p.GetComponent<AI>();

            aiClass.Board = this.board;

            pieces.ForEach(p => p.GetComponent<BaseAI>().AIManager = aiClass);
        }

        p.GetComponent<BasePlayer>().SetPieces(pieces);
        p.GetComponent<BasePlayer>().Manager = this;

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
    private void PopulateHumanCell(List<GameObject> playerList, GameObject newPiece, int i, int j, bool isBlack)
    {
        if (newPiece.GetComponent<Pawn>())
        {
            if (j > 2)
            {
                newPiece.GetComponent<Pawn>().MoveUp = false;
            }
        }

        if(newPiece.GetComponent<IRoyalty>() != null)
        {
            newPiece.GetComponent<IRoyalty>().ResetPos(new int[2] { i, j });
        }

        //var temp = newPiece.GetComponent<IPieceBase>().PieceID;

        PopulateCell(playerList, i, j, isBlack, newPiece);
    }

    private void PopulateCell(List<GameObject> playerList, int i, int j, bool isBlack, GameObject newPiece)
    {
        newPiece.GetComponent<PieceColorManager>().SetMaterialAndRotation(isBlack);

        if (newPiece.GetComponent<BaseAI>())
        {
            if ((j == 6 && i == 7) || (j == 6 && i == 6) || (j == 6 && i == 5) || (j == 7 && i == 6) || (j == 7 && i == 5))
            {
                AI.BishopLPieces.Add(newPiece);
            }
            if ((j == 6 && i == 0) || (j == 6 && i == 1) || (j == 6 && i == 2) || (j == 7 && i == 1) || (j == 7 && i == 2))
            {
                AI.BishopRPieces.Add(newPiece);
            }
            if ((j == 6 && i == 3) || (j == 6 && i == 4) || (j == 7 && i == 0) || (j == 7 && i == 7) || (j == 7 && i == 3) || (j == 7 && i == 4))
            {
                AI.KingPieces.Add(newPiece);
            }
        }
        else
        {
            if ((j == 1 && i == 0) || (j == 1 && i == 1) || (j == 1 && i == 2) || (j == 0 && i == 1) || (j == 0 && i == 2))
            {
                AI.PlayerBishopLPieces.Add(newPiece);
            }
            if ((j == 1 && i == 7) || (j == 1 && i == 6) || (j == 1 && i == 5) || (j == 0 && i == 6) || (j == 0 && i == 5))
            {
                AI.PlayerBishopRPieces.Add(newPiece);
            }
            if ((j == 1 && i == 3) || (j == 1 && i == 4) || (j == 0 && i == 0) || (j == 0 && i == 7) || (j == 0 && i == 3) || (j == 0 && i == 4))
            {
                AI.PlayerKingPieces.Add(newPiece);
            }
        }

        newPiece.GetComponent<IPieceBase>().CurrColPos = j;
        newPiece.GetComponent<IPieceBase>().CurrRowPos = i;

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

        newPiece.transform.position = boardArr[i, j].transform.position /*+ new Vector3(0, 0.01f, 0)*/;
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

        newCell.transform.position = (startPos - cellRenderer.bounds.min) + (new Vector3(offsetX, 0.025f, offsetZ));
        return newCell;
    }
}
