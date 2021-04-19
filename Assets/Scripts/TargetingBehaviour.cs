using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingBehaviour : MonoBehaviour
{
    public MeshRenderer visorMesh;
    public Material blueVisor;
    public Material orangeVisor;
    public Material redVisor;

    public AudioSource audioSource;
    public AudioClip[] sounds;

    public void offTarget()
    {
        visorMesh.material = blueVisor;
        audioSource.clip = sounds[0];
        audioSource.Stop();
    }

    public void lostTarget()
    {
        visorMesh.material = orangeVisor;
        audioSource.clip = sounds[2];
        audioSource.Play();
    }

    public void onTarget()
    {
        visorMesh.material = redVisor;
        audioSource.clip = sounds[1];
        audioSource.Play();
    }
}
