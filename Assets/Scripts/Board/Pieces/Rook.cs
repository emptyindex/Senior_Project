using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : BasePiece
{
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
        return base.HighlightCells(board, x, y, 2);
    }
}
