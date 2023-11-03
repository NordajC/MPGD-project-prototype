using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    [Header("Item info")]
    public TextMeshProUGUI itemNameText;
    public RawImage itemIconTexture;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI useText;

    [Header("Buttons")]
    public Button useButton;
    public Button dropButton;
    public Button removeButton;

    [HideInInspector] public int selectedInventorySlot;
    private CanvasGroup interactSectionCanvas;

    public void Start()
    {
        interactSectionCanvas = GameObject.Find("InteractSection").GetComponent<CanvasGroup>();
        //Add a listener to the drop button so it can be managed in script.
        dropButton.onClick.AddListener(onDropPressed);
    }

    public void SetInfo(string itemName, Texture itemIcon, string itemDescription, bool canUse, bool equippable)
    {
        //Set each slot info with a parameter. If the item is an equippable item, then the text of the useButton is changed .

        itemNameText.text = itemName;
        itemIconTexture.texture = itemIcon;
        itemDescriptionText.text = itemDescription;
        useText.text = equippable ? "EQUIP" : "USE";
        
        useButton.interactable = canUse;
        dropButton.interactable = true;
        removeButton.interactable = true;

        interactSectionCanvas.alpha = 1.0f;
        interactSectionCanvas.interactable = true;
    }

    public void SetDefault()
    {
        //Set info to default by calling function and setting parameters to default ones.
        SetInfo("ITEM NAME", null, "Item description", true, false);

        interactSectionCanvas.alpha = 0f;
        interactSectionCanvas.interactable = false;
    }

    public void onDropPressed()
    {
        //If drop item button is pressed, it will drop one of that item by calling the function created in PlayerInventory script.
        var dropItemObject = GameObject.Find("Player").GetComponent<PlayerInventory>().playerInventory[selectedInventorySlot].itemTemplate.pickupPrefab;
        var playerLocation = GameObject.Find("Player").transform;
        var spawnPosition = playerLocation.position - (playerLocation.right * 1); //Spawns the drop object to the right of the player with an offset.
        Instantiate(dropItemObject, spawnPosition, Quaternion.identity);

        GameObject.Find("Player").GetComponent<PlayerInventory>().RemoveFromInventory(selectedInventorySlot); //Function called to update the actual inventory list after dropping.
    }
}
