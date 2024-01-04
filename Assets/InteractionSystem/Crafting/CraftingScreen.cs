using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

// A class used to keep track of the items to remove and quantity of it to remove for crafting. Set when a crafting item is clicked.
[System.Serializable]
public class itemsToRemove
{
    public int itemToRemove;
    public int quantityToRemove;
}

public class CraftingScreen : MonoBehaviour
{
    [Header("Inventory")]
    private PlayerInventory playerInventory;
    public GameObject InventoryPanel;
    public GameObject InventorySlots;

    [Header("Crafting tabs")]
    public GameObject toolsPanel;
    public Image toolsButton;
    public GameObject weaponsPanel;
    public Image weaponsButton;
    public GameObject armourPanel;
    public Image armourButton;
    public Color defaultTint;

    [Header("Crafting")]
    public GameObject craftingInteractionPanel;
    [HideInInspector] public ItemTemplate itemToCraft;
    [HideInInspector] public SetCraftableInfo targetButton;
    [HideInInspector] public List<itemsToRemove> toRemoveCrafting = new List<itemsToRemove>();

    [Header("Item info")]
    public TextMeshProUGUI itemNameText;
    public Image itemIconTexture;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI craftRequirementsText;
    public Button craftButton;
    
    public void setInteractionPanelVisibility(bool visible)
    {
        // Hide or show the interaction panel based on if an item was clicked.
        craftingInteractionPanel.GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
        craftingInteractionPanel.GetComponent<CanvasGroup>().interactable = visible ? true : false;
    }
    
    public void setInventorySlots()
    {
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();

        // Set the height of the backpack in the crafting screen by setting it to the same as the one in inventory screen where the calculation is already done.
        var panelHeight = playerInventory.InventoryPanel.GetComponent<RectTransform>().sizeDelta.y;
        InventoryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(315, panelHeight);

        // When called, the already existing slots in the backpack are destroyed so it is updated based on the last crafted item.
        foreach(Transform slot in InventorySlots.transform)
        {
            if(slot.CompareTag("InventorySlot"))
                Destroy(slot.gameObject);
        }

        // Adds all the slots of current items.
        foreach(Transform slot in playerInventory.InventorySlots.transform)
        {
            if(slot.CompareTag("InventorySlot"))
                Instantiate(slot, InventorySlots.transform);
        }
    }

    void Awake()
    {
        setInteractionPanelVisibility(false);
        setInventorySlots();
    }

    public void Start()
    {
        setTools(); // By default, tools panel is shown
    }

    // Functions called when the tab buttons are pressed to show the corresponding tab and hide the others.
    public void setTools()
    {
        setButtonState(ref toolsPanel, ref toolsButton);
    }

    public void setWeapons()
    {
        setButtonState(ref weaponsPanel, ref weaponsButton);
    }

    public void setArmour()
    {
        setButtonState(ref armourPanel, ref armourButton);
    }

    public void setButtonState(ref GameObject panel, ref Image btnImage)
    {
        // To set visual state of current panel and button.
        toolsPanel.SetActive(false);
        weaponsPanel.SetActive(false);
        armourPanel.SetActive(false);
        panel.SetActive(true); // Only set target panel to active.

        toolsButton.color = defaultTint;
        weaponsButton.color = defaultTint;
        armourButton.color = defaultTint;
        btnImage.color = new Color(1, 1, 1, 1); // Only set target button colour.

        setInteractionPanelVisibility(false);
    }

    public void craftItem()
    {
        // Craft item called when craft button pressed. Calculation to check if there are enough items to craft done when item is clicked (button enabled or disabled)
        foreach(var item in toRemoveCrafting)
        {
            // Gets every item index in the items to remove list and the quantity of it, then subtracts the quantity from the current one.
            playerInventory.playerInventory[item.itemToRemove].itemAmount -= item.quantityToRemove;
        }

        playerInventory.removeEmptyItems();
        playerInventory.AddToInventory(itemToCraft); // Adds the item that was crafted to the inventory.
        setInventorySlots();
        toRemoveCrafting = new List<itemsToRemove>(); // Resets the variable
        targetButton.onItemClicked(); // Calls the function again on the button as it needs to update the requirements and button state if the item can be crafted or not.
    }

    public void closeCraftScreen()
    {
        // Sets current screen to none, enables input and destroys the craft screen.
        PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        playerInventory.currentScreen = CurrentScreen.None;
        playerInventory.disableInput(false, false);
        Destroy(gameObject);
    }
}
