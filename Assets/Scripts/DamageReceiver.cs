using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public float hitPoints = 30f;

    void ApplyDamage(float damage)
    {
        if (hitPoints < 0f)
        {
            return;
        }

        hitPoints -= damage;

        if (hitPoints <= 0f)
        {
            Invoke("SelfTerminate", 0);
        }
    }

    void SelfTerminate()
    {
        Destroy(gameObject);
    }
}
