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
    [SerializeField] float timeBetweenMessages = 3f;
    [Tooltip("Delay inbetween character typing rate")]
    [SerializeField] float textDelay = 0.02f;

    // Queues are easier to work with coroutines, but keep array of strings
    // for easy edits from the inspector
    private Queue<string> messageQueue = new Queue<string>();

    // Start is called before the first frame update
    void Start()
    {
        ConvertArrayToQueue();
        WelcomeMessage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ConvertArrayToQueue()
    {
        for (int i = 0; i < messages.Length; i++)
        {
            messageQueue.Enqueue(messages[i]);
        }
    }

    // Displays all welcome messages
    private void WelcomeMessage()
    {
        // base case
        if (messageQueue.Count <= 0) return;
        
        StartCoroutine(DelayText(messageQueue.Dequeue()));
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

        // Wait some time, then recursively call itself.
        yield return new WaitForSecondsRealtime(timeBetweenMessages);
        WelcomeMessage();
    }
}
