using UnityEngine;

public class Gate : MonoBehaviour
{
    // Audio
    [SerializeField] AudioSource audioSource;

    // For use with the big gate with 2 sliding doors
    [SerializeField] Transform leftDoor;
    [SerializeField] Transform rightDoor;

    [Tooltip("Slides doors open. Takes in a multiplier of how much")]
    [SerializeField] float leftDoorOffset = 1.25f;
    [SerializeField] float rightDoorOffset = 1.25f;
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
        leftDoor.localPosition = Vector3.Lerp(oldLeftPos, newLeftPos, lerpPct);
        rightDoor.localPosition = Vector3.Lerp(oldRightPos, newRightPos, lerpPct);
    }

    void CalculatePos()
    {
        oldLeftPos = leftDoor.localPosition;
        oldRightPos = rightDoor.localPosition;
        newLeftPos = leftDoor.localPosition + Vector3.forward * leftDoorOffset;
        newRightPos = rightDoor.localPosition + Vector3.back * rightDoorOffset;
    }

    public void OpenGates()
    {
        audioSource.Play();
        stillLocked = false;
    }
}
