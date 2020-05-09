using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerminalScreen : MonoBehaviour
{
    [SerializeField] GameObject inputField;
    [SerializeField] Text inputText;
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

    // to display and hide after interacting
    private GameObject hud;
    private bool currentlyInteracting = false;

    // Start is called before the first frame update
    void Start()
    {
        ConvertArrayToQueue();
    }

    // Update is called once per frame
    void Update()
    {
        CloseTerminal();
        InputField();
    }

    private void InputField()
    {
        if (!EventSystem.current.currentSelectedGameObject) return;

        bool keyPress = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return); 
        if (EventSystem.current.currentSelectedGameObject.Equals(inputField) && keyPress)
        {
            print(inputText.text);
        }
    }

    private void CloseTerminal()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentlyInteracting)
        {
            StopAllCoroutines();
            Time.timeScale = 1f;
            this.gameObject.SetActive(false);
            hud.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            currentlyInteracting = false;
        }
    }

    public void StartTerminal(GameObject hud)
    {
        this.hud = hud;
        currentlyInteracting = true;
        ConvertArrayToQueue();
        WelcomeMessage();
    }

    private void ConvertArrayToQueue()
    {
        messageQueue.Clear();
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
