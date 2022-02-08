using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dice : MonoBehaviour
{   
    Rigidbody rb;
    Vector3 diceVelocity ;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = rb.velocity;

        if(Input.GetKeyDown(KeyCode.Space)){
            DiceNumberTextScript.diceNumber = 0;
            float dirX = Random.Range(0,500);
            float dirY = Random.Range(0,500);
            float dirZ = Random.Range(0,500);
            transform.position = new Vector3(0,2,0);
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * 500);
            rb.AddTorque (dirX, dirY, dirZ);

        }
        
        if(diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
           Collider [] hitColliders = Physics.OverlapSphere(Vector3.right, 0.005f);
           foreach (var collider in hitColliders)
           {
               print("Hit Collidors"+collider.gameObject.name);
           }
            
        }
    }
}
