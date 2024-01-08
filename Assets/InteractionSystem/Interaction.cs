using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Interaction : MonoBehaviour
{
    // Enum to switch between different detection modes.
    public enum DetectMode
    {
        closestToPlayer,
        closestToCamera
    }
    
    [Header("Detection system")]
    public DetectMode detectMode;
    public bool lockInteraction = false;
    public float minAngle = 15f;
    private List<BaseInteraction> itemsInRange = new List<BaseInteraction>();
    private BaseInteraction previousClosestItem;
    public BaseInteraction closestItem;
    [HideInInspector] public BaseInteraction interactedItem;
    
    [Header("References")]
    public GameObject interactUI;
    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }
    
    void Update()
    {
        if(GetComponent<MovementStateManager>().rotationMode == RotationMode.Default && GetComponent<PlayerCombat>().canAttack == true && !lockInteraction)
        {
            closestItem = findClosestItem();
        } else {
            closestItem = null;
        }
        
        if(closestItem != null)
        {
            // If interact item in range, then set text of interact text.
            interactUI.GetComponent<TextMeshProUGUI>().text = closestItem.interactText;
        } else {
            // If no interact item in range, then disable outlines.
            if(previousClosestItem != null && previousClosestItem.outline != null)
                previousClosestItem.outline.enabled = false;

            if(closestItem != null && closestItem.outline != null)
                closestItem.outline.enabled = false;
        }

        // Smoothly show or hide interact text if there is an interact item in range or not.
        float targetAlpha = closestItem != null && playerInventory.currentScreen == CurrentScreen.None ? 1 : 0;
        interactUI.GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(interactUI.GetComponent<CanvasGroup>().alpha, targetAlpha, 6f * Time.deltaTime);
    }

    public void addItem(BaseInteraction item)
    {
        // Adds the target item to the in range list if it is not already in it. Used for item detection.
        if(!itemsInRange.Contains(item))
        {
            itemsInRange.Add(item);
        }
    }

    public void removeItem(BaseInteraction item)
    {
        // Removes the target item to the in range list if it is not already in it. Used for item detection.
        if(itemsInRange.Contains(item))
        {
            itemsInRange.Remove(item);
        }
    }

    public BaseInteraction findClosestItem()
    {
        // Only run code to find closest if the list is not empty.
        if(itemsInRange.Count != 0)
        {
            int closestIndex = 0;

            // Switch case used to run different algorithm based on detect mode.
            switch(detectMode)
            {
                case DetectMode.closestToPlayer:
                    // Initial distance set as the distance to the first item so the rest can be compared against.
                    float compareDistance = Vector3.Distance(transform.position, itemsInRange[0].transform.position);

                    // Find closest to player by checking distance between each object in list and player. Lowest distance is found.
                    for(int i = 0; i < itemsInRange.Count; i++)
                    {
                        float checkDistance = Vector3.Distance(transform.position, itemsInRange[i].transform.position);
                        if(checkDistance < compareDistance)
                        {
                            closestIndex = i;
                            compareDistance = checkDistance;
                        }
                    }
                    break;

                case DetectMode.closestToCamera:
                    // Initial angle set as the angle to the first item so the rest can be compared against.
                    Vector3 directionToObject = itemsInRange[0].transform.position - Camera.main.transform.position;
                    float compareAngle = Vector3.Angle(Camera.main.transform.forward, directionToObject);

                    // Find closest to camera by checking distance angle each object in list and player. Lowest angle is found.
                    for(int i = 0; i < itemsInRange.Count; i++)
                    {
                        directionToObject = itemsInRange[i].transform.position - Camera.main.transform.position;
                        float checkAngle = Vector3.Angle(Camera.main.transform.forward, directionToObject);
                        if(checkAngle < compareAngle)
                        {
                            closestIndex = i;
                            compareAngle = checkAngle;
                        }
                    }

                    // The closest object must be within a certain angle to be detected.
                    if(Vector3.Angle(Camera.main.transform.forward, itemsInRange[closestIndex].transform.position - Camera.main.transform.position) > minAngle)
                        return null;
                    
                    break;
            }
            
            // If the interact item is changed, then disable outline of previous one and enable it for new one.
            if(previousClosestItem != closestItem)
            {
                if(previousClosestItem != null && previousClosestItem.outline != null)
                    previousClosestItem.outline.enabled = false;

                if(closestItem != null && closestItem.outline != null)
                    closestItem.outline.enabled = true;

                previousClosestItem = closestItem;
            }

            return itemsInRange[closestIndex]; // Returns the closest item using the index value found by the detection code.
        }
                    
        return null; // If no items in range, return null.
    }
    
    private void OnInteractPrimary()
    {
        // If closest item is not null, call the primary interaction function which uses an interface.
        if(closestItem != null && playerInventory.currentScreen == CurrentScreen.None)
            closestItem.onInteractPrimary();
    }

    private void OnInteractSecondary()
    {
        // If closest item is not null, call the secondary interaction function which uses an interface.
        if(closestItem != null && playerInventory.currentScreen == CurrentScreen.None)
            closestItem.onInteractSecondary();
    }
}
