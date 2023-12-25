using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;
using TMPro;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Defaults")]
    private GameObject playerRef;
    private PlayerInventory playerInventoryRef;
    private ItemInteraction itemInteraction;
    public GameObject itemTemplate;
    private int dragItemPosition;
    private bool canDragDrop;
    private GameObject dragVisual;
    public ItemTemplate emptyItemTemplate;
    public InventoryItem occupiedBy;
    private ItemTemplate interactItem;

    [Header("Button visuals")]
    public Image darkenImage;

    public void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
        playerInventoryRef = playerRef.GetComponent<PlayerInventory>();
        itemInteraction = playerInventoryRef.InventoryScreen.GetComponent<ItemInteraction>();
        
        // By default, each button should not be interactable and not darkened
        setButtonColours(false);
        darkenImage.enabled = false;
    }

    public void setButtonColours(bool enable)
    {
        transform.GetComponent<Button>().interactable = enable;
    }

    public void setEquippableInteraction(string targetEquipSlot)
    {
        // Remove darken effect from only one slot which is the corresponding one of the dragged item.
        GameObject.Find(targetEquipSlot).GetComponent<Button>().interactable = true;
        GameObject.Find(targetEquipSlot).GetComponent<DragAndDrop>().darkenImage.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only works with left and middle mouse input.
        if(itemInteraction.dragInput == DragInput.None && (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle))
        {
            dragItemPosition = eventData.pointerEnter.transform.parent.GetSiblingIndex(); // Find index of the clicked item by checking its position in the hierarchy.

            bool isInventoryItem = eventData.pointerEnter.transform.parent.transform.IsChildOf(playerInventoryRef.InventorySlots.transform); // Check if item is in inventory or equipped.

            bool checkSplit = eventData.button == PointerEventData.InputButton.Middle; // If middle mouse, then trying to split the item.

            // Drag and drop is only enabled if the clicked slot had an item and wasn't empty
            InventoryItem item = playerInventoryRef.playerInventory[dragItemPosition];
            if(!checkSplit && isInventoryItem && item.itemTemplate.ItemId != 0 ||
                checkSplit && isInventoryItem && item.itemTemplate.ItemId != 0)
            {
                // If trying to split stack, only set can drag drop to true if the item has more than 1 quantity.
                if(!checkSplit || checkSplit && item.itemAmount > 1)
                {
                    playerInventoryRef.DropFromSlot = dragItemPosition;
                    playerInventoryRef.DropFromInventory = true;
                    canDragDrop = true;
                }

                // Sets the outline slot image based on if clicked on an occupied slot
                if(playerInventoryRef.playerInventory[dragItemPosition].itemTemplate.ItemId != 0)
                    itemInteraction.setItemInteraction(playerInventoryRef.playerInventory[dragItemPosition].itemTemplate, dragItemPosition, true);

                playerInventoryRef.splitStack = checkSplit && isInventoryItem && item.itemTemplate.ItemId != 0 && item.itemAmount > 1;
                    
            } else {
                if(!checkSplit)
                {
                    // Code if clicked item was an equipped item.
                    playerInventoryRef.DropFromInventory = false;
                    bool equippedItemDetected = false;

                    // Checks if the equipped item slot is not empty.
                    var droppedSlot = eventData.pointerEnter.transform.parent;
                    switch(occupiedBy.itemTemplate.ItemType)
                    {
                        default:
                            break;
                        case Type.Weaponry:
                            WeaponryItem weaponryItem = occupiedBy.itemTemplate as WeaponryItem;
                            if(weaponryItem.weaponType != WeaponType.Shield)
                            {
                                equippedItemDetected = GameObject.Find(droppedSlot.name).GetComponent<DragAndDrop>().occupiedBy.itemTemplate.ItemId != 0;
                            } else {
                                equippedItemDetected = GameObject.Find(weaponryItem.weaponType + "Slot").GetComponent<DragAndDrop>().occupiedBy.itemTemplate.ItemId != 0;
                            }
                            break;
                        case Type.Armour:
                            ArmourItem armourItem = occupiedBy.itemTemplate as ArmourItem;
                            equippedItemDetected = GameObject.Find(armourItem.armourType + "Slot").GetComponent<DragAndDrop>().occupiedBy.itemTemplate.ItemId != 0;
                            break;
                    }

                    canDragDrop = equippedItemDetected;

                    if(canDragDrop)
                    {
                        itemInteraction.setItemInteraction(occupiedBy.itemTemplate, dragItemPosition, false);
                        itemInteraction.setEquipState(UseState.Unequip);
                        playerInventoryRef.DropFromSlotName = eventData.pointerEnter.transform.parent.transform.name;
                    }
                }
            }

            itemInteraction.dragInput = DragInput.WaitInput; // Set to a temporary state so that when drag begins, can be checked if dragged with same mouse click down as when clicked.

            playerInventoryRef.lastDraggedItem = occupiedBy;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // When mouse click release, only reset the state if released with the same mouse input as when clicked.
        if(itemInteraction.dragInput == DragInput.WaitInput && (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle))
        {
            itemInteraction.dragInput = DragInput.None;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Middle )
        {
            if(canDragDrop && GameObject.Find("DragDropRef").transform.childCount == 0)
            {
                // Set info of dragged item visual.
                dragVisual = Instantiate(itemTemplate, GameObject.Find("DragDropRef").transform);
                dragVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(60,60);
                dragVisual.GetComponent<CanvasGroup>().alpha = 0.7f; // Opacity lowered so can differentiate easily.
                dragVisual.GetComponent<CanvasGroup>().blocksRaycasts = false;
                dragVisual.GetComponent<RectTransform>().localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Default position at mouse position when dragged.
                if(dragVisual.transform.Find("TypeText") != null)
                    Destroy(dragVisual.transform.Find("TypeText").GetComponent<TextMeshProUGUI>());
                playerInventoryRef.isDragDrop = true;

                // Set all highlights to true while dragging
                int inventoryAmount = playerRef.GetComponent<PlayerInventory>().InventoryAmount;
                GameObject inventorySlots = playerRef.GetComponent<PlayerInventory>().InventorySlots;
                for(int i = 0; i < inventoryAmount; i++)
                {
                    inventorySlots.transform.GetChild(i).GetComponent<DragAndDrop>().setButtonColours(true);
                }

                itemInteraction.setEquippableHighlight(playerInventoryRef.lastDraggedItem.itemTemplate);
                
                itemInteraction.dragInput = DragInput.WaitLeft;

                // If splitting. Half current stack and set visual text to remaining.
                if(playerInventoryRef.splitStack)
                {
                    int originalAmount = playerInventoryRef.lastDraggedItem.itemAmount;
                    playerInventoryRef.lastDraggedItem.itemAmount /= 2;
                    playerInventoryRef.splitAmount = originalAmount - playerInventoryRef.lastDraggedItem.itemAmount; // Set remaining value after split.
                    playerInventoryRef.setSlotVisuals(inventorySlots.transform.GetChild(playerInventoryRef.DropFromSlot), playerInventoryRef.lastDraggedItem);
                    dragVisual.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = (originalAmount - playerInventoryRef.lastDraggedItem.itemAmount).ToString();
                    dragVisual.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().color = Color.green; // Set text in visual to green for differentiation.
                    
                    itemInteraction.dragInput = DragInput.WaitMiddle;
                }
                
                itemInteraction.setButtonsInteractable(false);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Only called if dragging with same mouse click down as when pointer down.
        if(eventData.button == PointerEventData.InputButton.Left && itemInteraction.dragInput == DragInput.WaitLeft && canDragDrop)
        {
            dragVisual.transform.position = eventData.position;
            Transform targetObject = eventData.pointerEnter.transform.parent;
        } else if(eventData.button == PointerEventData.InputButton.Middle && itemInteraction.dragInput == DragInput.WaitMiddle && canDragDrop) {
            dragVisual.transform.position = eventData.position;
            Transform targetObject = eventData.pointerEnter.transform.parent;
        }  
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left && itemInteraction.dragInput == DragInput.WaitLeft ||
            eventData.button == PointerEventData.InputButton.Middle && itemInteraction.dragInput == DragInput.WaitMiddle)
        {
            var droppedSlot = eventData.pointerEnter.transform.parent; // Find which slot the player dropped on.
            GameObject inventorySlots = playerRef.GetComponent<PlayerInventory>().InventorySlots;

            if(playerInventoryRef.isDragDrop && droppedSlot.GetComponent<Button>().interactable)
            {
                // Can only drop if currently dragging item and the dropped button is interactable (can't equip items of mistmatching type)
                
                dragItemPosition = eventData.pointerEnter.transform.parent.GetSiblingIndex();

                var itemRef = playerInventoryRef.playerInventory[playerInventoryRef.DropFromSlot];

                // Checks if dropped on weapon or armour slot to equip.
                WeaponryItem weaponryItem = itemRef.itemTemplate as WeaponryItem;
                ArmourItem armourItem = itemRef.itemTemplate as ArmourItem;
                if(itemRef.itemTemplate.ItemType == Type.Weaponry &&
                    (playerInventoryRef.DropFromSlotName == "" || playerInventoryRef.DropFromSlotName == null) &&
                    (droppedSlot.name == "PrimaryWeaponSlot" || droppedSlot.name == "SecondaryWeaponSlot" || droppedSlot.name == "ShieldWeaponSlot" || droppedSlot.name == weaponryItem.weaponType + "Slot") ||
                    (itemRef.itemTemplate.ItemType == Type.Armour && droppedSlot.name == armourItem.armourType + "Slot"))
                {
                    playerInventoryRef.equipItem(playerInventoryRef.DropFromSlot, droppedSlot.name);
                } else {
                    if(!playerInventoryRef.DropFromInventory)
                    {
                        bool isInventoryItem = eventData.pointerEnter.transform.parent.transform.IsChildOf(playerInventoryRef.InventorySlots.transform);
                        
                        if(playerInventoryRef.DropFromSlotName != eventData.pointerEnter.transform.parent.transform.name && 
                            (eventData.pointerEnter.transform.parent.transform.name == "PrimaryWeaponSlot" || eventData.pointerEnter.transform.parent.transform.name == "SecondaryWeaponSlot"))
                        {
                            playerInventoryRef.swapPrimarySecondaryWeapon();
                        }
                        
                        playerInventoryRef.unequipItem(dragItemPosition, isInventoryItem);
                    } else {
                        DragAndDrop targetSlot = eventData.pointerEnter.transform.parent.GetComponent<DragAndDrop>();
                        if(targetSlot != null && targetSlot.darkenImage.enabled == false)
                        {
                            playerInventoryRef.changeItemSlot(dragItemPosition); // Swaps the items of the dragged item and the dropped.
                            playerInventoryRef.DropFromSlot = dragItemPosition;
                        }
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(canDragDrop &&
            (eventData.button == PointerEventData.InputButton.Left && itemInteraction.dragInput == DragInput.WaitLeft ||
            eventData.button == PointerEventData.InputButton.Middle && itemInteraction.dragInput == DragInput.WaitMiddle))
        {
            if(GameObject.Find("DragDropRef").transform.childCount > 0)
                Destroy(GameObject.Find("DragDropRef").transform.GetChild(0).gameObject); // Remove the drag drop visual if it is currently there.
            
            // Reset default variables to null/false so it does not interfere with othe drag drops
            dragVisual = null;
            interactItem = null;
            
            dragItemPosition = 0;

            playerInventoryRef.isDragDrop = false;
            playerInventoryRef.DropFromSlotName = null;
            canDragDrop = false;

            itemInteraction.setEquippableHighlight(null);

            itemInteraction.setButtonsInteractable(true);

            itemInteraction.dragInput = DragInput.None; // Resets drag state enum.

            // Resets split stack if true.
            if(playerInventoryRef.splitStack)
                playerInventoryRef.returnStack();
        }
    }
}

