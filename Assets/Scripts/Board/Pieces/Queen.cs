using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : BasePiece
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: Add Jump Restriction
    public List<GameObject> HighlightCells(GameObject[,] board, int x, int y)
    {
        return base.HighlightCells(board, x, y, 3);
    }
}
