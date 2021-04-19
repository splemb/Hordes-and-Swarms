using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : MonoBehaviour
{
    [SerializeField] public LayerMask mask;
    int ammoCount = 16;
    public TMPro.TextMeshProUGUI ammoText;
    public AudioSource audioSource;
    public ParticleSystem emitter;

    public float fireDelay = 2f;
    float nextFire;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) nextFire = Time.time;

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            FireOneShot();
            nextFire = Time.time + fireDelay;
        }

        //ammoText.text = ammoCount.ToString();
    }

    void FireOneShot()
    {
        emitter.transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 10f);
        GetComponentInChildren<Animator>().Play("pistolShoot");
        audioSource.Play();
        //ammoCount--;
        //ammoText.text = ammoCount.ToString();

        Vector3 direction = Camera.main.transform.forward;
        RaycastHit hitObject;
        int damage = 10;
        if (Physics.Raycast(Camera.main.transform.position, direction, out hitObject, 20f, mask))
        {


            if (hitObject.collider.GetType() == typeof(CapsuleCollider))
            {
                emitter.transform.LookAt(hitObject.point);
                Debug.DrawLine(Camera.main.transform.position, hitObject.point, Color.magenta);
                Debug.Log("Test: " + hitObject.distance.ToString());
                hitObject.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        emitter.Play();
    }
}
