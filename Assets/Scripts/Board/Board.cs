using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [HideInInspector]
    public Vector3 scale = new Vector3(100, 1, 100);

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
        this.transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
