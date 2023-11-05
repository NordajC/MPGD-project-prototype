using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HPBar healthBar;
    public int maxHunger = 100;
    public int currentHunger;
    public HungerBar hungerBar;

    public int maxThirst = 100;
    public int currentThirst;
    public WaterBar waterBar;


    public float hungerDecreaseTime = 15f; // Time in seconds to decrease hunger
    public int hungerDecreaseAmount = 1; // Amount of hunger to decrease
    public float thirstDecreaseTime = 10f; // Time in seconds to decrease thirst
    public int thirstDecreaseAmount = 1; // Amount of thirst to decrease

    public float starvationDamageTime = 10f; // Time in seconds to apply damage when starving
    public int starvationDamageAmount = 2; // Amount of damage to apply

    private Vector3 lastPosition;

    private void Start()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        lastPosition = transform.position;

        StartCoroutine(DecreaseHunger());
        StartCoroutine(DecreaseThirst());
        StartCoroutine(ApplyStarvationDamage());
    }

    private IEnumerator DecreaseHunger()
    {
        while (true)
        {
            yield return new WaitForSeconds(hungerDecreaseTime);
            AdjustHunger(transform.position != lastPosition ? hungerDecreaseAmount * 2 : hungerDecreaseAmount);
        }
    }

    private IEnumerator DecreaseThirst()
    {
        while (true)
        {
            yield return new WaitForSeconds(thirstDecreaseTime);
            AdjustThirst(transform.position != lastPosition ? thirstDecreaseAmount * 2 : thirstDecreaseAmount);
        }
    }

    private IEnumerator ApplyStarvationDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(starvationDamageTime);
            if (currentHunger <= 0 || currentThirst <= 0)
            {
                TakeDamage(starvationDamageAmount);
            }
        }
    }


    private void AdjustHunger(int amount)
    {
        currentHunger -= amount;
        currentHunger = Mathf.Max(currentHunger, 0);
        hungerBar.SetHung(currentHunger);
    }

    private void AdjustThirst(int amount)
    {
        currentThirst -= amount;
        currentThirst = Mathf.Max(currentThirst, 0);
        waterBar.SetWater(currentThirst);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBar.SetHP(currentHealth);

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