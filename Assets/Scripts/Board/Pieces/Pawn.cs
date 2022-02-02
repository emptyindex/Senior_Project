using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : BasePiece
{
    public bool MoveUp { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override List<GameObject> Highlight(GameObject[,] board, int x, int y)
    {
        switch (MoveUp)
        {
            case true:
                return base.HighlightCells(board, x, y, 1, true, true, false, false, true, false, false, false);
            case false:
                return base.HighlightCells(board, x, y, 1, false, false, true, true, false, true, false, false);
        }
    }
}
