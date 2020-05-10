using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Player player;
    [SerializeField] GameObject playerHud;

    private void Awake()
    {
        // Singleton
        int x = FindObjectsOfType<GameManager>().Length;

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

    private void Update()
    {
        PauseMenu();
    }

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

    private void PauseMenuOff()
    {
        player.playerState = Player.PlayerState.Alive;
        Time.timeScale = 1f;
        playerHud.SetActive(true);
        pauseMenu.SetActive(false);
    }

    private void PauseMenuOn()
    {
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
