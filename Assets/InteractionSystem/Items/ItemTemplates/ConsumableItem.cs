using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Inventory/Item/Consumable")]
public class ConsumableItem : ItemTemplate
{
    //Values for the health and hunger amount to restore
    public float restoreHealth;
    public float restoreHunger;
    public float restoreHydration;

    public void Awake()
    {
        ItemType = Type.Consumable;
    }

    public override void onItemUsed()
    {
        bool canUse = false;

        // Checks if the stats to increase are not full.
        PlayerAttributes playerAttributes = GameObject.FindWithTag("Player").GetComponent<PlayerAttributes>();
        if(playerAttributes.currentHealth != playerAttributes.maxHealth && restoreHealth != 0)
        {
            playerAttributes.adjustHealth(restoreHealth, true);
            canUse = true;
        }
        if(playerAttributes.currentHunger != playerAttributes.maxHunger && restoreHunger != 0)
        {
            playerAttributes.adjustHunger(restoreHunger, true);
            canUse = true;
        }
        if(playerAttributes.currentThirst != playerAttributes.maxThirst && restoreHydration != 0)
        {
            playerAttributes.adjustThirst(restoreHydration, true);
            canUse = true;
        }

        if(canUse) 
        {
            // The item is only removed from the inventory if one or more of the stats were not full and it was increased.
            PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            playerInventory.RemoveFromInventory(playerInventory.interactSection.selectedInventorySlot);
        }
    } 
}
