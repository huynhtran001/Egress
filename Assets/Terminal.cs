using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    // Position of the game object that the script is attached to
    // Intended use with terminals
    [SerializeField] GameObject terminal;

    [Tooltip("Shape of the detection range (Turn on gizmos)")]
    [SerializeField] Vector3 shape = new Vector3(3f, 1f, 2f);
    [Tooltip("Offset of the detection range (Turn on gizmos)")]
    [SerializeField] Vector3 offsetFromObject = new Vector3(0, 1f, 2.5f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Vector3 targetPosition = terminal.transform.position + offsetFromObject;

        // Note: layermask is just bit shifting: layer 1 = 1 << 1, layer 2 = 1 << 2, etc.
        // player mask is layer 8
        int layerMask = 1 << 8;

        // Ignores "default" layer mask
        Collider[] colliders = Physics.OverlapBox(targetPosition, shape, Quaternion.identity, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // sends raycast where ever the player is looking only when they're standing
                // within the overlap range.
                Player player = collider.GetComponent<Player>();
                player.WithinRange(this.gameObject.tag);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(terminal.transform.position + offsetFromObject, shape);
        //Gizmos.DrawCube(terminal.transform.position + offsetFromObject, shape);
    }
}
