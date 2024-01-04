using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : BaseInteraction
{
    [Header("Pickup item")]
    // Variables used to select the target item in the prefab variant.
    public ItemTemplate item;
    public GameObject inspectScreen;
    public float inspectFOV = 12;

    public override void onInteractPrimary()
    {
        PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        Interaction interaction = GameObject.FindWithTag("Player").GetComponent<Interaction>();
        
        // Item only picked up if slot available.
        int? hasFreeSlot = playerInventory.getAvailableSlotIndex();
        if(hasFreeSlot != null && (playerInventory.currentScreen == CurrentScreen.None || playerInventory.currentScreen == CurrentScreen.Inspect))
        {
            playerInventory.AddToInventory(item);
            interaction.removeItem(this);
            Destroy(gameObject);
        }
    }

    public override void onInteractSecondary()
    {
        PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();

        // Initiate inspect item.
        if(playerInventory.currentScreen == CurrentScreen.None)
        {
            var inspect = Instantiate(inspectScreen);
            inspect.GetComponent<InspectItem>().initialise(item, inspectFOV);

            playerInventory.currentScreen = CurrentScreen.Inspect;
            playerInventory.disableInput(true, true);
        }
    }
}
