using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.UI;
using TMPro;

using Cinemachine;

using System;

// Enum used to keep track of the currently opened ui screen. This is so different screens can't be opened while another one is.
public enum CurrentScreen
{
    None,
    Inventory,
    Crafting,
    Inspect
}

public enum EquippedWeaponType
{
    Primary,
    Secondary,
    Shield
}

[System.Serializable]
[SerializeField]
public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory System")]
    public GameObject InventoryPanel;
    public GameObject InventorySlots;
    public GameObject ItemUITemplatePrefab;
    public ItemTemplate emptyItemTemplate;
    public int inventoryRowSize = 4;
    private int inventoryColumnSize;
    [Range(15,20)] public int InventoryAmount = 16;
    public GameObject inventoryDOF;

    [Header("Inventory Slot Visuals")]
    public Color defaultBGColour = new Color(0.55f,0.35f,0.1f,0.5f);
    public Color consumableBGColour = new Color(0.8f,0.6f,0.6f,0.5f);
    public Color resourceBGColour = new Color(0.8f,1f,1f,0.5f);
    public Color toolBGColour = new Color(0f,0.85f,0.8f,0.5f);
    public Color weaponryBGColour = new Color(0.6f,0.55f,0.4f,0.5f);
    public Color armourBGColour = new Color(0.4f,0.4f,0.4f,0.5f);

    [Header("Inventory List")]
    public List<InventoryItem> playerInventory = new List<InventoryItem>();
    public InventoryItem playerWeaponPrimary = new InventoryItem();
    public InventoryItem playerWeaponSecondary = new InventoryItem();
    public InventoryItem playerShield = new InventoryItem();
    public InventoryItem playerHelmet = new InventoryItem();
    public Mesh defaultHelmet;
    public InventoryItem playerVest = new InventoryItem();
    public Mesh defaultVest;
    public InventoryItem playerGauntlets = new InventoryItem();
    public Mesh defaultGauntlets;
    public InventoryItem playerTrousers = new InventoryItem();
    public Mesh defaultTrousers;
    public InventoryItem playerBoots = new InventoryItem();
    public Mesh defaultBoots;
    [HideInInspector] public int DropFromSlot;
    [HideInInspector] public string DropFromSlotName;
    [HideInInspector] public bool DropFromInventory;
    [HideInInspector] public InventoryItem lastDraggedItem = new InventoryItem();
    [HideInInspector] public bool splitStack;
    [HideInInspector] public int splitAmount;

    [Header("Interaction")]
    [HideInInspector] public CurrentScreen currentScreen;
    [HideInInspector] public GameObject InventoryScreen;

    [Header("Inventory interaction")]
    public ItemInteraction interactSection;
    [HideInInspector] public bool isDragDrop = false;

    [Header("Crafting")]
    public GameObject craftScreen;

    [Header("Weapon handling")]
    public GameObject primaryWeaponImage;
    public GameObject secondaryWeaponImage;
    public GameObject shieldWeaponImage;

    [Header("Stats bars")]
    public float lagSpeed = 1f;
    public float maxAttackValue;
    private float currentAttackValue;
    public Image attackBarImage;
    public TextMeshProUGUI attackBarText;
    public float maxDefenceValue;
    private float currentDefenceValue;
    public Image defenceBarImage;
    public TextMeshProUGUI defenceBarText;
    
    public InventoryItem getEmptyItem()
    {
        // Gets an empty item to be used when resetting slots.
        var emptyItem = new InventoryItem();
        emptyItem.itemTemplate = emptyItemTemplate;
        return emptyItem;
    }
    
    public Color getItemTypeColour(Type itemType)
    {
        // Returns the item type colour based on the given input.
        Dictionary<Type, Color> itemTypeColourMap;
        itemTypeColourMap = new Dictionary<Type, Color>
        {
            {Type.Default, defaultBGColour},
            {Type.Consumable, consumableBGColour},
            {Type.Resource, resourceBGColour},
            {Type.Tool, toolBGColour},
            {Type.Weaponry, weaponryBGColour},
            {Type.Armour, armourBGColour}
        };
        
        return itemTypeColourMap[itemType];
    }

    public void destroyAttachedObjects(string parentObj)
    {
        foreach(Transform attachedObj in GameObject.Find(parentObj).transform)
        {
            Destroy(attachedObj.gameObject);
        }
    }
    
    public void setSlotVisuals(Transform slotRef, InventoryItem item)
    {
        slotRef.GetComponent<DragAndDrop>().occupiedBy = item;

        // Set the info of the slots to display the item in that slot.
        if(slotRef.transform.Find("DefaultBG").Find("Texture") != null)
            slotRef.transform.Find("DefaultBG").Find("Texture").GetComponent<RawImage>().color = getItemTypeColour(item.itemTemplate.ItemType);
        
        // DEBUG: Set item name text
        // slotRef.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemTemplate.ItemName;
        
        slotRef.Find("Icon").GetComponent<Image>().sprite = item.itemTemplate.ItemIcon;

        // If item is not stackable, the amount text is 1 which doesn't need to be displayed so it is set to be an empty string.
        TextMeshProUGUI amountText = slotRef.Find("AmountText").GetComponent<TextMeshProUGUI>();
        amountText.text = item.itemTemplate.IsStackable && item.itemAmount > 1 ? item.itemAmount.ToString() : "";
        
        var tooltipRef = slotRef.Find("ItemVisualImage");
        if(tooltipRef != null)
            tooltipRef.GetComponent<Image>().enabled = item.itemTemplate.ItemId == 0;

        var buttonRef = slotRef.GetComponent<Button>();
        if(buttonRef != null)
            buttonRef.interactable = item.itemTemplate.ItemId != 0; // A slot button should only be interactable if it is not empty.

        if(!slotRef.IsChildOf(InventorySlots.transform))
            slotRef.GetComponent<DragAndDrop>().darkenImage.enabled = false;
    }

    public void updateArmour(string typeName, ref InventoryItem itemToCheck, Mesh defaultMesh, string[] removeFlags)
    {
        // Function to set armour visuals by modifying the mesh.
        // removeFlags is used to hide other meshes if needed to when armour is added.

        setSlotVisuals(GameObject.Find(typeName + "Slot").transform, itemToCheck);

        SkinnedMeshRenderer targetMesh = GameObject.Find("Male_Armor_" + typeName).GetComponent<SkinnedMeshRenderer>();
        bool isItemEquipped = itemToCheck.itemTemplate.ItemId != 0;

        // Also update the UI mesh to have the armour.
        targetMesh.sharedMesh = isItemEquipped ? itemToCheck.itemTemplate.getEquipMesh() : defaultMesh;
        GameObject.Find("UI_" + typeName).GetComponent<SkinnedMeshRenderer>().sharedMesh = targetMesh.sharedMesh;

        if(removeFlags != null)
        {
            // If remove flags is included, then hide those meshes.
            foreach(string obj in removeFlags)
            {
                GameObject.Find("Male_Armor_" + obj).GetComponent<SkinnedMeshRenderer>().enabled = !isItemEquipped;
                GameObject.Find("UI_" + obj).GetComponent<SkinnedMeshRenderer>().enabled = !isItemEquipped;
            }
        }
    }

    public void updateWeapon(string typeName, ref InventoryItem itemToCheck, EquippedWeaponType equippedWeaponType, ref GameObject iconRef)
    {
        // Function to set weapon visuals by adding mesh.
        setSlotVisuals(GameObject.Find(typeName + "WeaponSlot").transform, itemToCheck);

        WeaponryItem weaponryItem = itemToCheck.itemTemplate as WeaponryItem;

        if(itemToCheck.itemTemplate.ItemId != 0)
        {
            GameObject spawnedVisual;
            GameObject UIWeaponVisual;

            // Different functionality if primary, secondary or shield item.
            switch (equippedWeaponType)
            {
                case EquippedWeaponType.Primary:
                    spawnedVisual = Instantiate(weaponryItem.equipWeapon, GameObject.Find("Male_Weapon_Primary_Right").transform);
                    UIWeaponVisual = Instantiate(weaponryItem.equipWeapon, GameObject.Find("UI_Male_Weapon_Primary_Right").transform);
                    UIWeaponVisual.layer = LayerMask.NameToLayer("UICamera"); // Need to set layer so the UI one is only seen by the UI camera.
                    break;
                case EquippedWeaponType.Secondary:
                        spawnedVisual = Instantiate(weaponryItem.equipWeapon);
                        spawnedVisual.GetComponent<Weapon>().setWeaponSecondary(false);
                        UIWeaponVisual = Instantiate(weaponryItem.equipWeapon);
                        UIWeaponVisual.GetComponent<Weapon>().setWeaponSecondary(true);
                        UIWeaponVisual.layer = LayerMask.NameToLayer("UICamera");
                        iconRef.SetActive(true);
                    break;
                case EquippedWeaponType.Shield:
                        spawnedVisual = Instantiate(weaponryItem.equipWeapon, GameObject.Find("Male_Weapon_Primary_Left").transform);
                        UIWeaponVisual = Instantiate(weaponryItem.equipWeapon, GameObject.Find("UI_Male_Weapon_Primary_Left").transform);
                        UIWeaponVisual.layer = LayerMask.NameToLayer("UICamera");
                        iconRef.SetActive(true);
                    break;
            }

            iconRef.transform.Find("Icon").GetComponent<Image>().sprite = itemToCheck.itemTemplate.ItemIcon;
            iconRef.transform.Find("DefaultIcon").GetComponent<Image>().enabled = false; // If item not empty then default tooltip image hidden.
        } else {
            iconRef.transform.Find("Icon").GetComponent<Image>().sprite = getEmptyItem().itemTemplate.ItemIcon; // To set image to the empty icon image.
            iconRef.transform.Find("DefaultIcon").GetComponent<Image>().enabled = true;

            // If secondary or shield item is empty, then hide the icon gameobject.
            if(equippedWeaponType == EquippedWeaponType.Secondary || equippedWeaponType == EquippedWeaponType.Shield)
                iconRef.SetActive(false);
            
        }
    }

    public void setInventorySlots()
    {
        // Function that is called when the inventory is updated so that the visuals of it are updated as well.
        foreach (Transform obj in InventorySlots.transform)
        {
            setSlotVisuals(obj, playerInventory[obj.GetSiblingIndex()]);
        }

        // Destroy any attached objects when called so only currently equipped ones are shown.
        destroyAttachedObjects("Male_Weapon_Primary_Right");
        destroyAttachedObjects("UI_Male_Weapon_Primary_Right");

        destroyAttachedObjects("Male_Weapon_Secondary_Left");
        destroyAttachedObjects("UI_Male_Weapon_Secondary_Left");
        destroyAttachedObjects("Male_Weapon_Secondary_Back");
        destroyAttachedObjects("UI_Male_Weapon_Secondary_Back");

        destroyAttachedObjects("Male_Weapon_Primary_Left");
        destroyAttachedObjects("UI_Male_Weapon_Primary_Left");

        // For weapons.
        updateWeapon("Primary", ref playerWeaponPrimary, EquippedWeaponType.Primary, ref primaryWeaponImage);
        updateWeapon("Secondary", ref playerWeaponSecondary, EquippedWeaponType.Secondary, ref secondaryWeaponImage);
        updateWeapon("Shield", ref playerShield, EquippedWeaponType.Shield, ref shieldWeaponImage);
        
        // For armour.
        updateArmour("Helmet", ref playerHelmet, defaultHelmet, new string[] {"Hair", "Beard"});
        updateArmour("Vest", ref playerVest, defaultVest, null);
        updateArmour("Gauntlets", ref playerGauntlets, defaultGauntlets, null);
        updateArmour("Trousers", ref playerTrousers, defaultTrousers, null);
        updateArmour("Boots", ref playerBoots, defaultBoots, null);

        // Calculate value in range of 0 and 1 by dividing it by the max
        currentAttackValue = (playerWeaponPrimary.itemTemplate.getAttackValue() + playerWeaponSecondary.itemTemplate.getAttackValue() + playerShield.itemTemplate.getAttackValue() +
                            playerGauntlets.itemTemplate.getAttackValue() + playerBoots.itemTemplate.getAttackValue()) /
                            maxAttackValue;
        currentAttackValue = Mathf.Round(currentAttackValue * 20)/20; // Values are rounded to nearest 0.05 for better readability.

        currentDefenceValue = (playerShield.itemTemplate.getDefenceValue() +
                            playerHelmet.itemTemplate.getDefenceValue() + playerVest.itemTemplate.getDefenceValue() + playerGauntlets.itemTemplate.getDefenceValue() + playerTrousers.itemTemplate.getDefenceValue() + playerBoots.itemTemplate.getDefenceValue()) /
                            maxDefenceValue;

        currentDefenceValue = Mathf.Round(currentDefenceValue * 20)/20;
    }
    
    void Awake()
    {
        InventoryScreen = GameObject.FindWithTag("InventoryScreen");

        // Dynamically sets the inventory panel sizes based on the size of the inventory.
        // Only the y size needs to be modified as X will remain the same since the no of columns is pre-defined.
        inventoryColumnSize = ((InventoryAmount + (inventoryRowSize - 1))/inventoryRowSize) - 1;
        InventoryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 125 + (inventoryColumnSize * 75) + 5);

        for(int i = 0; i < InventoryAmount; i++)
        {
            var spawnedTemplate = Instantiate(ItemUITemplatePrefab, InventorySlots.transform);
            playerInventory.Add(getEmptyItem()); // By default each item is set to an empty item as they don't have any.
        }

        // Each weapon and armour needs to be empty by default. Also the last dragged item.
        playerWeaponPrimary = getEmptyItem();
        playerWeaponSecondary = getEmptyItem();
        playerShield = getEmptyItem();
        playerHelmet = getEmptyItem();
        playerVest = getEmptyItem();
        playerGauntlets = getEmptyItem();
        playerTrousers = getEmptyItem();
        playerBoots = getEmptyItem();
        
        lastDraggedItem = getEmptyItem();

        setInventorySlots();
    }

    void Update()
    {
        // DEBUG. Inventory in text format.
        /*
        string debugText = "";
        TextMeshProUGUI textRef = GameObject.Find("DebugInventoryText").GetComponent<TextMeshProUGUI>();
        foreach(InventoryItem item in playerInventory)
        {
            debugText += "\n" + "   " + item.itemTemplate.ItemName + "| " + item.itemAmount + "|< " + item.itemTemplate.ItemId + ">";
        }
        debugText += "\n" + "\n" + "   Primary: " + playerWeaponPrimary.itemTemplate.ItemName + "| < " + playerWeaponPrimary.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Secondary:" + playerWeaponSecondary.itemTemplate.ItemName + "|< " + playerWeaponSecondary.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Shield:" + playerShield.itemTemplate.ItemName + "|< " + playerShield.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Helmet:" + playerHelmet.itemTemplate.ItemName + "|< " + playerHelmet.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Vest:" + playerVest.itemTemplate.ItemName + "|< " + playerVest.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Gauntlet:" + playerGauntlets.itemTemplate.ItemName + "|< " + playerGauntlets.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Trousers:" + playerTrousers.itemTemplate.ItemName + "|< " + playerTrousers.itemTemplate.ItemId + ">";
        debugText += "\n" + "   Boots:" + playerBoots.itemTemplate.ItemName + "|< " + playerBoots.itemTemplate.ItemId + ">";
        textRef.text = debugText;
        */
        
        // Smoothly interpolate the attack and defence bars and text.
        if(attackBarImage.fillAmount != currentAttackValue)
        {
            attackBarImage.fillAmount = Mathf.MoveTowards(attackBarImage.fillAmount, currentAttackValue, lagSpeed * Time.deltaTime);
            attackBarText.text = (attackBarImage.fillAmount + 1).ToString("0.00"); // To format to 2dp.
            Color originalAttackColour = attackBarText.color;
            originalAttackColour.a = 0.2f + (attackBarImage.fillAmount) * (0.8f)/(1.0f); // Maps the range between 0.2 and 1.0 for opacity. Higher value has more visible text colour.
            attackBarText.color = originalAttackColour;
        }

        if(defenceBarImage.fillAmount != currentDefenceValue)
        {
            defenceBarImage.fillAmount = Mathf.MoveTowards(defenceBarImage.fillAmount, currentDefenceValue, lagSpeed * Time.deltaTime);
            defenceBarText.text = (defenceBarImage.fillAmount + 1).ToString("0.00");
            Color originalDefenceColour = defenceBarText.color;
            originalDefenceColour.a = 0.2f + (defenceBarImage.fillAmount) * (0.8f)/(1.0f);
            defenceBarText.color = originalDefenceColour;
        }
    }

    public void disableInput(bool disableCamera, bool disableMovement)
    {
        // If inventory open, then the cursor should be visible and unlocked so it can be moved.
        Cursor.visible = disableCamera;
        Cursor.lockState = disableCamera ? CursorLockMode.None : CursorLockMode.Locked;

        // Disable mouse movement by disabling aim script.
        gameObject.GetComponent<AimStateManager>().enabled = !disableCamera;

        // Disable player movement by disabling movement script.
        gameObject.GetComponent<MovementStateManager>().canMove = !disableMovement;
    }

    private void OnToggleInventory()
    {

        // Player can toggle the inventory using the input.
        if((currentScreen == CurrentScreen.None || currentScreen == CurrentScreen.Inventory) && gameObject.GetComponent<MovementStateManager>().rotationMode == RotationMode.Default)
        {
            // To toggle the inventory open, use a ternary operator that checks the current state of the current screen enum.
            currentScreen = currentScreen == CurrentScreen.None ? CurrentScreen.Inventory : CurrentScreen.None; 
            bool inventoryOpen = currentScreen == CurrentScreen.Inventory;

            // If inventory open or closed, then it is shown or hidden by setting the canvas group settings.
            CanvasGroup playerUI = InventoryScreen.GetComponent<CanvasGroup>();
            playerUI.alpha = inventoryOpen ? 1 : 0;
            playerUI.interactable = inventoryOpen;
            inventoryDOF.SetActive(inventoryOpen);
            

            disableInput(inventoryOpen, false);

            // Reset the UI character model rotation back to default.
            GameObject.Find("UICharacter").transform.rotation = new Quaternion(0,0,0,0);

            // Set the default values of the items in the interact panel. This is so no data is saved on what was clicked last when inventory is closed.
            interactSection.SetDefault();
            interactSection.disableOutline();
        }
    }

    public void AddToInventory(ItemTemplate itemToAdd)
    {
        // Add a new item to the inventory
        if(itemToAdd.IsStackable)
        {
            // If the item is stackable, we want to check if the player already has that item. This is done by checking each item
            bool wasItemFoundInInventory = false;
            int foundSlotNo = 0;
            for(int i = 0; i < InventoryAmount; i++)
            {       
                if(playerInventory[i].itemTemplate.ItemId == itemToAdd.ItemId)
                {
                    //Checks if the player has space left in that slot to update the quantity (if they have less than the max item count)
                    if(playerInventory[i].itemAmount < itemToAdd.maxItemCount)
                    {
                        wasItemFoundInInventory = true;
                        foundSlotNo = i;
                    }
                }
            }

            // If the player already has that item, then we update the quantity text, otherwise add a new slot for it
            if(wasItemFoundInInventory)
            {
                updateSlotAmount(foundSlotNo);
            } else {
                checkAvailableSlot(itemToAdd);
            }

        } else {
            checkAvailableSlot(itemToAdd);
        }

        setInventorySlots(); // To reinitialise the inventory visuals after the update.
    }

    public void updateSlotAmount(int targetSlot)
    {
        // Increases the item count by 1.
        playerInventory[targetSlot].itemAmount ++;
    }

    public void checkAvailableSlot(ItemTemplate itemToAdd)
    {
        // To find a new available slot, checks the first available slot in order where the ID is 0, which means empty.
        for(int i = 0; i < InventoryAmount; i++)
        {
            if(playerInventory[i].itemTemplate.ItemId == 0)
            {
                var addItem = new InventoryItem();
                addItem.itemTemplate = itemToAdd;
                addItem.itemAmount = 1;
                
                playerInventory[i] = addItem;
                
                var updateSlot = InventorySlots.transform.GetChild(i);
                updateSlot.gameObject.GetComponent<DragAndDrop>().setButtonColours(updateSlot);
                break;
            }
        }
    }

    public int? getAvailableSlotIndex()
    {
        // Nullable int used. If no available slots, then null returned. Otherwise, the index is.
        for(int i = 0; i < InventoryAmount; i++)
        {
            if(playerInventory[i].itemTemplate.ItemId == 0)
                return i;
        }
        
        return null;
    }

    public void changeItemSlot(int targetSlot)
    {
        // Uses a temp variable so that one item can be swapped, but then you need the original value of the one that was swapped to replace the other one.
        var temp = playerInventory[DropFromSlot];

        if(!splitStack)
        {
            if(playerInventory[targetSlot].itemTemplate.ItemId == 0)
            {
                // If moved to an empty slot, just move the current one to the target and set the target one to be an empty slot.
                playerInventory[DropFromSlot] = playerInventory[targetSlot];
                playerInventory[targetSlot] = temp;
                
                playerInventory[DropFromSlot].itemTemplate = emptyItemTemplate;
                playerInventory[DropFromSlot].itemAmount = 0;
            } else {
                if(playerInventory[targetSlot].itemTemplate.ItemId == playerInventory[DropFromSlot].itemTemplate.ItemId &&
                playerInventory[targetSlot].itemAmount < playerInventory[targetSlot].itemTemplate.maxItemCount && 
                playerInventory[DropFromSlot].itemAmount < playerInventory[DropFromSlot].itemTemplate.maxItemCount &&
                targetSlot != DropFromSlot)
                {
                    // If item in start and target slots are the same item type, then merge them if there is space
                    int maxAmount = playerInventory[targetSlot].itemTemplate.maxItemCount - playerInventory[targetSlot].itemAmount;
                    int amountToMove = 0;
                    if(playerInventory[DropFromSlot].itemAmount <= maxAmount) //If the whole quantity can be moved to the target
                    {
                        amountToMove = playerInventory[DropFromSlot].itemAmount;
                        playerInventory[targetSlot].itemAmount += amountToMove;
                        playerInventory[DropFromSlot].itemAmount = 0;
                    } else {
                        // If the whole quantity cannot be moved, then find the amount that can be so the slot dropped from will not be 0 but have the remainder amount
                        amountToMove = playerInventory[targetSlot].itemTemplate.maxItemCount - playerInventory[targetSlot].itemAmount;
                        playerInventory[targetSlot].itemAmount += amountToMove;
                        playerInventory[DropFromSlot].itemAmount -= amountToMove;
                    }
                } else {
                    playerInventory[DropFromSlot] = playerInventory[targetSlot];
                    playerInventory[targetSlot] = temp;
                }
            }

            interactSection.setItemInteraction(playerInventory[targetSlot].itemTemplate, targetSlot, true);

            DropFromSlot = 0;
        } else {
            // If moved item was a split stack.
            if(playerInventory[targetSlot].itemTemplate.ItemId == playerInventory[DropFromSlot].itemTemplate.ItemId)
            {
                if(playerInventory[targetSlot].itemAmount < playerInventory[targetSlot].itemTemplate.maxItemCount)
                {
                    int maxAmount = playerInventory[targetSlot].itemTemplate.maxItemCount - playerInventory[targetSlot].itemAmount;
                    // Find the available amount left in the target slot to move, if same item type.
                    if(splitAmount < maxAmount)
                    {
                        // If amount to move less than max, then move whole amount.
                        playerInventory[targetSlot].itemAmount += splitAmount;
                    } else {
                        // If amount to move not less than max, then move max possible amount.
                        int moveableAmount = playerInventory[targetSlot].itemTemplate.maxItemCount - splitAmount;
                        playerInventory[targetSlot].itemAmount += maxAmount;
                        playerInventory[DropFromSlot].itemAmount += splitAmount - maxAmount;
                    }

                    interactSection.setItemInteraction(playerInventory[targetSlot].itemTemplate, targetSlot, true);
                } else {
                    // If no space but same item type, then return stack.
                    returnStack();
                }
            } else if (playerInventory[targetSlot].itemTemplate.ItemId == 0) {
                // Move whole stack if target slot is empty.
                playerInventory[targetSlot].itemTemplate = temp.itemTemplate;
                playerInventory[targetSlot].itemAmount = splitAmount;

                interactSection.setItemInteraction(playerInventory[targetSlot].itemTemplate, targetSlot, true);
            } else {
                // If target slot occupied by item of different type, return stack.
                returnStack();
            }

            splitStack = false;
            splitAmount = 0;
        }

        removeEmptyItems();
        setInventorySlots();
    }

    public void returnStack()
    {
        // Return the dragged stack amount back to the original by just adding it back.
        playerInventory[DropFromSlot].itemAmount += splitAmount;
        splitStack = false;
        splitAmount = 0;
        removeEmptyItems();
        setInventorySlots();
    }

    public void RemoveFromInventory(int index)
    {
        // If stackable item, check if you have more than 1 of that item, and reduce the quantity of it by 1.
        if(playerInventory[index].itemTemplate.IsStackable && playerInventory[index].itemAmount > 1)
        {
            playerInventory[index].itemAmount --;
        } else {
            playerInventory[index] = getEmptyItem();
            interactSection.disableOutline();
            interactSection.SetDefault();
        }

        setInventorySlots();
    }

    public void removeEmptyItems()
    {
        // If any items in inventory have a quantity of 0, they are removed.
        // Used after crafting an item as it will set the quantity of used items to 0 but the item template will still remain.
        for(int i = 0; i < InventoryAmount; i++)
        {
            if(playerInventory[i].itemAmount == 0)
                playerInventory[i] = getEmptyItem();
        }
    }

    public void equipUnequipItem()
    {
        if(interactSection.useState == UseState.Equip)
        {
            equipItem(DropFromSlot, "PrimaryWeaponSlot");
        } else {
            // If the current selected item is an equipped item, then call the unequip item function if there is an available slot.
            int? unequipTargetSlot = getAvailableSlotIndex();
            if(unequipTargetSlot != null)
            {
                // If trying to unequip, and there is an available slot, then unequip.
                unequipItem(unequipTargetSlot.Value, true);
                DropFromSlot = unequipTargetSlot.Value;
                interactSection.selectedInventorySlot = unequipTargetSlot.Value;
                interactSection.selectedOutline.SetActive(true);
            }
        }  
    }

    public void equipItem(int movedFromSlot, string droppedSlotName)
    {
        // To equip item, the item type is checked and then the corresponding variable for that type is set.
        InventoryItem inventoryItem = playerInventory[movedFromSlot];
        DropFromSlotName = droppedSlotName;
        switch (inventoryItem.itemTemplate.ItemType)
        {
            case Type.Weaponry:
                WeaponryItem weaponryItem = inventoryItem.itemTemplate as WeaponryItem;
                switch (weaponryItem.weaponType)
                {
                    default:
                        if(droppedSlotName == "PrimaryWeaponSlot")
                        {
                            (playerInventory[movedFromSlot], playerWeaponPrimary) = (playerWeaponPrimary, inventoryItem);
                        } else {
                            (playerInventory[movedFromSlot], playerWeaponSecondary) = (playerWeaponSecondary, inventoryItem);
                        }
                        break;
                    case WeaponType.Shield:
                        (playerInventory[movedFromSlot], playerShield) = (playerShield, inventoryItem);
                        break;
                }
                break;
            case Type.Armour:
                ArmourItem armourItem = inventoryItem.itemTemplate as ArmourItem;
                switch (armourItem.armourType)
                {
                    case ArmourType.Helmet:
                        (playerInventory[movedFromSlot], playerHelmet) = (playerHelmet, inventoryItem);
                        break;
                    case ArmourType.Vest:
                        (playerInventory[movedFromSlot], playerVest) = (playerVest, inventoryItem);
                        break;
                    case ArmourType.Gauntlets:
                        (playerInventory[movedFromSlot], playerGauntlets) = (playerGauntlets, inventoryItem);
                        break;
                    case ArmourType.Trousers:
                        (playerInventory[movedFromSlot], playerTrousers) = (playerTrousers, inventoryItem);
                        break;
                    case ArmourType.Boots:
                        (playerInventory[movedFromSlot], playerBoots) = (playerBoots, inventoryItem);
                        break;                      
                }
                break;
        }
        
        setInventorySlots();
        interactSection.disableOutline();
        interactSection.setEquipState(UseState.Unequip); // When an item is just equipped, the button should be for unequipping, until the item is changed.
    }

    public void unequipItem(int targetSlot, bool shouldUnequip)
    {
        // Should only be able to unequip if the item is dragged onto an empty slot or the same weapon/armour type.
        if(shouldUnequip)
        {
            Type itemType = lastDraggedItem.itemTemplate.ItemType;
            // Unequip item by checking the item type of the item that was dropped to unequip.
            switch (lastDraggedItem.itemTemplate.ItemType)
                {
                    case Type.Weaponry:
                        WeaponryItem weaponryItem = lastDraggedItem.itemTemplate as WeaponryItem;
                        switch (weaponryItem.weaponType)
                        {
                            default:
                                if(DropFromSlotName == "PrimaryWeaponSlot")
                                {
                                    unequipItemValue(ref playerWeaponPrimary, targetSlot, itemType, null);
                                } else {
                                    unequipItemValue(ref playerWeaponSecondary, targetSlot, itemType, null);
                                }
                                break;
                            case WeaponType.Shield:
                                unequipItemValue(ref playerShield, targetSlot, itemType, WeaponType.Shield);
                                break;
                        }
                        break;
                    case Type.Armour:
                        ArmourItem armourItem = lastDraggedItem.itemTemplate as ArmourItem;
                        switch (armourItem.armourType)
                        {
                            case ArmourType.Helmet:
                                unequipItemValue(ref playerHelmet, targetSlot, itemType, ArmourType.Helmet);
                                break;
                            case ArmourType.Vest:
                                unequipItemValue(ref playerVest, targetSlot, itemType, ArmourType.Vest);
                                break;
                            case ArmourType.Gauntlets:
                                unequipItemValue(ref playerGauntlets, targetSlot, itemType, ArmourType.Gauntlets);
                                break;
                            case ArmourType.Trousers:
                                unequipItemValue(ref playerTrousers, targetSlot, itemType, ArmourType.Trousers);
                                break;
                            case ArmourType.Boots:
                                unequipItemValue(ref playerBoots, targetSlot, itemType, ArmourType.Boots);
                                break;                      
                        }
                        break;
                }

                setInventorySlots();
        }
    }

    public void unequipItemValue(ref InventoryItem item, int targetSlot, Type itemType, Enum classType)
    {
        // Function to remove the actual item from the inventory if possible by modifying the player inventory list and the corresponding equippable item variable.
        ItemTemplate itemCompare = playerInventory[targetSlot].itemTemplate;
        
        if (playerInventory[targetSlot].itemTemplate.ItemId == 0)
        {
            playerInventory[targetSlot] = item;
            item = getEmptyItem();

            interactSection.setEquipState(UseState.Equip);
        } else {
            // Checks if either a weapon or armour item, then checks if the target is an empty slot or is the same type of weapon or armour as well to swap.
            if(itemType == Type.Weaponry)
            {
                WeaponryItem weaponryItem = itemCompare as WeaponryItem;
                if (itemCompare != null && (itemCompare.ItemType == itemType || classType != null))
                {
                    var temp = item;
                    item = playerInventory[targetSlot];
                    playerInventory[targetSlot] = temp;
                    
                    interactSection.selectedInventorySlot = targetSlot;
                    interactSection.selectedOutline.SetActive(true);
                
                    interactSection.setEquipState(UseState.Equip);
                } else {
                    interactSection.disableOutline();
                }
            } else if (itemType == Type.Armour) {
                ArmourItem armourItem = itemCompare as ArmourItem;
                if (itemCompare != null && itemCompare.ItemType == itemType && armourItem.armourType == (ArmourType)classType)
                {
                    var temp = item;
                    item = playerInventory[targetSlot];
                    playerInventory[targetSlot] = temp;

                    interactSection.selectedInventorySlot = targetSlot;
                    interactSection.selectedOutline.SetActive(true);

                    interactSection.setEquipState(UseState.Equip);
                } else {
                    interactSection.disableOutline();
                }
            }
        }
    }

    public void swapPrimarySecondaryWeapon()
    {
        // Swap primary and secondary weapons using a temp variable.
        var temp = playerWeaponPrimary;
        playerWeaponPrimary = playerWeaponSecondary;
        playerWeaponSecondary = temp;

        setInventorySlots();
    }

    private void OnChangeWeapon(InputValue value)
    {
        swapPrimarySecondaryWeapon();
    }

    public void OnToggleCrafting()
    {
        // Open crafting screen if current UI is none.
        if(currentScreen == CurrentScreen.None)
        {
            currentScreen = CurrentScreen.Crafting;
            Instantiate(craftScreen);
            disableInput(true, true);
        }
    }
}

public class InventoryComparer : IComparer<InventoryItem>
{
    public int Compare(InventoryItem x, InventoryItem y)
    {
        // To compare the items in the inventory, the id's are compared as the primary 'key'
        int compareId = x.itemTemplate.ItemId.CompareTo(y.itemTemplate.ItemId);
        if(compareId != 0)
        {
            return compareId;
        }
        // The item amount is also compared since there may be multiple stacks and it should order those descending.
        return x.itemAmount.CompareTo(y.itemAmount);
    }
}