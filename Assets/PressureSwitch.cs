using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    [SerializeField] Gate gateToOpen;

    [Tooltip("Shape of the detection range (Turn on gizmos)")]
    [SerializeField] Vector3 shape = new Vector3(3f, 1f, 2f);
    [Tooltip("Offset of the detection range (Turn on gizmos)")]
    [SerializeField] Vector3 offsetFromObject = new Vector3(0, 1f, 2.5f);

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Vector3 targetPosition = this.transform.position + offsetFromObject;

        // Note: layermask is just bit shifting: layer 1 = 1 << 1, layer 2 = 1 << 2, etc.
        // player mask is layer 8
        int layerMask = 1 << 8;

        // Ignores "default" layer mask
        Collider[] colliders = Physics.OverlapBox(targetPosition, shape, Quaternion.identity, layerMask);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                gateToOpen.OpenGates();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + offsetFromObject, shape);
    }
}
