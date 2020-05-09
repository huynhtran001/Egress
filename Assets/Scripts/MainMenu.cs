using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        // Load first scene
        SceneManager.LoadScene(1);
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am) am.PlayNextLevelMusic();
    }

    public void Quit()
    {
        // Quits game
        Application.Quit();
    }
}
