using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using Cinemachine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject InventoryPanel;
    [Range(1,5)] public int inventoryRowSize;
    [Range(1,5)] public int inventoryColumnSize;
    public GameObject ItemUITemplatePrefab;
    private bool inventoryOpen;

    [HideInInspector] public bool isDragDrop = false;

    [Header("Inventory List")]
    [HideInInspector] public List<InventoryItem> playerInventory = new List<InventoryItem>();
    private int InventoryAmount;
    public ItemTemplate emptyItemTemplate;
    [HideInInspector] public int DropFromSlot;

    [Header("Inventory interaction")]
    public ItemInteraction interactSection;

    void Awake()
    {
        InventoryAmount = inventoryRowSize * inventoryColumnSize; //Calculates the inventory size by multiplying row and column size.

        //Dynamically sets the inventory panel sizes based on the size of the inventory.
        InventoryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2((inventoryRowSize*60)+(inventoryRowSize*15)+200,(inventoryColumnSize*60)+(inventoryColumnSize*15)+50);
        Vector2 startPos = GameObject.Find("InventoryText").GetComponent<RectTransform>().localPosition;
        float panelWidth = InventoryPanel.GetComponent<RectTransform>().sizeDelta.x;

        //Initialises each inventory slot UI item in the inventory panel by adding all the rows, then the columns. Tested values are used to set the position of them to be correct in the UI panel.
        for(int j = 0; j < inventoryColumnSize; j++)
        {
            for(int i = 0; i < inventoryRowSize; i++)
            {
                var spawnedTemplate = Instantiate(ItemUITemplatePrefab, InventoryPanel.transform);
                spawnedTemplate.GetComponent<RectTransform>().localPosition = new Vector2(-panelWidth + 40 + (i*75), startPos.y - 70 + (j * -75));
                var emptyItem = new InventoryItem();
                emptyItem.itemTemplate = emptyItemTemplate;
                playerInventory.Add(emptyItem); //By default each item is set to an empty item as they don't have any.
            }
        }
    }

    public void setInventorySlots()
    {
        //Function that is called when the inventory is updated so that the visuals of it are updated as well.
        for(int i = 0; i < InventoryAmount; i++)
        {
            setSlotVisuals(i,playerInventory[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setInventorySlots();
    }

    // Update is called once per frame
    void Update()
    {
        //For debugging using text
        /*
        string debugText = "";
        TextMeshProUGUI textRef = GameObject.Find("DebugInventoryText").GetComponent<TextMeshProUGUI>();
        foreach(InventoryItem item in playerInventory)
        {
            debugText += "\n" + "   " + item.itemTemplate.ItemName + "| " + item.itemAmount + "|< " + item.itemTemplate.ItemId + ">";
        }
        textRef.text = debugText;
        */

        //Player can toggle the inventory using the E key
        if(Input.GetKeyDown(KeyCode.E))
        {
            //To toggle the inventory open, simply get the not operator of the current value.
            inventoryOpen = !inventoryOpen;

            //If inventory open or closed, then it is shown or hidden by setting the canvas group settings.
            CanvasGroup playerUI = GameObject.Find("PlayerInventoryUI").GetComponent<CanvasGroup>();
            playerUI.alpha = inventoryOpen ? 1 : 0;
            playerUI.interactable = inventoryOpen;

            //If inventory open, then the cursor should be visible and unlocked so it can be moved.
            Cursor.visible = inventoryOpen;
            Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;

            //To disable camera movement while inventory open the x and y speed is changed to 0 if open, and back to their default values if closed.
            CinemachineFreeLook gameCamera = GameObject.Find("ThirdPersonCam").GetComponent<CinemachineFreeLook>();
            gameCamera.m_YAxis.m_MaxSpeed = inventoryOpen ? 0 : 2;
            gameCamera.m_XAxis.m_MaxSpeed = inventoryOpen ? 0 : 300;

            //Reset the UI character model rotation back to default.
            GameObject.Find("UICharacter").transform.rotation = new Quaternion(0,0,0,0);

            //Set the default values of the items in the interact panel. This is so no data is saved on what was clicked last when inventory is closed.
            interactSection.SetDefault();

            //Remove an item slot outline if there was one.
            GameObject targetOutline =  GameObject.Find("InventoryOutline");
            if(targetOutline != null)
            {
                targetOutline.SetActive(false);
            }
        }        
    }

    public void setSlotVisuals(int index, InventoryItem item)
    {
        //Use a switch case to determine a type colour based on the item type
        var itemTypeColour = new Color(0.7f,0.7f,0.7f,0.7f);
        switch (item.itemTemplate.ItemType)
        {
            case Type.Default:
                itemTypeColour = new Color(0.7f,0.7f,0.7f,1f);
                break;
            case Type.Consumable:
                itemTypeColour = new Color(0.8f,0.6f,0.6f,1f);
                break;
            case Type.Resource:
                itemTypeColour = new Color(0.8f,1f,1f,1f);
                break;
            case Type.Weaponry:
                itemTypeColour = new Color(0.6f,0.55f,0.4f,1f);
                break;
            case Type.Armour:
                itemTypeColour = new Color(0.4f,0.4f,0.4f,1f);
                break;
        }

        //Set the info of the slots to display the item in that slot.
        var slotRef = InventoryPanel.transform.GetChild(index + 1);
        slotRef.Find("Background").GetComponent<Image>().color = itemTypeColour;
        slotRef.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemTemplate.ItemName;
        TextMeshProUGUI amountText = slotRef.Find("AmountText").GetComponent<TextMeshProUGUI>();
        //If item is not stackable, the amount text is 1 which doesn't need to be displayed so it is set to be an empty string.
        if(item.itemTemplate.IsStackable)
        {
            amountText.text = item.itemAmount.ToString();
        } else {
            amountText.text = "";
        }
    }

    public void AddToInventory(ItemTemplate itemToAdd)
    {
        //Add a new item to the inventory
        if(itemToAdd.IsStackable)
        {
            //If the item is stackable, we want to check if the player already has that item. This is done by checking each item
            bool wasItemFoundInInventory = false;
            int foundSlotNo = 0;
            for(int i = 0; i < InventoryAmount; i++)
            {       
                if(playerInventory[i].itemTemplate.ItemId == itemToAdd.ItemId)
                {
                    if(playerInventory[i].itemAmount < itemToAdd.maxItemCount) //Checks if the player has space left in that slot to update the quantity (if they have less than the max item count)
                    {
                        wasItemFoundInInventory = true;
                        foundSlotNo = i;
                    }
                }
            }

            //If the player already has that item, then we update the quantity text, otherwise add a new slot for it
            if(wasItemFoundInInventory)
            {
                updateSlotAmount(foundSlotNo);
            } else {
                checkAvailableSlot(itemToAdd);
            }

        } else {
            checkAvailableSlot(itemToAdd);
        }

        setInventorySlots(); //To reinitialise the inventory visuals after the update.
    }

    public void updateSlotAmount(int targetSlot)
    {
        //Increases the item count by 1.
        playerInventory[targetSlot].itemAmount ++;
    }

    public void checkAvailableSlot(ItemTemplate itemToAdd)
    {
        //To find a new available slot, checks the first available slot in order where the ID is 0, which means empty.
        for(int i = 0; i < InventoryAmount; i++)
        {
            if(playerInventory[i].itemTemplate.ItemId == 0)
            {
                playerInventory[i].itemTemplate = itemToAdd;
                playerInventory[i].itemAmount = 1;
                var updateSlot = InventoryPanel.transform.GetChild(i + 1);
                updateSlot.gameObject.GetComponent<DragAndDrop>().setButtonColours(updateSlot, true);
                break;
            }
        }
    }

    public void changeItemSlot(int targetSlot)
    {
        //Uses a temp variable so that one item can be swapped, but then you need the original value of the one that was swapped to replace the other one.
        var temp = playerInventory[DropFromSlot];

        if(playerInventory[targetSlot].itemTemplate.ItemId == 0)
        {
            //If moved to an empty slot, just move the current one to the target and set the target one to be an empty slot.
            playerInventory[DropFromSlot] = playerInventory[targetSlot];
            playerInventory[targetSlot] = temp;
            
            playerInventory[DropFromSlot].itemTemplate = emptyItemTemplate;
            playerInventory[DropFromSlot].itemAmount = 0;
        } else {
            playerInventory[DropFromSlot] = playerInventory[targetSlot];
            playerInventory[targetSlot] = temp;
        }

        //Only need to update the slot visuals of the two slots that were updated
        setSlotVisuals(DropFromSlot, playerInventory[DropFromSlot]);
        setSlotVisuals(targetSlot, playerInventory[targetSlot]);

        DropFromSlot = 0;
    }

    public void RemoveFromInventory(int index)
    {
        if(playerInventory[index].itemTemplate.IsStackable) //If stackable item, check if you have more than 1 of that item, and reduce the quantity of it by 1.
        {
            if(playerInventory[index].itemAmount > 1)
            {
                playerInventory[index].itemAmount --;
            } else {
                playerInventory[index].itemTemplate = emptyItemTemplate;
                interactSection.SetDefault();
            }
        } else {
            playerInventory[index].itemTemplate = emptyItemTemplate;
            interactSection.SetDefault();
        }

        setInventorySlots(); //Update slot visuals.
    }
}

