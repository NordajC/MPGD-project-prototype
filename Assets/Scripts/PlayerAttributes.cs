using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Implement death logic (e.g., destroy object, play animation)
        Destroy(gameObject);
    }
}
