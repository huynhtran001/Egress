using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        PlayCurrentLevelMusic();
    }

    public void PlayCurrentLevelMusic()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene < clips.Length) PlayClip(clips[currentScene]);
    }

    // Call after loading next scene
    public void PlayNextLevelMusic()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (currentScene < clips.Length) PlayClip(clips[currentScene]);
    }

    // Plays the audioclip passed into this function
    void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
