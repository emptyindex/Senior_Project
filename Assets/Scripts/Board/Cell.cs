using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private GameObject contains;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetCurrentPiece
    {
        get => contains;
        set => contains = value;
    }

    public bool hasPiece()
    {
        return contains != null;
    }
}
