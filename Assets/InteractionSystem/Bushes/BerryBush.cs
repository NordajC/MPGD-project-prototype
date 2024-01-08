using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : BaseInteraction
{
    public ItemTemplate berryItem;
    public int noOfBerries;
    public GameObject[] berryObjects;
    public Transform moveToPosition;

    public override void onInteractPrimary()
    {
        moveAndRotateTo(moveToPosition, -1);
        interactionAnimation("Pick Fruit", "collecting berries");
    }

    public override void onInteractSecondary()
    {
    }

    public override void onInteractionEnd()
    {
        PlayerInventory playerInventory = playerRef.GetComponent<PlayerInventory>();

        // Add berries to player inventory.
        for(int i = 0; i < noOfBerries; i++)
        {
            playerInventory.AddToInventory(berryItem);
            Destroy(berryObjects[i]);
        }
        
        GameObject.FindWithTag("Player").GetComponent<Interaction>().removeItem(this);
        togglePlayerMovement(true);
        Destroy(this);
    }
}
