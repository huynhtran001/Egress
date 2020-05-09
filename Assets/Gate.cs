using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // For use with the big gate with 2 sliding doors
    [SerializeField] Transform itself;
    [SerializeField] Transform leftDoor;
    [SerializeField] Transform rightDoor;

    [Tooltip("Slides doors open. Takes in a multiplier of how much")]
    [SerializeField] float leftDoorOffset = 7f;
    [SerializeField] float rightDoorOffset = 7f;
    [Tooltip("How fast the door opens")]
    [Range(0, 1)]
    [SerializeField] float lerpModifier = 1f;
    
    // for lerping
    private float lerpPct = 0f;

    private Vector3 oldLeftPos;
    private Vector3 oldRightPos;
    private Vector3 newLeftPos;
    private Vector3 newRightPos;
    private bool stillLocked = true;

    private void Start()
    {
        CalculatePos();
    }

    private void Update()
    {
        if (stillLocked) return;
        ApplyOpening();
    }

    private void ApplyOpening()
    {
        if (lerpPct <= 1f) lerpPct = lerpPct + lerpModifier * Time.deltaTime;
        leftDoor.position = Vector3.Lerp(oldLeftPos, newLeftPos, lerpPct);
        rightDoor.position = Vector3.Lerp(oldRightPos, newRightPos, lerpPct);
    }

    void CalculatePos()
    {
        oldLeftPos = leftDoor.position;
        oldRightPos = rightDoor.position;
        newLeftPos = leftDoor.position + Vector3.back * leftDoorOffset;
        newRightPos = rightDoor.position + Vector3.forward * rightDoorOffset;
    }

    public void OpenGates()
    {
        stillLocked = false;
    }
}
