using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area_Loader : MonoBehaviour
{
    public GameObject area;

    private void Start()
    {
        area.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") area.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") area.SetActive(false);
    }

}
