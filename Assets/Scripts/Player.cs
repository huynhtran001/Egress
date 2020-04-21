using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float maxInteractionDistance = 7f;
    [Tooltip("Controls how much force is applied to throwing objects")]
    [SerializeField] float forceMultiplier = 10f;

    // Offset is used to adjust the distance for ground check. We want to find
    // the minimum value here, but still > 0. Must play around with jumping
    // in game to find a value that works fluidly
    private float raycastMaxDist;
    private const float raycastOffset = 0.1f;

    // for use with gravity
    private Vector3 currentFallingVelocity = new Vector3(0, -0.2f, 0);
    private bool grounded;

    // Global references to things we need to move the interactable around
    private GameObject heldObject = null;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        raycastMaxDist = controller.bounds.extents.y;
    }

    private void Update()
    {
        RaycastDebugLines();
        Interact();

    }

    private void FixedUpdate()
    {
        BasicMovement();
        ApplyGravity();
        GroundCheck();
    }

    private void Interact()
    {
        MoveObject();

        if (Input.GetKeyDown(KeyCode.E))
        {
            // sends out a raycast where the Camera is pointing
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Physics.Raycast(ray, out hit, maxInteractionDistance);
            
            if (hit.collider == null) return;

            if (hit.collider.CompareTag("Interactable"))
            {
                heldObject = hit.collider.gameObject;
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
            }
        }

        if (heldObject && Input.GetMouseButtonDown(0))
        {
            // We want to launch the object
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            
            // Make global ref null so MoveObject() doesn't override the object's position
            heldObject = null;

            Vector3 forceDirection = Camera.main.transform.forward;
            rb.useGravity = true;
            rb.AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
        }
    }
    
    private void MoveObject()
    {
        if (!heldObject) return;

        heldObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
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
        // Gameplay feels better without the 1/2
        
        currentFallingVelocity += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        controller.Move(currentFallingVelocity);
    }

    private void BasicMovement()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        // Since we want to move right/forward relative to the player, we
        // use transform.right/forward to get that direction for us
        Vector3 direction = transform.right * xMove + transform.forward * zMove;
        direction *= movementSpeed * Time.fixedDeltaTime;

        controller.Move(direction);
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
