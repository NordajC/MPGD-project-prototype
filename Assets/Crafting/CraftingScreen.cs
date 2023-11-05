using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class used to keep track of the items to remove and quantity of it to remove for crafting. Set when a crafting item is clicked.
[System.Serializable]
public class itemsToRemove
{
    public int itemToRemove;
    public int quantityToRemove;
}

public class CraftingScreen : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject InventoryPanel;

    public ItemTemplate emptyItemTemplate;

    [Header("Craftin tabs")]
    public GameObject toolsPanel;
    public GameObject weaponsPanel;
    public GameObject armourPanel;

    [HideInInspector] public ItemTemplate itemToCraft;

    private PlayerInventory playerInventory;

    [HideInInspector] public SetCraftableInfo targetButton;

    [HideInInspector] public List<itemsToRemove> toRemoveCrafting = new List<itemsToRemove>();

    private GameObject craftingInteractionPanel;

    public void setInteractionPanelVisibility(bool visible)
    {
        //Hide or show the interaction panel based on if an item was clicked.
        craftingInteractionPanel.GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
        craftingInteractionPanel.GetComponent<CanvasGroup>().interactable = visible ? true : false;
    }
    
    void Awake()
    {
        setInventorySlots();
        craftingInteractionPanel = GameObject.Find("CraftSection");
        setInteractionPanelVisibility(false);
    }

    public void setInventorySlots()
    {
        //Same code as player inventory but modified to fit the new backpack size. (Does not need extra width for the interaction as this is only for showing player their current backpack)
        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();
        int InventoryAmount = playerInventory.InventoryAmount;
        int inventoryRowSize = playerInventory.inventoryRowSize;
        int inventoryColumnSize = playerInventory.inventoryColumnSize;

        //Dynamically sets the inventory panel sizes based on the size of the inventory.
        InventoryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2((inventoryRowSize*60)+(inventoryRowSize*15),(inventoryColumnSize*60)+(inventoryColumnSize*15)+50);
        Vector2 startPos = GameObject.Find("InventoryText").GetComponent<RectTransform>().localPosition;
        float panelWidth = InventoryPanel.GetComponent<RectTransform>().sizeDelta.x;

        int count = 0;
        
        //When called, the already existing slots in the backpack are destroyed so it is updated based on the last crafted item.
        foreach(Transform slot in InventoryPanel.transform)
        {
            if(slot.CompareTag("InventorySlot"))
            {
                Destroy(slot.gameObject);
            }
        }

        //Initialises each inventory slot UI item in the inventory panel by adding all the rows, then the columns. Tested values are used to set the position of them to be correct in the UI panel.
        for(int j = 0; j < inventoryColumnSize; j++)
        {
            for(int i = 0; i < inventoryRowSize; i++)
            {
                if(playerInventory.playerInventory[count].itemTemplate.ItemId > 0)
                {
                    GameObject spawnItemSlot = GameObject.Find("InventoryPanel").transform.GetChild(count + 1).gameObject;
                    var spawnedTemplate = Instantiate(spawnItemSlot, InventoryPanel.transform);
                    spawnedTemplate.GetComponent<RectTransform>().localPosition = new Vector2(-panelWidth + 40 + (i*75), startPos.y - 70 + (j * -75));
                }

                count++;
            }
        }
    }

    public void Start()
    {
        //By default, tools panel is shown
        toolsPanel.SetActive(true);
        weaponsPanel.SetActive(false);
        armourPanel.SetActive(false);
    }

    //Functions called when the tab buttons are pressed to show the corresponding tab and hide the others.
    public void setTools()
    {
        toolsPanel.SetActive(true);
        weaponsPanel.SetActive(false);
        armourPanel.SetActive(false);
        setInteractionPanelVisibility(false);
    }

    public void setWeapons()
    {
        toolsPanel.SetActive(false);
        weaponsPanel.SetActive(true);
        armourPanel.SetActive(false);
        setInteractionPanelVisibility(false);
    }

    public void setArmour()
    {
        toolsPanel.SetActive(false);
        weaponsPanel.SetActive(false);
        armourPanel.SetActive(true);
        setInteractionPanelVisibility(false);
    }

    public void craftItem()
    {
        //Craft item called when craft button pressed. Calculation to check if there are enough items to craft done when item is clicked (button enabled or disabled)
        foreach(var item in toRemoveCrafting)
        {
            //Gets every item index in the items to remove list and the quantity of it, then subtracts the quantity from the current one.
            playerInventory.playerInventory[item.itemToRemove].itemAmount -= item.quantityToRemove;
        }

        playerInventory.removeEmptyItems();
        playerInventory.AddToInventory(itemToCraft); //Adds the item that was crafted to the inventory.
        setInventorySlots();
        toRemoveCrafting = new List<itemsToRemove>(); //Resets the variable
        targetButton.onItemClicked(); //Calls the function again on the button as it needs to update the requirements and button state if the item can be crafted or not.
    }

    public void closeCraftScreen()
    {
        //Sets curretn screen to none, enables input and destroys the craft screen.
        GameObject.Find("Player").GetComponent<PlayerInventory>().currentScreen = CurrentScreen.None;
        GameObject.Find("Player").GetComponent<PlayerInventory>().disableInput(false);
        Destroy(gameObject);
    }
}
