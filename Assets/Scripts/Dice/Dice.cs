using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dice : MonoBehaviour
{
    public GameManager manager;

    private float time = 0.0f;
    private bool isStarted = false;

    Rigidbody rb;
    Vector3 startPos;
    Quaternion startRot;

    //Vector3 diceVelocity;

    public delegate void DiceFinishedEvent();
    public event DiceFinishedEvent OnDiceEnded;
    private bool hasFinished = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody> ();
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            time += Time.deltaTime;
            if (rb.velocity.magnitude == 0 && !hasFinished && time > 3f)
            {
                OnDiceEnded.Invoke();
                hasFinished = true;
                isStarted = false;
                time = 0f;

                transform.position = startPos;
                transform.rotation = startRot;
            }
        }
    }

    public void Roll()
    {
        DiceNumberTextScript.diceNumber = 0;

        float dirX = Random.Range(-900, 900);
        float dirY = Random.Range(-900, 900);
        float dirZ = Random.Range(-900, 900);

        //transform.SetPositionAndRotation(transform.position, transform.rotation);

        rb.AddForce(transform.up * 100);
        rb.AddTorque(dirX, dirY, dirZ);

        isStarted = true;
        hasFinished = false;
    }
}
