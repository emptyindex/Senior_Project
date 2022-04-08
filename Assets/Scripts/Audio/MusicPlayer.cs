using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
 {
  public AudioSource audioSource;
  public AudioClip[] audioClipArray;
  AudioClip lastClip;
  void Start() 
  {
    audioSource.PlayOneShot(RandomClip());
  }
  AudioClip RandomClip()
  {
    int attempts = 3;
    AudioClip newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    while (newClip == lastClip && attempts > 0) 
    {
      newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
      attempts--;
    }
    lastClip = newClip;
    return newClip;
  }
}