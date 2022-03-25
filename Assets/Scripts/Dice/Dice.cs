using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dice : MonoBehaviour
{
    public GameManager manager;
    private BasePlayer[] players;

    private float time = 0.0f;
    private bool isStarted = false;

    Rigidbody rb;
    //Vector3 startPos;

    //Vector3 diceVelocity ;

    public delegate void DiceFinishedEvent();
    public event DiceFinishedEvent OnDiceEnded;
    private bool hasFinished = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody> ();

        //startPos = transform.position;

        /*players = manager.GetBasePlayers();

        foreach(var p in players)
        {
            p.AttackRollNeeded += Roll;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //diceVelocity = rb.velocity;

        //if(diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        //{
        //   Collider [] hitColliders = Physics.OverlapSphere(Vector3.right, 0.005f);
        //   foreach (var collider in hitColliders)
        //   {
        //       print("Hit Collidors"+collider.gameObject.name);
        //   }      
        //}
        if (isStarted)
        {
            time = time + Time.deltaTime;
            if (rb.velocity.magnitude == 0 && !hasFinished && time > 3f)
            {
                OnDiceEnded.Invoke();
                hasFinished = true;
                isStarted = false;
                time = 0f;
            }

        }
    }

    public void Roll()
    {
        DiceNumberTextScript.diceNumber = 0;

        float dirX = Random.Range(-700, 700);
        float dirY = Random.Range(-700, 700);
        float dirZ = Random.Range(-700, 700);

        isStarted = true;
        hasFinished = false;

        transform.SetPositionAndRotation(transform.position, transform.rotation);

        rb.AddForce(transform.up * 1100);
        rb.AddTorque(dirX, dirY, dirZ);

        
    }
}
