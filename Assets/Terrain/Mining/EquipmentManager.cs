using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public Text interactionText; // UI Text to display the interaction prompt

    [Header("Detection")]
    public float detectionRadius = 3f;
    public LayerMask breakableLayer; // Layer for breakable objects

    [Header("Equipment")]
    public GameObject axePrefab;
    public GameObject pickaxePrefab;
    private GameObject currentEquippedItem;

    private GameObject detectedObject;

    private void Update()
    {
        // Check for breakable objects in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, breakableLayer);
        detectedObject = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Breakable")) // Ensure your breakable objects have the tag "Breakable"
            {
                detectedObject = hitCollider.gameObject;
                break;
            }
        }

        // Show or hide the interaction text based on whether a breakable object is detected
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(detectedObject != null);
            interactionText.text = detectedObject != null ? "Press E to mine" : string.Empty;
        }

        // If the player presses the interaction key near a breakable object, equip the appropriate tool
        if (Input.GetKeyDown(interactionKey) && detectedObject != null)
        {
            EquipToolFor(detectedObject);
        }
    }

    private void EquipToolFor(GameObject breakableObject)
    {
        // Determine which tool to equip based on the type of the breakable object
        // This is a placeholder switch, you'll need to determine the logic based on your game's rules
        switch (breakableObject.name) // This could also be a property or a type of the object
        {
            case "Rock":
                EquipItem(pickaxePrefab);
                break;
            case "Tree":
                EquipItem(axePrefab);
                break;
            default:
                Debug.Log("No appropriate tool for this object.");
                break;
        }
    }

    private void EquipItem(GameObject itemPrefab)
    {
        // If there's already an item equipped, unequip it first
        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
        }

        // Instantiate the item and place it at the hand's position
        currentEquippedItem = Instantiate(itemPrefab, transform.position, transform.rotation);
        currentEquippedItem.transform.SetParent(transform); // Parent the tool to the player or hand transform

        // You might want to adjust the position and rotation to fit the player's hand
        currentEquippedItem.transform.localPosition = new Vector3(0, 0, 0); // Adjust as necessary
        currentEquippedItem.transform.localRotation = Quaternion.identity; // Adjust as necessary
    }
}
