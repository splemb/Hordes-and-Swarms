using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private const int maxHealth = 100;
    private int currentHealth = maxHealth;

    private bool receivingDamage = false;
    public TMPro.TextMeshProUGUI healthText;

    public AudioClip hurt;
    public AudioClip slap;

    public AudioSource audioSource;

    public float recoverDelay;

    float nextRecoverTime = 0f;

    public void ChangeHealth(int amount = 0)
    {
        if (!receivingDamage)
        {
            //if (amount < 0) receivingDamage = true;
            if (amount < 0) audioSource.PlayOneShot(hurt);
            if (amount < 0) audioSource.PlayOneShot(slap);
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            if (currentHealth <= 0) { currentHealth = 0; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
            nextRecoverTime = Time.time + recoverDelay;
        }
    }

    public bool CheckAtMax()
    {
        if (currentHealth == maxHealth) return true;
        else return false;
    }

    public void Update()
    {
        if (Time.time > nextRecoverTime) ChangeHealth(1);
        healthText.text = currentHealth.ToString();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Hazard") receivingDamage = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hazard") receivingDamage = false;
    }
}
