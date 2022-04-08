using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the human player.
/// </summary>
public class PlayerManager : BasePlayer
{
    public GameObject[] pieces = new GameObject[16];

    private List<GameObject> corpsCommanders = new List<GameObject>();

    private List<GameObject> usedCommanders = new List<GameObject>();

    private Camera camera;

    private GameObject selectedPiece = null;

    private int movesTaken = 0;
    private readonly int maxMovesPerTurn = 3;

    private readonly string endTurnButtonName = "EndTurnButton";
    private GameObject endTurnButton;

    public override void SetPieces(List<GameObject> pieces)
    {
        this.pieces = pieces.ToArray();
    }

    public void EndTurn()
    {
        canMove = false;

        //corpsCommanders.ForEach(c => c.GetComponent<BasePiece>().spotLight.enabled = false);
        corpsCommanders.ForEach(c => c.GetComponent<PieceColorManager>().SetHighlight(false));

        usedCommanders.Clear();
        movesTaken = 0;

        endTurnButton.SetActive(false);

        IsTurn(false);
        Manager.ChangeTurn(this.gameObject);
    }

    public List<GameObject> GetBishopCommanders()
    {
        return corpsCommanders.Where(c => c.GetComponent<Bishop>()).ToList();
    }

    private void Start()
    {
        if(gameObject.transform.GetChild(0).GetComponent<Canvas>())
        {
            endTurnButton = gameObject.transform.GetChild(0).Find(endTurnButtonName).gameObject;
            endTurnButton.SetActive(false);
        }

        camera = Camera.main;
        corpsCommanders = pieces.Where(p => p.GetComponent<CommanderController>() != null).ToList();

        this.AssignCorpsPieces();

        corpsCommanders.ForEach(c => c.GetComponent<CommanderController>().OnCommandEnded += UpdateMoves);
        corpsCommanders.ForEach(c => c.GetComponent<CommanderController>().player = this);
    }

    /// <summary>
    /// Update is called once per frame.
    /// If it's this player's turn and the user has pressed the left mouse button,
    /// then check if the player is selecting a piece to move or a place to move the piece.
    /// If the player is chosing a piece to move, highlight all the valid moves. 
    /// Otherwise, if the player has already selected a piece and has clicked on a valid 
    /// square, move the piece and tell the GameManager that our turn has ended.
    /// </summary>
    void Update()
    {
        if(movesTaken >= maxMovesPerTurn)
        {
            canMove = false;
        }

        if(canMove)
        {
            endTurnButton.SetActive(true);

            if (!selectedPiece)
            {
                HighlightCorpsCommanders();

                if (Input.GetMouseButtonUp(0))
                {
                    Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

                    if (hit.transform)
                    {
                        var cell = hit.transform.gameObject.GetComponent<Cell>();

                        // if you clicked on a cell with one of the corps commanders. Also checks that the corps commander has not already taken its turn
                        if (cell && cell.GetCurrentPiece && corpsCommanders.Contains(cell.GetCurrentPiece) && !usedCommanders.Contains(cell.GetCurrentPiece))
                        {
                            selectedPiece = cell.GetCurrentPiece;

                            selectedPiece.GetComponent<CommanderController>().HasTakenCommand = true;
                            corpsCommanders.ForEach(c =>
                            {
                                if (!c.Equals(selectedPiece))
                                    c.GetComponent<PieceColorManager>().SetHighlight(false);
                            });
                        }
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            InvokeAttackRoll();
        }
    }

    private void AssignCorpsPieces()
    {
        var bishops = corpsCommanders.Where(c => c.GetComponent<Bishop>() != null);
        var piecesLeft = pieces.ToList();

        foreach (var commander in bishops)
        {
            var posX = commander.GetComponent<BasePiece>().CurrRowPos;
            var posY = commander.GetComponent<BasePiece>().CurrColPos;

            bool isLeftSide = true;

            if (posX > 3)
            {
                isLeftSide = false;
            }

            commander.GetComponent<CommanderController>().controlledPieces = pieces.Where(p => CanAdd(isLeftSide, p, posX, posY)
                && p.GetComponent<Rook>() == null).ToList();

            piecesLeft = piecesLeft.Except(commander.GetComponent<CommanderController>().controlledPieces).ToList();
        }

        var king = corpsCommanders.Where(c => c.GetComponent<KingCorpsCommander>() != null).FirstOrDefault().GetComponent<KingCorpsCommander>();
        king.SetStartingPieces(piecesLeft);
    }

    private bool CanAdd(bool isLeftSide, GameObject pieceToCheck, int posX, int posY)
    {
        var newX = pieceToCheck.GetComponent<BasePiece>().CurrRowPos;
        var newY = pieceToCheck.GetComponent<BasePiece>().CurrColPos;

        bool boolRange;

        if(isLeftSide)
        {
            boolRange = newX <= posX;
        }
        else
        {
            boolRange = newX >= posX;
        }

        return boolRange 
            && Mathf.Abs(posX - newX) <= 2
            && Mathf.Abs(newY - posY) <= 1;
    }

    private void UpdateMoves()
    {
        movesTaken++;
        usedCommanders.Add(selectedPiece);
        selectedPiece = null;

        Debug.Log("moves taken: " + movesTaken);
    }

    private void HighlightCorpsCommanders()
    {
        var toHighlight = corpsCommanders.Except(usedCommanders).ToList();

        //toHighlight.ForEach(c => c.GetComponent<BasePiece>().spotLight.enabled = true);
        toHighlight.ForEach(c => c.GetComponent<PieceColorManager>().SetHighlight(true));
    }
}
