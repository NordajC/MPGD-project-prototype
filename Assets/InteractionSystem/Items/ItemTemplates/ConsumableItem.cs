using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Inventory/Item/Consumable")]
public class ConsumableItem : ItemTemplate
{
    //Values for the health and hunger amount to restore
    public float restoreHealth;
    public float restoreHunger;
    public float restoreHydration;
    
    [Header("Sounds")]
    public AudioClip[] consumedSounds;

    public void Awake()
    {
        ItemType = Type.Consumable;
    }

    public override void onItemUsed()
    {
        bool canUse = false;

        Slider slider = GameObject.Find("DisableBar").GetComponent<Slider>();
        
        // Checks if the stats to increase are not full.
        PlayerAttributes playerAttributes = GameObject.FindWithTag("Player").GetComponent<PlayerAttributes>();
        if(playerAttributes.currentHealth != playerAttributes.maxHealth && restoreHealth != 0 && slider.value == 0)
        {
            playerAttributes.adjustHealth(restoreHealth, true);
            canUse = true;
        }
        if(playerAttributes.currentHunger != playerAttributes.maxHunger && restoreHunger != 0 && slider.value == 0)
        {
            playerAttributes.adjustHunger(restoreHunger, true);
            canUse = true;
        }
        if(playerAttributes.currentThirst != playerAttributes.maxThirst && restoreHydration != 0  && slider.value == 0)
        {
            playerAttributes.adjustThirst(restoreHydration, true);
            canUse = true;
        }

        if(canUse) 
        {
            // The item is only removed from the inventory if one or more of the stats were not full and it was increased.
            PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            playerInventory.RemoveFromInventory(playerInventory.interactSection.selectedInventorySlot);
            
            AudioSource audioSource = GameObject.Find("ConsumableAudio").GetComponent<AudioSource>();
            Manager.playRandomSound(ref audioSource, consumedSounds, 0.2f);

            GameObject.FindWithTag("InventoryScreen").GetComponent<Animator>().Play("Cooldown");
        }
    } 
}
