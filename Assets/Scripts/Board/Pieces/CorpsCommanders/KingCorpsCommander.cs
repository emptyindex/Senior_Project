using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KingCorpsCommander : CommanderController
{
    public TextMeshProUGUI notesText;

    private List<GameObject> startingPieces;

    private bool isDelegating = false;
    private bool isRecalling = false;

    public void SetToDelegate()
    {
        CurrentMove = MoveToMake.Delegate;
        HideMenuAndToolTip();
    }

    public void SetToRecallDelegate()
    {
        CurrentMove = MoveToMake.RecallDelegate;
        HideMenuAndToolTip();
    }

    public void SetStartingPieces(List<GameObject> pieces)
    {
        startingPieces = new List<GameObject>(pieces);
        controlledPieces = new List<GameObject>(pieces);
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        notesText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (HasTakenCommand)
        {
            if (CurrentMove != MoveToMake.None && !isMoving && !isDelegating && !isRecalling)
            {
                switch (CurrentMove)
                {
                    case MoveToMake.Delegate:
                        // select piece to delegate
                        HighlightAllExceptCommander(controlledPieces);
                        isDelegating = SelectPiece(false, controlledPieces);

                        break;
                    case MoveToMake.RecallDelegate:
                        // select pieces that are delegated
                        //HighlightAllExceptCommander();

                        if(HighlightToRecall())
                        {
                            isRecalling = SelectPiece(false, startingPieces.Except(controlledPieces).ToList());
                        }

                        break;

                }
            }
        }


        if (isDelegating && selectedPiece)
        {
            DelegateToOther();
        }

        if (isRecalling && selectedPiece)
        {
            DelegateBackToSelf();
        }
    }

    private void DelegateToOther()
    {
        var bishops = player.GetBishopCommanders();
        bishops.ForEach(c => c.GetComponent<PieceColorManager>().SetHighlight(true));

        if (Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            // if we clicked on a cell populated with one of this corps commander's controlled pieces
            if (cell && cell.GetCurrentPiece && bishops.Contains(cell.GetCurrentPiece))
            {
                if(cell.GetCurrentPiece.GetComponent<CommanderController>().controlledPieces.Count >= 8)
                {
                    StartCoroutine(DisplayTextWaitAndInvoke(5f, "This bishop already has 8 pieces under its command! Choose another."));
                }
                else
                {
                    cell.GetCurrentPiece.GetComponent<CommanderController>().controlledPieces.Add(selectedPiece);
                    controlledPieces.Remove(selectedPiece);

                    isDelegating = false;

                    //others.ForEach(c => c.GetComponent<BasePiece>().spotLight.enabled = false); TODO: add back in later (add yield to announce commander done)
                    ResetMove();
                }             
            }
        }
    }

    private void DelegateBackToSelf()
    {
        var bishops = player.GetBishopCommanders().Select(b => b.GetComponent<CommanderController>());

        foreach (var b in from b in bishops
                          where b.controlledPieces.Exists(p => p.Equals(selectedPiece))
                          select b)
        {
            b.controlledPieces.Remove(selectedPiece);
        }

        controlledPieces.Add(selectedPiece);

        isRecalling = false;

        ResetMove();
    }

    private bool HighlightToRecall()
    {
        var difference = startingPieces.Except(controlledPieces);

        if (!difference.Any())
        {
            StartCoroutine(DisplayTextWaitAndInvoke(5f, "There are currently no pieces being delegated, please choose another action."));

            isDelegating = false;
            CurrentMove = MoveToMake.None;

            return false;
        }
        else
        {
            HighlightAllExceptCommander(difference.ToList());
        }

        return true;
    }

    IEnumerator DisplayTextWaitAndInvoke(float secondsToWait, string text)
    {
        notesText.text = text;

        yield return new WaitForSeconds(secondsToWait);

        notesText.text = "";
    }
}
