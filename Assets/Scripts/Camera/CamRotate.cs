using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    private readonly float xAngle = 40, yAngle = 40;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            ChangeRotation(0, -yAngle, 0);

            //Detect when the left arrow key has been released
            Debug.Log("Q key was released.");
        }

        if (Input.GetKey(KeyCode.E))
        {
            ChangeRotation(0, yAngle, 0);

            //Detect when the left arrow key has been released
            Debug.Log("E key was released.");
        }

        if(Input.GetKey(KeyCode.Z))
        {
            ChangeRotation(xAngle, 0, 0);
        }

        if (Input.GetKey(KeyCode.X))
        {
            ChangeRotation(-xAngle, 0, 0);
        }
    }

    private void ChangeRotation(float xAngle, float yAngle, float zAngle)
    {
        Vector3 vector = new Vector3(xAngle, yAngle, zAngle);
        transform.Rotate(vector * Time.deltaTime);
    }
}
