using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public GameObject target;
    private Vector3 directionToTarget;
    private float angleToTarget;

    private Quaternion initialRotation;

    

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            //directionToTarget = new Vector3(target.transform.position.x - transform.position.x, 0f, target.transform.position.z - transform.position.z);
            directionToTarget = target.transform.position - transform.position;
            angleToTarget = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            //Debug.Log(directionToTarget + ". Angle: " + angleToTarget);

            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, angleToTarget, transform.eulerAngles.z);

            //Quaternion newRotation = Quaternion.LookRotation(directionToTarget); //Calculates rotation towards the movement direction
            //Quaternion newRotation = Quaternion.AngleAxis(angleToTarget, Vector3.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 3f); //Rotates the model smoothly
        } 
    }
}
