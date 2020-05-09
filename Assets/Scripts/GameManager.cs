using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
