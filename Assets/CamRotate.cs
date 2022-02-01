using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
public float xAngle, yAngle, zAngle;






        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        


        if (Input.GetKey(KeyCode.Q))
        {
             
             transform.Rotate(-xAngle, -yAngle, -zAngle);

        //Detect when the left arrow key has been released
            Debug.Log("Q key was released.");
        }

        if (Input.GetKey(KeyCode.E))
        {
           
            transform.Rotate(xAngle, yAngle, zAngle);

        //Detect when the left arrow key has been released
            Debug.Log("E key was released.");
        }
    }
}
