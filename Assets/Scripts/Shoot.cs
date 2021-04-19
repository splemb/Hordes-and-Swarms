using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject bullet;
    Vector3 spawnPosition;
    Quaternion spawnRotation;
    public int ammoCount = 8;
    const float rateOfFire = 0.2f;
    const float speed = 1000f;
    float fireDelay;
    public TMPro.TextMeshProUGUI ammoText;

    // Update is called once per frame
    void Update()
    {
        ammoText.text = ammoCount.ToString();
        if (Input.GetButtonDown("Fire1") && Time.time > fireDelay && ammoCount > 0)
        {
            GetComponentInChildren<Animator>().Play("bowShoot");
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            fireDelay = Time.time + rateOfFire;
            GameObject bulletInstance = Instantiate(bullet, spawnPosition, spawnRotation);
            bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            //Physics.IgnoreCollision(bulletInstance.GetComponent<Collider>(), GetComponentInChildren<Collider>());

            ammoCount--;
            
        }
    }
}
