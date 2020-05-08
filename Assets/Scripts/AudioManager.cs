using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    private void Awake()
    {
        // Singleton
        int x = FindObjectsOfType<AudioManager>().Length;

        if (x > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        PlayClip(clips[currentScene]);
    }

    // Plays the audioclip passed into this function
    void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
