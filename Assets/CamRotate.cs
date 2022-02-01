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

            Vector3 vector = new Vector3(-xAngle, -yAngle, -zAngle);
             transform.Rotate(vector * Time.deltaTime);
        //Detect when the left arrow key has been released

            Debug.Log("Q key was released.");
        }

        if (Input.GetKey(KeyCode.E))
        {        
             Vector3 vector = new Vector3(xAngle, yAngle, zAngle);
             transform.Rotate(vector * Time.deltaTime);

        //Detect when the left arrow key has been released
            Debug.Log("E key was released.");
        }
    }
}
