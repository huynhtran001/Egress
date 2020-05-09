using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDoor : MonoBehaviour
{
    [SerializeField] Transform door;

    [Tooltip("Slides doors open. Takes in a multiplier of how much")]
    [SerializeField] float doorOffset = 1.25f;
    [Tooltip("How fast the door opens")]
    [Range(0, 1)]
    [SerializeField] float lerpModifier = 1f;

    private Vector3 oldPos;
    private Vector3 newPos;
    private float lerpPct = 0f;

    public bool stillLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        CalculatePos();
    }


    // Update is called once per frame
    void Update()
    {
        if (stillLocked) return;
        ApplyOpening();
    }

    private void ApplyOpening()
    {
        if (lerpPct <= 1f) lerpPct = lerpPct + lerpModifier * Time.deltaTime;
        door.localPosition = Vector3.Lerp(oldPos, newPos, lerpPct);
    }

    private void CalculatePos()
    {
        oldPos = door.localPosition;
        newPos = door.localPosition + Vector3.up * doorOffset;
    }

    public void OpenDoor()
    {
        stillLocked = false;
    }
}
