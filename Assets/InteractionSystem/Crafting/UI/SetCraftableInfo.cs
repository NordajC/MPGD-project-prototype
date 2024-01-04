using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SetCraftableInfo : MonoBehaviour
{
    [Header("References")]
    private CraftingScreen craftingScreen;
    private ItemTemplate itemTemplate;    
    
    [Header("Item data")]
    public RawImage backgroundImage;
    public Image iconImage;

    public void Awake()
    {
        craftingScreen = GameObject.FindWithTag("CraftScreen").GetComponent<CraftingScreen>(); // Set crafting screen reference.
    }
    
    // Function called when initialising the buttons. Sets the item template and visuals of it.
    public void setInfo(ItemTemplate item)
    {   
        itemTemplate = item;
        var typeColour = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().getItemTypeColour(item.ItemType);
        backgroundImage.color = typeColour;
        iconImage.sprite = item.ItemIcon;
    }

    public void onItemClicked()
    {
        craftingScreen.setInteractionPanelVisibility(true); // If an item was clicked, the craft screen interaction panel should show.

        // Sets the information in the interaction panel
        craftingScreen.itemNameText.text = itemTemplate.ItemName;
        craftingScreen.itemDescriptionText.text = itemTemplate.ItemDescription;
        craftingScreen.itemIconTexture.sprite = itemTemplate.ItemIcon;

        List<InventoryItem> playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().playerInventory;
        int inventorySize = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().InventoryAmount - 1;
        string requirementsText = ""; // Empty string so the requirements can be appended accordingly.

        bool hasMissingItem = false; // By default, no missing items.

        var toRemove = craftingScreen.toRemoveCrafting; // References the items to remove.

        foreach(CraftableItem craftItem in itemTemplate.craftingRequirements) // Checks every item requirement for the selected item
        {
            int checkedAmount = 0;

            for(int i = 0; i < playerInventory.Count; i++)
            {
                // Checks that the item to compare with is occupied and it is the same item as the one required (by checking if the id of them matches).
                if(playerInventory[inventorySize - i].itemTemplate.ItemId > 0 && playerInventory[inventorySize - i].itemTemplate.ItemId == craftItem.item.ItemId)
                {
                    if(craftItem.item.IsStackable) // If stackable, then needs to check if enough of that item is there. If not then multiple stacks must be checked.
                    {
                        if(playerInventory[inventorySize - i].itemAmount >= craftItem.requiredAmount) // Might need to change when checking across multiple stacks
                        {
                            // Has enough of that item in the checked stack.
                            checkedAmount += craftItem.requiredAmount;

                            // Adds the item index and quantity to the items to remove list
                            var itemTemp = new itemsToRemove();
                            itemTemp.itemToRemove = inventorySize - i;
                            itemTemp.quantityToRemove = craftItem.requiredAmount;
                            toRemove.Add(itemTemp);

                            break;
                        } else {
                            // Checks multiple stacks.
                            if(checkedAmount < craftItem.requiredAmount)
                            {
                                checkedAmount += playerInventory[inventorySize - i].itemAmount; // Adds the amount of the current slot.

                                var itemTemp = new itemsToRemove();
                                itemTemp.itemToRemove = inventorySize - i;

                                // If the current slot has more than the amount left to craft, then the quantity to remove is set to the amount left and not the total amount in that slot
                                bool checkRemainder = checkedAmount > craftItem.requiredAmount; 
                                itemTemp.quantityToRemove = checkRemainder ? checkedAmount - craftItem.requiredAmount : playerInventory[inventorySize - i].itemAmount;
                                toRemove.Add(itemTemp);

                                if(checkedAmount >= craftItem.requiredAmount)
                                    break; // If the requirement has been met, then stop checking further stacks.
                            }
                        }
                    } else {
                        // If not stackable.
                        checkedAmount += playerInventory[inventorySize - i].itemAmount;
                        if(checkedAmount >= craftItem.requiredAmount)
                        {
                            var itemTemp = new itemsToRemove();
                            itemTemp.itemToRemove = inventorySize - i;
                            itemTemp.quantityToRemove = craftItem.requiredAmount;
                            toRemove.Add(itemTemp);

                            break;
                        }
                    }
                } 
            }

            // After checking the whole inventory, adds the item requirement text to the crafting interaction panel. If met, then the text is green, otherwise red.
            if(checkedAmount >= craftItem.requiredAmount)
            {
                requirementsText += "<color=green>" + craftItem.item.ItemName + " x" + craftItem.requiredAmount + "\n";
                craftingScreen.itemToCraft = itemTemplate;
                craftingScreen.targetButton = GetComponent<SetCraftableInfo>(); // Sets the target button in crafting screen if can craft.
            } else {
                requirementsText += "<color=red>" + craftItem.item.ItemName + " x" + craftItem.requiredAmount + "\n";
                craftingScreen.targetButton = null; // If can't craft, set target button to null.
                hasMissingItem = true; // If even one of the required item and/or quantity is not met, then has missing item is set to true and the item can't be crafted.
            }
        }

        // Debugging
        /*
        foreach(var test in toRemove)
        {
            Debug.Log("Index: " + test.itemToRemove + ", Amount: " + test.quantityToRemove);
        }
        */

        craftingScreen.craftRequirementsText.text = requirementsText; // Sets the text in the UI.
        
        craftingScreen.craftButton.interactable = !hasMissingItem; // If has missing item, then the craft button is disabled.

        // If has missing item, then the items to remove list is set to an empty one, otherwise the items to remove values are set.
        craftingScreen.toRemoveCrafting = hasMissingItem ? new List<itemsToRemove>() : toRemove;
    }
}
