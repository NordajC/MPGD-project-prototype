using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : BaseInteraction
{
    public GameObject DestroyedRock;
    public float maxHealth = 50;
    public float currentHealth;
    public StatsBar HPBar;
    public string animationName;

    public int neededId;

    public void Start()
    {
        if (DestroyedRock != null)
        {
            DestroyedRock.SetActive(false);
        }

        currentHealth = maxHealth;
        HPBar.maxValue = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (HPBar != null)
        {
            HPBar.updateStatBarValue(currentHealth);
        }

        if (currentHealth <= 0)
        {
            BreakRock();
        }
    }

    private void BreakRock()
    {
        transform.Find("MainRock").gameObject.SetActive(false);
        Destroy(HPBar.gameObject);

        if (DestroyedRock != null)
        {
            DestroyedRock.SetActive(true);
            foreach (Rigidbody rb in DestroyedRock.GetComponentsInChildren<Rigidbody>(true))
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(500f, transform.position, 1f);
            }
        }

        GameObject.FindWithTag("Player").GetComponent<Interaction>().removeItem(this);
        togglePlayerMovement(true);
        Destroy(this);
    }

    public override void onInteractPrimary()
    {
        PlayerInventory playerInventory = playerRef.GetComponent<PlayerInventory>();
        int? hasItem = playerInventory.getItemSlotIndex(neededId, false);

        if(hasItem != null)
        {
            Vector3 direction = transform.position - playerRef.transform.position;
            direction.Normalize();
            direction.y = 0f;
            StartCoroutine(SmoothRotateTo(playerRef, Quaternion.LookRotation(direction), 0.2f));
            
            interactionAnimation(animationName, "");
        }
    }

    public override void onInteractSecondary()
    {
    }

    public override void onInteractionEnd()
    {
    }


    private void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.CompareTag("Player"))
        {
            // If player enters triggers, then add this item to the interaction list.
            other.gameObject.GetComponent<Interaction>().addItem(this);
            HPBar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        if(other.gameObject.CompareTag("Player"))
        {
            // If player exits triggers, then remove this item to the interaction list.
            other.gameObject.GetComponent<Interaction>().removeItem(this);
            HPBar.gameObject.SetActive(false);
        }
    }
}
