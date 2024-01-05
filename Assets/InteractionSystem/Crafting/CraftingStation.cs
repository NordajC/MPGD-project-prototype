using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : BaseInteraction
{
    public override void onInteractPrimary()
    {
        // Call function to toggle crafting.
        PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        playerInventory.OnToggleCrafting();
    }

    public override void onInteractSecondary()
    {
    }

    public override void onInteractionEnd()
    {
    }
}
