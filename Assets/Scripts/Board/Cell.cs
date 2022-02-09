using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents each cell or square that comprise the chess board.
/// </summary>
public class Cell : MonoBehaviour
{
    public Material highlightBorder;

    private Material originalColor;
    private GameObject contains;

    private new Renderer renderer;

    /// <summary>
    /// Start is called before the first frame update.
    /// Sets up the renderer component and gets the original cell color for the highlighting border.
    /// </summary>
    void Start()
    {
        renderer = GetComponent<Renderer>();

        originalColor = renderer.material;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Checks if this cell should be highlighted, if so, change the color to yellow
    /// otherwise, return the cell to the original color.
    /// </summary>
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

    /// <summary>
    /// Gets and sets the highlighted status of this cell.
    /// </summary>
    public bool IsHighlighted { get; set; }

    /// <summary>
    /// Gets and sets the current piece on this cell.
    /// </summary>
    public GameObject GetCurrentPiece
    {
        get => contains;
        set => contains = value;
    }
}