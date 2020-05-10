using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Player player;
    [SerializeField] GameObject playerHud;

    private void Update()
    {
        PauseMenu();
    }


    // We can refactor pause menu functionality into its own script if we have more time
    void PauseMenu()
    {
        if (!(player.playerState == Player.PlayerState.Alive || player.playerState == Player.PlayerState.Paused)) return;

        if (Input.GetKeyDown(KeyCode.Escape) && player.playerState == Player.PlayerState.Alive)
        {
            PauseMenuOn();
        } else if (Input.GetKeyDown(KeyCode.Escape) && player.playerState == Player.PlayerState.Paused)
        {
            PauseMenuOff();
        }
    }

    public void RestartLevel()
    {
        PauseMenuOff();
        int x = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(x);
    }

    public void PauseMenuOff()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.playerState = Player.PlayerState.Alive;
        Time.timeScale = 1f;
        playerHud.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void PauseMenuOn()
    {
        Cursor.lockState = CursorLockMode.None;
        player.playerState = Player.PlayerState.Paused;
        Time.timeScale = 0f;
        playerHud.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void DisplayText(Text text)
    {
        if (text)
        {
            text.enabled = true;
        }
    }

    public void HideText(Text text)
    {
        if (text) text.enabled = false;
    }

    public void ToggleText(Text text)
    {
        if (text) text.enabled = !text.enabled;
    }
}
