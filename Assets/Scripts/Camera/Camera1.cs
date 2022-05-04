using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera1 : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;
    public float minZoomDist;
    public float maxZoomDist;

    private Vector3 snapPosition;

    public void Awake()
    {
        snapPosition = transform.position;
        SnapToPosition();
    }

    void Update() 
    {
        Move();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SnapToPosition();
        }
    }
    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * zInput + transform.right * xInput;

        transform.position += dir*moveSpeed * Time.deltaTime;
    }

    private void SnapToPosition()
    {
        this.transform.position = snapPosition;
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public void FocusOnPosition (Vector3 pos) {
        transform.position = pos;
        
    }
}
