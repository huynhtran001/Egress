using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerminalScreen : MonoBehaviour
{
    [Tooltip("Not case sensitive")]
    [SerializeField] LastDoor doorToOpen;
    [SerializeField] string password;
    [Tooltip("Drag the main text object to be changed here, should be a child of this game object")]
    [SerializeField] Text terminalText;
    [Tooltip("Leave blank if you don't want a message to show")]
    [SerializeField] GameObject inputField;
    [SerializeField] Text inputText;
    [Tooltip("Messages to display when screen boots up")]
    [TextArea(3, 4)] [SerializeField] string[] messages;
    [SerializeField] float timeBetweenMessages = 3f;
    [Tooltip("Delay inbetween character typing rate")]
    [SerializeField] float textDelay = 0.02f;

    // For use with event system
    private InputField actualInputField;

    // Queues are easier to work with coroutines, but keep array of strings
    // for easy edits from the inspector
    private Queue<string> messageQueue = new Queue<string>();

    // to display and hide after interacting
    private GameObject hud;
    private bool currentlyInteracting = false;

    // Start is called before the first frame update
    void Start()
    {
        if (inputField) actualInputField = inputField.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        TryToCloseTerminal();
        if (inputField) InputField();
    }

    private void InputField()
    {
        // Just focuses on the input field so we can type
        if (!EventSystem.current.currentSelectedGameObject) actualInputField.Select();

        bool keyPress = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return); 
        if (EventSystem.current.currentSelectedGameObject.Equals(inputField) && keyPress)
        {
            if (inputText.text.ToUpper().Equals(password.ToUpper()))
            {
                doorToOpen.OpenDoor();
                CloseTerminal();
            }
        }
    }

    private void TryToCloseTerminal()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentlyInteracting)
        {
            CloseTerminal();
        }
    }

    private void CloseTerminal()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
        hud.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentlyInteracting = false;

        // a "hack". need player reference to change player state
        Player player = FindObjectOfType<Player>();
        player.playerState = Player.PlayerState.Alive;
        player.PlayTerminalSound();
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
        terminalText.text = "";
        foreach (char c in msg.ToCharArray())
        {
            terminalText.text += c;
            yield return new WaitForSecondsRealtime(textDelay);
        }

        // Wait some time, then recursively call itself.
        yield return new WaitForSecondsRealtime(timeBetweenMessages);
        WelcomeMessage();
    }
}
