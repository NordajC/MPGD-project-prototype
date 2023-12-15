using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerAttributes : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    [HideInInspector] public float currentHealth;
    public StatsBar healthBar;
    public GameObject screenDamage;

    [Header("Hunger")]
    public float maxHunger = 100;
    [HideInInspector] public float currentHunger;
    public StatsBar hungerBar;
    public float hungerDecreaseTime = 15f;
    public int hungerDecreaseAmount = 1;
    public float starvationDamageTime = 10f;
    public int starvationDamageAmount = 2;
        
    [Header("Thirst")]
    public float maxThirst = 100;
    [HideInInspector] public float currentThirst;
    public StatsBar waterBar;
    public float thirstDecreaseTime = 10f;
    public int thirstDecreaseAmount = 1;

    [Header("Other")]
    private Vector3 lastPosition;

    private void Start()
    {
        // Setting default values.
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;

        currentHunger = maxHunger;
        hungerBar.maxValue = maxHunger;

        currentThirst = maxThirst;
        waterBar.maxValue = maxThirst;

        lastPosition = transform.position;

        // If hunger not finished, then call functions to decrease hunger and check for starvation damage.
        if(hungerDecreaseAmount > 0)
        {
            StartCoroutine(DecreaseHunger());
            StartCoroutine(ApplyStarvationDamage());
        }

        // If thirst not finished, then call functions to decrease thirst.
        if(thirstDecreaseAmount > 0)
            StartCoroutine(DecreaseThirst());
            StartCoroutine(ApplyStarvationDamage());
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    
    private void adjustStat(ref float stat, ref float maxStat, ref StatsBar statBar, float amount, bool regen)
    {
        float original = stat; // Original value taken so it can be compared with modified.
        stat = regen ? stat += amount : stat -= amount; // Either subtract or add based on if regenerated or not.
        stat = Mathf.Clamp(stat, 0, maxStat); // Clamp so the stat can not exceed the min and max value.
        stat = Mathf.Max(stat, 0);
        // Only need to update bar if any change was made.
        if(original != stat)
            statBar.updateStatBar(stat, regen);
    }
 
    public void adjustHealth(float amount, bool regen)
    {
        adjustStat(ref currentHealth, ref maxHealth, ref healthBar, amount, regen);
        
        if(!regen)
        {
            Instantiate(screenDamage, GameObject.Find("ScreenDamage").transform);
        } else {
            // Add healing effect.
        }

        // Call death function if no more health.
        if (currentHealth <= 0)
            Die();
    }
    
    public void adjustHunger(float amount, bool regen)
    {
        // Call adjust stat with variabled set to hunger ones.
        adjustStat(ref currentHunger, ref maxHunger, ref hungerBar, amount, regen);
    }

    public void adjustThirst(float amount, bool regen)
    {
        // Call adjust stat with variabled set to thirst ones.
        adjustStat(ref currentThirst, ref maxThirst, ref waterBar, amount, regen);
    }
    
    private IEnumerator DecreaseHunger()
    {
        while (true)
        {
            yield return new WaitForSeconds(hungerDecreaseTime);
            // Hunger depleted faster if moving.
            adjustHunger(transform.position != lastPosition ? hungerDecreaseAmount * 2 : hungerDecreaseAmount, false);
        }
    }

    private IEnumerator DecreaseThirst()
    {
        while (true)
        {
            yield return new WaitForSeconds(thirstDecreaseTime);
            // Thirst depleted faster if moving.
            adjustThirst(transform.position != lastPosition ? thirstDecreaseAmount * 2 : thirstDecreaseAmount, false);
        }
    }

    private IEnumerator ApplyStarvationDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(starvationDamageTime);
            // Add damage if hunger or thirst completely depleted.
            if (currentHunger <= 0 || currentThirst <= 0)
                adjustHealth(starvationDamageAmount, false);
        }
    }
}