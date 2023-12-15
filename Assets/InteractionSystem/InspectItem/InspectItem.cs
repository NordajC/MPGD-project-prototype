using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using TMPro;

public class InspectItem : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("Prefabs")]
    public GameObject defaultPanel;
    public GameObject threeStatsPanel;

    [Header("Default")]
    public Transform descriptionPanel;
    
    [Header("Colours")]
    public string consumableColour = "#FF0000";
    public string resourceColour = "#00FFFF";
    public string toolColour = "#00FF6A";
    public string weaponryColour = "#FFBE00";
    public string weaponryTypeColour = "#C19000";
    public string armourColour = "#89B1C3";
    public string armourTypeColour = "#507687";
    public string greenColour = "#00E757";
    public string redColour = "#C80014";
    public string defaultColour = "#919191";

    [Header("3D Object")]
    public float rotateSpeed = 0.4f;
    private Vector3 lastMousePosition;
    private GameObject inspectMesh;

    public TextMeshProUGUI getStatsText()
    {
        TextMeshProUGUI text = GameObject.Find("StatsTypeText").GetComponent<TextMeshProUGUI>();
        return text;
    }

    public TextMeshProUGUI getStatsValueText()
    {
        TextMeshProUGUI text = GameObject.Find("StatsValueText").GetComponent<TextMeshProUGUI>();
        return text;
    }

    public string getStatsValueText(float stat)
    {
        if(stat < 0)
        {
            return "<color=" + redColour + ">-" + stat + "</color>\n";
        } else if (stat == 0) {
            return "<color=" + defaultColour + ">" + stat + "</color>\n";
        } else {
            return "<color=" + greenColour + ">+" + stat + "</color>\n";
        }
    }

    public void initialise(ItemTemplate item, float FOV)
    {
        inspectMesh = GameObject.Find("InspectMesh"); // Set inspect mesh reference.

        switch (item.ItemType)
        {
            // Consumable stats: Restored health, hunger and hydration.
            case Type.Consumable:
                Instantiate(threeStatsPanel, descriptionPanel);
                ConsumableItem consumableItem = item as ConsumableItem;
                getStatsText().text = "Item type|\nHealth|\nHunger|\nHydration|";
                getStatsValueText().text = "<color=" + consumableColour + ">Consumable</color>\n" + 
                                        getStatsValueText(consumableItem.restoreHealth) + 
                                        getStatsValueText(consumableItem.restoreHunger) + 
                                        getStatsValueText(consumableItem.restoreHydration);
                break;
            // Resource stats: None.
            case Type.Resource:
                Instantiate(defaultPanel, descriptionPanel);
                getStatsText().text = "Item type|";
                getStatsValueText().text = "<color=" + resourceColour + ">Resource</color>";
                break;
            // Tool stats: None.
            case Type.Tool:
                Instantiate(defaultPanel, descriptionPanel);
                getStatsText().text = "Item type|";
                getStatsValueText().text = "<color=" + toolColour + ">Tool</color>";
                break;
            // Weaponry stats: Weapon type, min damage, defence factor.
            case Type.Weaponry:
                Instantiate(threeStatsPanel, descriptionPanel);
                WeaponryItem weaponryItem = item as WeaponryItem;
                getStatsText().text = "Item type|\nWeapon|\nDamage|\nDefence|";
                getStatsValueText().text = "<color=" + weaponryColour + ">Weaponry</color>\n" + 
                                        "<color=" + weaponryTypeColour + ">"+ weaponryItem.weaponType + "</color>\n" + 
                                        getStatsValueText(weaponryItem.minDamageAmount) + 
                                        getStatsValueText(weaponryItem.defenceFactor);
                break;
            // Armour stats: Armour type, defence factor, attack factor.
            case Type.Armour:
                Instantiate(threeStatsPanel, descriptionPanel);
                ArmourItem armourItem = item as ArmourItem;
                getStatsText().text = "Item type|\nArmour|\nDefence|\nDamage|";
                getStatsValueText().text = "<color=" + armourColour + ">Armour</color>\n" + 
                                        "<color=" + armourTypeColour + ">"+ armourItem.armourType + "</color>\n" + 
                                        getStatsValueText(armourItem.defenceFactor) + 
                                        getStatsValueText(armourItem.attackFactor);
                break;
        }

        GameObject.Find("ItemNameText").GetComponent<TextMeshProUGUI>().text = item.ItemName;
        GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = item.ItemDescription; // Set item description text.

        GameObject.Find("PickupUICamera").GetComponent<Camera>().fieldOfView = FOV; // Set field of view of the UI camera so it fits for every object.

        // Setting the item mesh using the prefab mesh. Also assigning the correct materials.
        inspectMesh.GetComponent<MeshFilter>().sharedMesh = item.pickupPrefab.GetComponent<MeshFilter>().sharedMesh;
        inspectMesh.GetComponent<MeshRenderer>().sharedMaterial = item.pickupPrefab.GetComponent<MeshRenderer>().sharedMaterial;
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
            // Find rotation amount  by using difference in mouse position.
            Vector3 currentMousePosition = eventData.position;
            Vector3 delta = currentMousePosition - lastMousePosition;
        
            inspectMesh.transform.Rotate(Vector3.up, -delta.x * rotateSpeed, 0);
            inspectMesh.transform.Rotate(Vector3.right, -delta.y * rotateSpeed, 0);

            // Update the mouse position while dragging
            lastMousePosition = eventData.position;
        }
    }
}
