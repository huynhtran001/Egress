using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private CharacterController controller;
    private MouseCamera playerCam;
    [SerializeField] GameObject hud;
    [SerializeField] float movementSpeed = 15f;
    [SerializeField] float jumpPower = 0.1f;
    [Tooltip("Controls how long the player jumps for when jump is pressed")]
    [SerializeField] float secondsToApplyJump = 0.5f;
    [SerializeField] float maxInteractionDistance = 7f;
    [Tooltip("Controls how much force is applied to throwing objects")]
    [SerializeField] float forceMultiplier = 5000f;

    // Offset is used to adjust the distance for ground check. We want to find
    // the minimum value here, but still > 0. Must play around with jumping
    // in game to find a value that works fluidly
    private float raycastMaxDist;
    [SerializeField] float raycastOffset = 0.4f;

    // for use with gravity
    private Vector3 currentFallingVelocity = new Vector3(0, -0.2f, 0);
    private bool grounded;
    private bool isJumping = false;
    private float jumpingLength = 0f;

    // Global references to things we need to move the interactable around
    private GameObject heldObject = null;
    private List<GameObject> frozenObjects;
    private GameObject lastHeldObject = null;

    // Audio section
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip terminalSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip freezeSound;
    [SerializeField] AudioClip unfreezeSound;

    public enum PlayerState { Alive, Interacting, Death, Paused}
    public PlayerState playerState;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        raycastMaxDist = controller.bounds.extents.y;

        frozenObjects = new List<GameObject>();
        playerState = PlayerState.Alive;

        gameManager = FindObjectOfType<GameManager>();
        playerCam = GetComponentInChildren<MouseCamera>();
    }

    private void Update()
    {
        if (playerState == PlayerState.Death) return;
        RaycastDebugLines();
        Interact();
        Jump();
        MoveObject();
    }

    private void FixedUpdate()
    {
        BasicMovement();
        ApplyGravity();
        GroundCheck();
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpObject();
        }

        if (heldObject && Input.GetMouseButtonDown(0))
        {
            ThrowObject();
        }

        if (heldObject && Input.GetKeyDown(KeyCode.F) || lastHeldObject && Input.GetKeyDown(KeyCode.F))
        {
            FreezeObject();
        }

        if (Input.GetMouseButtonDown(1))
        {
            UnfreezeAllObjects();
        }
    }

    public void Die()
    {
        PlayClip(deathSound);
        playerState = PlayerState.Death;
        StartCoroutine(AfterDeath());
    }

    IEnumerator AfterDeath()
    {
        // death timer before reloading
        yield return new WaitForSecondsRealtime(3f);
        gameManager.RestartLevel();
    }

    // Called from other gameobjects when the player steps into collision range
    // passes along text to game manager to display
    // Reason is because we need to meet 2 "conditions" based on terminal and player script
    public void WithinRange(string otherTag, Text text, GameObject terminalScreen)
    {
        RaycastHit[] hits;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        hits = Physics.RaycastAll(ray, maxInteractionDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(otherTag))
            {
                gameManager.DisplayText(text);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    BeginInteraction(terminalScreen);
                }

                return;
            }
        }
        gameManager.HideText(text);
    }

    private void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayTerminalSound()
    {
        audioSource.Stop();
        audioSource.clip = terminalSound;
        audioSource.Play();
    }

    private void BeginInteraction(GameObject terminalScreen)
    {
        PlayClip(terminalSound);
        // pause game
        Time.timeScale = 0f;
        terminalScreen.SetActive(true);
        TerminalScreen terminal = terminalScreen.GetComponent<TerminalScreen>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        hud.SetActive(false);
        playerState = PlayerState.Interacting;
        terminal.StartTerminal(hud);
    }

    // helper function
    private void UnfreezeAllObjects()
    {
        foreach (GameObject item in frozenObjects)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
        if (frozenObjects.Count > 0) PlayClip(unfreezeSound);

        frozenObjects.Clear();
    }

    // helper function
    private void UnfreezeObject(GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        frozenObjects.Remove(item);
    }

    // helper function
    private void FreezeObject()
    {
        // Freeze object by making it kinematic
        if (heldObject)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            // Add to list of frozen objects, then also set reference to current held
            // object to null
            frozenObjects.Add(heldObject);
            heldObject = null;
            PlayClip(freezeSound);
        } else if (lastHeldObject)
        {
            Rigidbody rb = lastHeldObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
            frozenObjects.Add(lastHeldObject);
            lastHeldObject = null;
            PlayClip(freezeSound);
        }
    }

    // helper function
    private void PickUpObject()
    {
        // If already holding onto something, release the object gently
        if (heldObject)
        {
            ReleaseObject();
            return;
        }

        // sends out a raycast where the Camera is pointing
        RaycastHit[] hits;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        hits = Physics.RaycastAll(ray, maxInteractionDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;
                // Check to see if frozen. If it is, unfreeze to throw.
                UnfreezeObject(heldObject);
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
            }
        }
    }

    // helper function
    private void ReleaseObject()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        heldObject = null;
    }

    // helper function
    // Note: remember to give the object some mass. ~300 is good
    private void ThrowObject()
    {
        // We want to launch the object
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        // Make global ref null so MoveObject() doesn't override the object's position
        lastHeldObject = heldObject;
        heldObject = null;

        Vector3 forceDirection = Camera.main.transform.forward;
        rb.useGravity = true;
        rb.AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
    }


    private void MoveObject()
    {
        if (!heldObject) return;

        heldObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 7;
    }

    // Purpose is twofold: 1) reset gravity when player lands
    // 2) reset jump trigger to know if we can jump again
    private void GroundCheck()
    {
        grounded = CastRayToGround();
        if (grounded)
        {
            // Only reset gravity if the Y velocity is already negative
            // Set to -0.2f instead of 0 because it feels better
            if (currentFallingVelocity.y <= -0.2f) currentFallingVelocity.y = -0.2f;
            //TODO: implement jump somewhere here
        }
    }

    private bool CastRayToGround()
    {
        // raycastMaxDist is found through the object's collider extents
        // then a tiny offset is added so we can raycast and detect anything
        // below the object
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, raycastMaxDist + raycastOffset);

        // Tests for collider nullity. null = false
        if (hit.collider) { return true; } 
        else { return false; }
    }

    private void ApplyGravity()
    {
        // Freefall formula = 1/2 * g * t^2
        currentFallingVelocity += Physics.gravity * 0.5f * Time.fixedDeltaTime * Time.fixedDeltaTime;
    }

    private void BasicMovement()
    {
        float xMove = 0f;
        float zMove = 0f;

        if (playerState == PlayerState.Alive)
        {
            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");
        }
        

        // Since we want to move right/forward relative to the player, we
        // use transform.right/forward to get that direction for us
        Vector3 direction = transform.right * xMove + transform.forward * zMove;
        direction *= movementSpeed * Time.fixedDeltaTime;
        direction.y = currentFallingVelocity.y;

        // If player is jumping, apply this to y instead. If player
        // isn't jumping, keep y values
        if (isJumping)
        {
            direction.y = Vector3.up.y * jumpPower;
            jumpingLength += Time.deltaTime;
            if (jumpingLength >= secondsToApplyJump)
            {
                isJumping = false;
                jumpingLength = 0f;
            }
        }

        controller.Move(direction);
    }

    private void Jump()
    {
        if (grounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            PlayClip(jumpSound);
        }

    }

    private void RaycastDebugLines()
    {
        // ground check
        Vector3 temp = new Vector3(transform.position.x, transform.position.y - (raycastMaxDist + raycastOffset), transform.position.z);
        Debug.DrawLine(transform.position, temp, Color.red);

        // camera raycast check
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * maxInteractionDistance, Color.green);

    }
}
