using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio1 : MonoBehaviour
{


    public AudioSource mouseClickSound;
   

    public void playSoundEffect()
    {
        mouseClickSound.Play();
    }



}
