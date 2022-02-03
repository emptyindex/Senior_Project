using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera1 : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;
    public float minZoomDist;
    public float maxZoomDist;

    

    private Camera cam;

    public void awake()
    {
        cam = Camera.main;
    }
    
    void Update() {
        Move();
        
    }
    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * zInput + transform.right * xInput;

        transform.position += dir*moveSpeed * Time.deltaTime;
    } 
        
    public void FocusOnPosition (Vector3 pos) {
        transform.position = pos;
        
    }
}
