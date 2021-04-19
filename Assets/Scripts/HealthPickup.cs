using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PlayerHealth>().CheckAtMax())
            {
                other.GetComponent<PlayerHealth>().ChangeHealth(10);
                Destroy(gameObject);
            }
        }
    }
}
