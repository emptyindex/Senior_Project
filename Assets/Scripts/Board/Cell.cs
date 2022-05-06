using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents each cell or square that comprise the chess board.
/// </summary>
public class Cell : MonoBehaviour
{
    public Material highlightBorder;
    public Material highlightAttackBorder;
    public Material aIMoveBorder;

    public bool containsPiece;

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

    private void FixedUpdate()
    {
        if (Physics.Raycast(gameObject.transform.position - new Vector3(0, 0.5f, 0), transform.up, out RaycastHit hit, Mathf.Infinity, 1 << 3) && hit.transform.gameObject.CompareTag("Piece"))
        {
            contains = hit.transform.gameObject;
        }
        else
        {
            contains = null;
        }
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
        
        if (IsAttackHighlighted)
        {
            renderer.material = highlightAttackBorder;
        }
        
        if(!IsAIHighlighted && !IsHighlighted && !IsAttackHighlighted)
        {
            renderer.material = originalColor;
        }

        containsPiece = contains != null;
    }

    public void ChangeAIBorder()
    {
        renderer.material = aIMoveBorder;
    }

    public bool IsAIHighlighted { get; set; }

    /// <summary>
    /// Gets and sets the highlighted status of this cell.
    /// </summary>
    public bool IsHighlighted { get; set; }
    public bool IsAttackHighlighted { get; set; }

    /// <summary>
    /// Gets and sets the current piece on this cell.
    /// </summary>
    public GameObject GetCurrentPiece
    {
        get => contains;
        private set => contains = value;
    }
}