using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBullet : MonoBehaviour
{
    [SerializeField] public LayerMask mask;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 10)
        {
            RotateToTravelDirection();
            CheckHit();
            
        }
        else
        {
            Invoke("Cleanup", 4);
        }
    }

    void RotateToTravelDirection()
    {
        transform.LookAt(transform.position + rb.velocity);
    }

    void CheckHit()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        RaycastHit hitObject;
        int damage = 20;
        if (Physics.Raycast(transform.position, direction, out hitObject, 1f, mask))
        {
            Debug.DrawLine(transform.position, hitObject.point, Color.magenta);
            Debug.Log("Test: " + hitObject.distance.ToString());

            hitObject.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            //Physics.IgnoreCollision(GetComponent<Collider>(), hitObject.collider);
        }
    }

    void Cleanup()
    {
        Destroy(gameObject);
    }
}
