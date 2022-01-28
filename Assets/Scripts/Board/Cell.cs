using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material highlightBorder;

    private Material originalColor;
    private GameObject contains;

    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        originalColor = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsHighlighted)
        {
            renderer.material = highlightBorder;
        }
        else
        {
            renderer.material = originalColor;
        }
    }

    public bool IsHighlighted { get; set; }

    public GameObject GetCurrentPiece
    {
        get => contains;
        set => contains = value;
    }

    public bool HasPiece()
    {
        return contains != null;
    }
}
