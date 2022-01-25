using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 centerPosition;
    public Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = centerPosition;
        this.transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
