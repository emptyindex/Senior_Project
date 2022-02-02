using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : BasePlayer
{
    private new Camera camera;

    private GameObject[] pieces = new GameObject[16];

    private List<GameObject> highlightedCells = new List<GameObject>();

    private Cell previousCell;

    private bool hasSelectedAPiece = false;

    public bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && Input.GetMouseButtonUp(0))
        {
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity);

            var cell = hit.transform.gameObject.GetComponent<Cell>();
            var obj = hit.transform.gameObject;

            if (cell)
            {
                if (cell.GetCurrentPiece && pieces.Contains(cell.GetCurrentPiece))
                {
                    if (!hasSelectedAPiece)
                    {
                        previousCell = cell;
                        hasSelectedAPiece = true;

                        highlightedCells = Manager.SetSelectedPiece(hit, cell);
                    }
                    else
                    {
                        ClearSelected();

                        previousCell = cell;
                        hasSelectedAPiece = true;

                        highlightedCells = Manager.SetSelectedPiece(hit, cell);
                    }
                }              
                else if(!cell.GetCurrentPiece && hasSelectedAPiece)
                {
                    if (highlightedCells.Contains(obj))
                    {
                        MovePiece(cell, obj);

                        IsTurn(false);
                        Manager.ChangeTurn(this.gameObject);

                        ClearSelected();
                    }
                } 
            }
        }
    }

    public override void IsTurn(bool newVal)
    {
        canMove = newVal;
    }

    private void ClearSelected()
    {
        previousCell = null;

        hasSelectedAPiece = false;

        highlightedCells.ForEach(c => c.GetComponent<Cell>().IsHighlighted = false);
        highlightedCells.Clear();
    }

    private void MovePiece(Cell cell, GameObject obj)
    {
        foreach (Transform child in previousCell.transform)
        {
            if (child == null || child.name == "InnerCell")
                continue;

            child.transform.parent = obj.transform;
            child.transform.localPosition = new Vector3(0, 0.02f, 0);
        }

        cell.GetCurrentPiece = previousCell.GetCurrentPiece;
        previousCell.GetCurrentPiece = null;
    }
    public GameManager Manager { get; set; }

    public void SetPieces(GameObject[] setPieces)
    {
        pieces = setPieces;
    }
}
