using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using UnityEngine.UI;
using TMPro;

public class DragAndDrop : MonoBehaviour, IDragHandler, IDropHandler, IPointerDownHandler
{
    [Header("Defaults")]
    public GameObject itemTemplate;
    private int dragItemPosition;
    private bool canDragDrop;
    private GameObject dragVisual;
    private ItemTemplate interactItem;
    private PlayerInventory playerInventoryRef;

    [Header("Button visuals")]
    public Color defaultColour;
    public Color highlightColourItem;
    public Color highlightColourDrag;
    public GameObject outlinesPrefab;
    public Image darkenImage;

    public void Start()
    {
        //Set default inventory ref;
        playerInventoryRef = GameObject.Find("Player").GetComponent<PlayerInventory>();
        
        //By default, each button should not be interactable and not darkened
        setButtonColours(transform, true);
        darkenImage.enabled = false;
    }

    public void setButtonColours(Transform button, bool getParent)
    {
        //Transform parameter used to select a target button component.
        //Bool parameter used to determine how the function is called.
        var btnColors = button.GetComponent<Button>().colors;
        var target = 0;
        if (getParent)
        {
            target = transform.parent.GetSiblingIndex() - 1;
        } else {
            target = transform.GetSiblingIndex() - 1;
        }
        if(canDragDrop)
        {
            btnColors.highlightedColor = highlightColourDrag; //if dragging and dropping, then all slots will set the highlighted colour. This is different to hovering over buttons without dragging.
        } else {
            if(playerInventoryRef.playerInventory[target].itemTemplate.ItemId == 0) //If hovering, only highlight if that slot has an item.
            {
                btnColors.normalColor = defaultColour;
                btnColors.highlightedColor = defaultColour;
            } else {
                btnColors.normalColor = defaultColour;
                btnColors.highlightedColor = highlightColourItem;
            }
        }
        
        button.GetComponent<Button>().colors = btnColors; //Set the button colours info.
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0)) //Only works if left clicking and dragging
        {
            if(canDragDrop)
            {
                dragVisual.transform.position = eventData.position;
                Transform targetObject = eventData.pointerEnter.transform.parent;

                if(targetObject.GetComponent<Button>() != null && targetObject.CompareTag("InventorySlot")) //Can only drag if the item is an inventory slot (set using a tag in the prefab)
                {
                    var hoveredOverSlotPosition = eventData.pointerEnter.transform.parent.GetSiblingIndex() - 1; 
                    setButtonColours(eventData.pointerEnter.transform.parent, true);
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Input.GetMouseButtonDown(0)) //Only works if pressed with left mouse button
        {
            dragItemPosition = eventData.pointerEnter.transform.parent.GetSiblingIndex() - 1;
            playerInventoryRef.DropFromSlot = dragItemPosition;

            //Drag and drop is only enabled if the clicked slot had an item and wasn't empty
            if(playerInventoryRef.playerInventory[dragItemPosition].itemTemplate.ItemId != 0)
            {
                canDragDrop = true;
            } else {
                canDragDrop = false;
            }

            //Sets the outline slot image based on if clicked on an occupied slot
            dragItemPosition = eventData.pointerEnter.transform.parent.GetSiblingIndex() - 1;
            if(playerInventoryRef.playerInventory[dragItemPosition].itemTemplate.ItemId != 0)
            {
                interactItem = playerInventoryRef.playerInventory[dragItemPosition].itemTemplate;
                bool isEquippable = interactItem.ItemType == Type.Weaponry || interactItem.ItemType == Type.Armour ? true : false;
                ItemInteraction itemInteraction = GameObject.Find("InteractSection").GetComponent<ItemInteraction>();
                itemInteraction.SetInfo(interactItem.ItemName, interactItem.ItemIcon, interactItem.ItemDescription, interactItem.canUse, isEquippable);
                itemInteraction.selectedInventorySlot = dragItemPosition;

                //Remove an item slot outline if there was one.
                GameObject targetOutline =  GameObject.Find("InventoryOutline");
                if(targetOutline != null)
                {
                    targetOutline.SetActive(false);
                }

                outlinesPrefab.SetActive(true); //Set only the clicked items outline to visible;
            } 
        }
    }

    public void setEquippableInteraction(string targetEquipSlot)
    {
        //Remove darken effect from only one slot which is the corresponding one of the dragged item.
        GameObject.Find(targetEquipSlot).GetComponent<Button>().interactable = true;
        GameObject.Find(targetEquipSlot).GetComponent<DragAndDrop>().darkenImage.enabled = false;
    }

    public void setArmourWeaponSlots(bool enabled)
    {
        //Either enables or disables the button component and darken image of each slot based on the current clicked one
        foreach(Transform itemObject in GameObject.Find("CharacterUIPanel").transform)
        {
            if(itemObject.CompareTag("InventorySlot"))
            {
                itemObject.GetComponent<Button>().interactable = !enabled;
                itemObject.GetComponent<DragAndDrop>().darkenImage.enabled = enabled;
            }
        }
    }

    public void canDrag()
    {
        if(Input.GetMouseButton(0)) //Only works if left clicking and begin drag.
        {
            if(canDragDrop)
            {
                //Set info of dragged item visual. Opacity lowered so can differentiate easily.
                dragVisual = Instantiate(itemTemplate, GameObject.Find("DragDropRef").transform);
                dragVisual.GetComponent<CanvasGroup>().alpha = 0.7f;
                dragVisual.GetComponent<CanvasGroup>().blocksRaycasts = false;
                dragVisual.GetComponent<RectTransform>().localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Default position at mouse position when dragged.
                dragVisual.GetComponent<DragAndDrop>().outlinesPrefab.SetActive(false);

                playerInventoryRef.isDragDrop = true;

                setArmourWeaponSlots(true); //Weapon and armour slots darkened by default

                //Switch case used to remove darken effect of only one slot based on the weapon type or armour type of the dragged item.
                if(interactItem != null)
                {
                    switch(interactItem.ItemType)
                    {
                        case Type.Weaponry:
                            WeaponryItem weaponryItem = interactItem as WeaponryItem;
                            if (weaponryItem != null)
                            {
                                setEquippableInteraction(weaponryItem.weaponType + "Slot");
                            } else{
                                return;
                            }
                            break;
                        case Type.Armour:
                            ArmourItem armourItem = interactItem as ArmourItem;
                            if (armourItem != null)
                            {
                                setEquippableInteraction(armourItem.armourType + "Slot");
                            } else{
                                return;
                            }
                            break;
                    }
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(playerInventoryRef.isDragDrop)
        {
            //Can only drop if currently dragging item.
            var droppedSlot = eventData.pointerEnter.transform.parent.GetSiblingIndex() - 1;

            DragAndDrop targetSlot = eventData.pointerEnter.transform.parent.GetComponent<DragAndDrop>();
            if(targetSlot != null && targetSlot.darkenImage.enabled == false)
            {
                playerInventoryRef.changeItemSlot(droppedSlot); //Swaps the items of the dragged item and the dropped.

                foreach(Transform itemUI in GameObject.Find("InventoryPanel").transform)
                {
                    if(itemUI.GetComponent<Button>() != null)
                    {
                        itemUI.GetComponent<DragAndDrop>().outlinesPrefab.SetActive(false); //Removes outlines of the current one
                    }
                }

                if(eventData.pointerEnter.transform.parent.GetComponent<DragAndDrop>() != null)
                {
                    eventData.pointerEnter.transform.parent.GetComponent<DragAndDrop>().outlinesPrefab.SetActive(true);
                }
            } 
        }
    }

    public void removeVisual()
    {
        if(canDragDrop)
        {
            if(GameObject.Find("DragDropRef").transform.childCount > 0)
            {
                Destroy(GameObject.Find("DragDropRef").transform.GetChild(0).gameObject); //Remove the drag drop visual if it is currently there.
            }
            
            //Reset default variables to null/false so it does not interferer with othe drag drops
            dragVisual = null;
            interactItem = null;

            playerInventoryRef.isDragDrop = false;
            canDragDrop = false;
        }

        //Reset button colours
        foreach(Transform itemUI in GameObject.Find("InventoryPanel").transform)
        {
            if(itemUI.GetComponent<Button>() != null)
            {
                itemUI.GetComponent<DragAndDrop>().setButtonColours(itemUI.transform, false);
            }
        }
    }
}

