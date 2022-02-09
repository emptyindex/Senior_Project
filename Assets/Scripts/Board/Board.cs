using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the table/plane that the chess board is built on top of.
/// </summary>
public class Board : MonoBehaviour
{
    [HideInInspector]
    public Vector3 scale = new Vector3(100, 1, 100);

    /// <summary>
    /// Start is called before the first frame update.
    /// Ensures that the board is perfectly centered in the world
    /// and is the correct scale.
    /// </summary>
    void Start()
    {
        this.transform.position = Vector3.zero;
        this.transform.localScale = scale;
    }
}
