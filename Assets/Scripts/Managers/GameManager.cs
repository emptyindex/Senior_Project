using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject board;

    public GameObject whiteCell;
    public GameObject blackCell;

    private GameObject[,] boardArr = new GameObject[8,8];

    // Start is called before the first frame update
    void Start()
    {
        Object.Instantiate(board);

        var renderer = board.GetComponent<Renderer>();
        var startPos = renderer.bounds.max;

        for(int i = 0; i < boardArr.GetLength(0); i++)
        {
            for(int j = 0; j < boardArr.GetLength(1); j++)
            {
                if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                {
                    var newCell = Object.Instantiate(whiteCell);
                    newCell.transform.SetParent(board.transform);

                    newCell.transform.position = startPos + new Vector3(i , 0, j);

                    boardArr[i, j] = newCell;
                }
                else
                {
                    boardArr[i, j] = Object.Instantiate(blackCell);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
