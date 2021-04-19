using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public GameObject player;
    private float distance;
    public GameObject target;

    public float angle = 120f;
    public float radius = 10f;

    const float timeToStop = 6f;
    float stopDelay;

    public int sightState = 0;

    [SerializeField] public LayerMask playerMask;

    public Vector3 FromVector
    {
        get
        {
            float leftAngle = -angle / 2;
            leftAngle += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0, Mathf.Cos(leftAngle * Mathf.Deg2Rad));
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCenterPosition = player.GetComponent<Collider>().bounds.center + (0.5f * player.GetComponent<Collider>().bounds.extents.y * Vector3.up);
        Vector3 directionVector = -(playerCenterPosition - (transform.position + Vector3.up * 0.8f)).normalized;
        distance = Vector3.Distance(transform.position, playerCenterPosition);
        //Debug.Log("Distance: " + distance);

        float dotProduct = Vector3.Dot(directionVector, transform.forward);
        Debug.DrawRay((transform.position + Vector3.up * 0.8f + Vector3.forward), -directionVector * radius, Color.magenta);
        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.up * 0.8f + Vector3.forward), -directionVector, out hit, radius, playerMask))
        {
            if (dotProduct < -0.5f && distance <= radius && (hit.transform.tag == "Player"))
            {
                if (target == null)
                {
                    target = player;
                    sightState = 2;
                    SendMessage("onTarget", SendMessageOptions.DontRequireReceiver);
                    //Debug.Log(dotProduct + ": IN FIELD OF VIEW. Distance: " + distance);
                }

            }
            else
            {
                if (target != null)
                {

                    target = null;
                    sightState = 1;
                    SendMessage("lostTarget", SendMessageOptions.DontRequireReceiver);
                    stopDelay = Time.time + timeToStop;

                    //Debug.Log(dotProduct + ": OUT OF SIGHT. Distance: " + distance);
                }
            }
        }
        if (Time.time > stopDelay && target == null)
        {
            if (sightState == 1)
            {
                SendMessage("offTarget", SendMessageOptions.DontRequireReceiver);
                sightState = 0;
            }
        }
    }
}
