using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;
using TMPro;

using System.Linq;

// Different items will do different actions.
public enum UseState
{
    Use,
    Equip,
    Unequip
}

// To check for drag and drop state.
public enum DragInput
{
    None,
    WaitLeft,
    WaitMiddle,
    WaitInput
}

public class ItemInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("References")]
    private GameObject playerRef;
    
    [Header("Item info")]
    public TextMeshProUGUI itemNameText;
    public Image itemIconTexture;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI useText;
    public GameObject isEquippedText;

    [Header("Buttons")]
    public Button useButton;
    public Button dropButton;

    [Header("Interaction")]
    public CanvasGroup interactSectionCanvas;
    public GameObject[] weaponArmourSlots;
    [HideInInspector] public bool slotSelected = false;
    [HideInInspector] public int selectedInventorySlot;
    public GameObject selectedOutline;
    private bool isEquippable;
    [HideInInspector] public UseState useState;
    public DragInput dragInput;

    [Header("3D Character")]
    public float rotateSpeed = 0.1f;
    private Vector3 lastMousePosition;

    public void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
        GameObject.FindWithTag("InventoryScreen").GetComponent<CanvasGroup>().alpha = 0f; // Inventory screen disabled by default.
        
        // Add a listener to the drop button so it can be managed in script.
        useButton.onClick.AddListener(onUsePressed);
        dropButton.onClick.AddListener(onDropPressed);
    }

    public void Update()
    {
        // If an item is selected, then the outline position is moved to the selected slots.
        if(selectedOutline != null && selectedOutline.activeSelf)
        {
            var targetPosition = playerRef.GetComponent<PlayerInventory>().InventorySlots.transform.GetChild(selectedInventorySlot).GetComponent<RectTransform>().position;
            selectedOutline.GetComponent<RectTransform>().position = targetPosition;
        }
    }

    public void disableOutline()
    {
        // Disable the outline visual by setting the position to 0 and then setting it to inactive.
        selectedOutline.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        selectedInventorySlot = 0;
        selectedOutline.SetActive(false);
    }

    public void setButtonsInteractable(bool interactable)
    {
        useButton.interactable = interactable;
        dropButton.interactable = interactable;
    }
    
    public void setItemInteraction(ItemTemplate item, int itemPosition, bool itemSelected)
    {
        // Sets all the information in the inventory interaction panel.
        
        ItemTemplate interactItem = item;

        UseState useState = interactItem.ItemType == Type.Weaponry || interactItem.ItemType == Type.Armour ? UseState.Equip : UseState.Use;

        SetInfo(interactItem.ItemName, interactItem.ItemIcon, interactItem.ItemDescription, interactItem.canUse, useState);
        
        selectedInventorySlot = itemPosition;
        
        if(itemSelected)
        {
            selectedOutline.SetActive(true);
        } else {
            disableOutline();
        }
    }

    public void SetInfo(string itemName, Sprite itemIcon, string itemDescription, bool canUse, UseState interactState)
    {
        // Set each slot info with a parameter.

        itemNameText.text = itemName;
        itemIconTexture.sprite = itemIcon;
        itemDescriptionText.text = itemDescription;

        setEquipState(interactState); // Calls function to set if it is use, equip or unequip.
        
        useButton.interactable = canUse; // Button only interactable if use, equip or unequippable item.
        dropButton.interactable = true;

        interactSectionCanvas.alpha = 1.0f;
        interactSectionCanvas.interactable = true;
    }

    public void setEquipState(UseState equipState)
    {
        // Sets the use button text using the enum.
        useState = equipState;
        useText.text = useState.ToString();
        isEquippedText.SetActive(equipState == UseState.Unequip);
        
        // If equipped item, the colour of the interact panel should be different. Drop button should also be disabled.
        interactSectionCanvas.GetComponent<Image>().color = equipState == UseState.Unequip ? new Color(0.1f, 0.25f, 0.35f, 1f) : new Color(0.35f, 0.25f, 0.1f, 1f);
        dropButton.interactable = equipState != UseState.Unequip;
    }

    public void SetDefault()
    {
        // Set info to default by calling function and setting parameters to default ones.
        SetInfo("ITEM NAME", null, "Item description", true, UseState.Use);

        interactSectionCanvas.alpha = 0f;
        interactSectionCanvas.interactable = false;

        disableOutline();
    }

    public void setArmourWeaponSlots(bool enabled)
    {
        // Either enables or disables the button component and darken image of each slot based on the current clicked one
        // Used to darken all slots when a weapon or armour item is dragged, so that only the corresponding one is not darkened.
        foreach(GameObject itemObject in weaponArmourSlots)
        {
            itemObject.GetComponent<Button>().interactable = !enabled;
            itemObject.GetComponent<DragAndDrop>().darkenImage.enabled = enabled;
        }
    }
    
    public void setEquippableHighlight(ItemTemplate selectedItemType)
    {
        // To set only the slot darken visibility of the item type that was dragged, a switch case is used. 
        if(selectedItemType != null)
        {
            switch(selectedItemType.ItemType)
            {
                default:
                    setArmourWeaponSlots(true);
                    break;
                case Type.Weaponry:
                    setArmourWeaponSlots(true);
                    WeaponryItem weaponryItem = selectedItemType as WeaponryItem;
                    if(weaponryItem.weaponType != WeaponType.Shield)
                    {
                        GameObject.Find("PrimaryWeaponSlot").GetComponent<DragAndDrop>().darkenImage.enabled = false;
                        GameObject.Find("PrimaryWeaponSlot").GetComponent<Button>().interactable = true;
                        GameObject.Find("SecondaryWeaponSlot").GetComponent<DragAndDrop>().darkenImage.enabled = false;
                        GameObject.Find("SecondaryWeaponSlot").GetComponent<Button>().interactable = true;
                    } else {
                        GameObject.Find(weaponryItem.weaponType + "WeaponSlot").GetComponent<DragAndDrop>().darkenImage.enabled = false;
                        GameObject.Find(weaponryItem.weaponType + "WeaponSlot").GetComponent<Button>().interactable = true;
                    }
                    break;
                case Type.Armour:
                    setArmourWeaponSlots(true);
                    ArmourItem armourItem = selectedItemType as ArmourItem;
                    GameObject.Find(armourItem.armourType + "Slot").GetComponent<DragAndDrop>().darkenImage.enabled = false;
                    GameObject.Find(armourItem.armourType + "Slot").GetComponent<Button>().interactable = true;
                    break;
            }
        } else {
            playerRef.GetComponent<PlayerInventory>().setInventorySlots();
        }
    }
    
    public void onUsePressed()
    {
        // Call function to use item when button pressed.
        PlayerInventory playerInventory = playerRef.GetComponent<PlayerInventory>();
        playerInventory.lastDraggedItem.itemTemplate.onItemUsed();
    }

    public void onDropPressed()
    {
        // If drop item button is pressed, it will drop one of that item by calling the function created in PlayerInventory script.
        GameObject dropItemObject = playerRef.GetComponent<PlayerInventory>().playerInventory[selectedInventorySlot].itemTemplate.pickupPrefab;
        
        Vector3 playerLocation = playerRef.transform.position;
        Vector2 spawnPointRadius = Random.insideUnitCircle.normalized * Random.Range(0.8f, 1f); // For random direction spawning. Random magnitude between 0.8 and 1.2.
        var spawnPosition = playerLocation + new Vector3(spawnPointRadius.x, 0, spawnPointRadius.y) + new Vector3(0, Random.Range(0.4f, 1f), 0); // Add a small random z offset.

        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        Instantiate(dropItemObject, spawnPosition, rotation);

        playerRef.GetComponent<PlayerInventory>().RemoveFromInventory(selectedInventorySlot); // Function called to update the actual inventory list after dropping.
    }
    
    public void sortInventory()
    {
        // Store two of the inventory, one that is not modified so it can be compared after sorting to check if anything was changed.
        var inventoryRef = playerRef.GetComponent<PlayerInventory>().playerInventory;
        List<InventoryItem> originalInventory = new List<InventoryItem>(inventoryRef);

        // Sort the initial list using the comparer, then call merge stacks function to merge any stacks if possible.
        inventoryRef.Sort(new InventoryComparer());
        mergeStacks(inventoryRef);

        // The sorted list is added to the end of the list as it is ascending order and empty item slots have id of 0, so this is to place it at the start.
        int count = 0;
        for(int i = 0; i < inventoryRef.Count; i++)
        {
            if(inventoryRef[i].itemTemplate.ItemId != 0)
            {
                var moveItem = inventoryRef[i];
                inventoryRef.RemoveAt(i);
                inventoryRef.Insert(count, moveItem);
                count++;
            }
        }

        playerRef.GetComponent<PlayerInventory>().removeEmptyItems();
        playerRef.GetComponent<PlayerInventory>().setInventorySlots();
        
        // To check if the list was sorted or remained the same as before sorting, each inventory item in the sorted and original list is checked.
        // If an are not equal, it means at least one value was changed by sorting so no more needs to be checked.
        bool wasSorted = false;
        for(int i = 0; i < inventoryRef.Count; i++)
        {
            if(inventoryRef[i].itemTemplate != originalInventory[i].itemTemplate && inventoryRef[i].itemAmount != originalInventory[i].itemAmount)
            {
                wasSorted = true;
                break;
            }
        }

        //Only if sorted, then the item interaction panel is reset.
        if(wasSorted)
            SetDefault();
    }

    public void mergeStacks(List<InventoryItem> inventory)
    {
        // Up to 'count - 1' since it is comparing with the next item and so the last item can not be compared with one after it since it is the last.
        for(int i = 0; i < inventory.Count - 1; i++)
        {
            // Checks if the item is stackable as only stackable items can be merged. Also checks if that item has reached the max item count.
            if(inventory[i].itemTemplate.IsStackable && inventory[i].itemAmount < inventory[i].itemTemplate.maxItemCount)
            {
                for(int j = i + 1; j < inventory.Count; j++)
                {
                    if(inventory[i].itemTemplate.ItemId == inventory[j].itemTemplate.ItemId &&
                    inventory[i].itemAmount < inventory[i].itemTemplate.maxItemCount &&
                    inventory[j].itemAmount > 0)
                    {
                        int maxAmount = inventory[i].itemTemplate.maxItemCount - inventory[i].itemAmount;
                        int moveAmount = maxAmount < inventory[j].itemAmount ? maxAmount : inventory[j].itemAmount;

                        inventory[i].itemAmount += moveAmount;
                        inventory[j].itemAmount -= moveAmount;

                        if (inventory[i].itemAmount == inventory[i].itemTemplate.maxItemCount)
                            break;
                    }
                }

                if (inventory[i].itemAmount == 0)
                    inventory[i].itemTemplate = playerRef.GetComponent<PlayerInventory>().getEmptyItem().itemTemplate;
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Set default mouse position so it can be used to calculate the difference when dragging.
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // Find rotation amount  by using difference in mouse position. We only need to modify the y rotation (left/right)
            Vector3 currentMousePosition = eventData.position;
            Vector3 delta = currentMousePosition - lastMousePosition;
            Vector3 rotation = new Vector3(0, -delta.x, 0) * rotateSpeed;
            GameObject.Find("UICharacter").transform.Rotate(rotation);

            // Update the mouse position while dragging
            lastMousePosition = eventData.position;
        }
    }
}