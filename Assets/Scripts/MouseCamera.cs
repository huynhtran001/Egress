using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows user to move the camera around with a mouse

public class MouseCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 100f;

    // Useful incase we want to attach this camera script to another object,
    // such as another camera, perspective, etc.
    [Tooltip("Enter the camera's parent's tag. Default is the Player tag")]
    [SerializeField] string objectTag = "Player";
    
    // Need this to rotate camera about the object
    private Transform objectBody;

    private float xRotate = 0f;

    void Start()
    {
        // Note - this script assumes it is attached to the camera and is a
        // child of the object's body
        FindParentTransform();

        // Hides cursor
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        MouseMovement();
    }

    private void FindParentTransform()
    {
        // Gets a list of all possible parent transforms and compares the
        // tag to see if it matches the tag we're looking for (default is
        // the player tag)
        Transform[] temp = GetComponentsInParent<Transform>();
        foreach (Transform transform in temp)
        {
            if (transform.CompareTag(objectTag))
            {
                objectBody = transform;
                break;
            }
        }

        // debug message
        if (!objectBody) Debug.LogError("Object body not found. "
                                      + "Perhaps you didn't set the parent's "
                                      + "tag for the camera script?");
    }

    private void MouseMovement()
    {
        float horizontal = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate about the Y-axis to look left or right,
        // rotate about the X-axis to look up or down
        objectBody.Rotate(Vector3.up * horizontal);

        // We are rotating the actual object outside of the camera to look
        // left/right while we are rotating the camera's transform itself
        // to look up/down.

        // Clamp value between -90 and 90 to lock camera from flipping upside
        // down
        xRotate -= vertical;
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotate, 0f, 0f);
    }

    private void RunTests()
    {
        if (objectBody)
        {
            print("Parent transform found");
        } else
        {
            print("Parent transform not found");
        }
    }
}
