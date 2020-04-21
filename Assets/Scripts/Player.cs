using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;
    private CharacterController controller;

    // Offset is used to adjust the distance for ground check. We want to find
    // the minimum value here, but still > 0. Must play around with jumping
    // in game to find a value that works fluidly
    private float raycastMaxDist;
    [SerializeField] float raycastOffset = 0.1f;


    // for use with gravity
    private Vector3 currentFallingVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        raycastMaxDist = controller.bounds.extents.y;
    }

    private void FixedUpdate()
    {
        BasicMovement();
        ApplyGravity();
        GroundCheck();
    }

    // Purpose is twofold: 1) reset gravity when player lands
    // 2) reset jump trigger to know if we can jump again
    private void GroundCheck()
    {
        bool grounded = CastRayToGround();
        if (grounded)
        {
            currentFallingVelocity = Vector3.zero;
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
        Vector3 temp = new Vector3(transform.position.x, transform.position.y - (raycastMaxDist + raycastOffset), transform.position.z);
        Debug.DrawLine(transform.position, temp, Color.red);
    }
}
