using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -80f;
    private const float Y_ANGLE_MAX = 80.0f;

    public Transform target;
    public float currentY = 0.0f;
    public float currentX = 0.0f;
    public float sensitivityY = 1.0f;
    public float sensitivityX = 1.0f;

    private void Start()
    {
        currentX = target.transform.rotation.eulerAngles.y;
        transform.parent = null;
    }

    private void Update()
    {
        currentY += Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 100;
        currentX += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime * 100;
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        if (target.GetComponent<PlayerController>().isCrouching)
        {
            transform.position = new Vector3(target.position.x, target.position.y + (0.8f), target.position.z);
        } else
        {
            transform.position = new Vector3(target.position.x, target.position.y + (1.8f), target.position.z);
        }
        transform.rotation = Quaternion.Euler(new Vector3(currentY, currentX, 0));
    }
}
