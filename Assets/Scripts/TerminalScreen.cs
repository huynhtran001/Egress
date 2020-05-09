using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalScreen : MonoBehaviour
{
    [Tooltip("Messages to display when screen boots up")]
    [TextArea(3, 4)] [SerializeField] string[] messages;
    [Tooltip("Drag the main text object to be changed here")]
    [SerializeField] Text targetText;
    [Tooltip("Delay inbetween character typing rate")]
    [SerializeField] float textDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        WelcomeMessage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Displays all welcome messages
    private void WelcomeMessage()
    {
        StartCoroutine(DelayText(messages[0]));
    }

    // Gives the effect of having one character "typed" at a time
    IEnumerator DelayText(string msg)
    {
        targetText.text = "";
        foreach (char c in msg.ToCharArray())
        {
            targetText.text += c;
            yield return new WaitForSecondsRealtime(textDelay);
        }
    }
}
