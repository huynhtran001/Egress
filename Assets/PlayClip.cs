using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClip : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    // Simple script that plays a single audioclip

    public void PlayAudioClip()
    {
        AudioSource tempAS = new AudioSource();
        tempAS.clip = audioClip;
        tempAS.Play();
    }
}
