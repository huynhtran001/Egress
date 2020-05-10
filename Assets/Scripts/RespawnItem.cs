using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnItem : MonoBehaviour
{
    private Vector3 position;
    private Quaternion quaternion;
    // Start is called before the first frame update
    void Start()
    {
        position = this.transform.position;
        quaternion = this.transform.rotation;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Deathbox"))
        {
            Instantiate(gameObject, position, quaternion);
            Destroy(gameObject, 1f);
        }
    }
}
