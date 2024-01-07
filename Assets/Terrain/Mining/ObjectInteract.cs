using UnityEngine;
using TMPro;

public class ObjectInteract : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    public float interactionDistance = 3f;
    public string interactionMessage = "<color=orange>E</color> | Break rock";

    private Transform playerTransform;
    private Rock rockScript;

    // Tool prefabs and hand transform
    public GameObject pickaxePrefab;
    public GameObject axePrefab;
    public Transform handTransform; // Assign this to the player's hand transform in the inspector

    // Position and rotation offsets
    private Vector3 positionOffset = new Vector3(0.120999999f, -0.0659999996f, 0.188999996f);
    private Quaternion rotationOffset = Quaternion.Euler(66.1921463f, 291.213776f, 0.252268225f);

    private GameObject currentTool; // To keep track of the currently equipped tool

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        interactionText.gameObject.SetActive(false);

        rockScript = GetComponent<Rock>();
        if (rockScript != null)
        {
            rockScript.OnRockBreak += HandleRockBreak; // Subscribe to the OnRockBreak event
        }
    }

    private void OnDestroy()
    {
        if (rockScript != null)
        {
            rockScript.OnRockBreak -= HandleRockBreak; // Unsubscribe to prevent memory leaks
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= interactionDistance)
        {
            interactionText.text = interactionMessage;
            interactionText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                EquipToolBasedOnMaterial();
                if (rockScript != null)
                {
                    rockScript.AttemptToDamageRock();
                }
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
            UnequipTool(); // Unequip the tool when moving away from the rock
        }
    }

    private void HandleRockBreak()
    {
        // Hide the interaction text and HP bar
        interactionText.gameObject.SetActive(false);
        if (rockScript.HPBar != null)
        {
            rockScript.HPBar.gameObject.SetActive(false);
        }
        UnequipTool(); // Unequip the tool when the rock breaks
    }

    private void EquipToolBasedOnMaterial()
    {
        if (rockScript.allowedDamageSource == Rock.DamageSource.Pickaxe)
        {
            EquipTool(pickaxePrefab);
        }
        else if (rockScript.allowedDamageSource == Rock.DamageSource.Axe)
        {
            EquipTool(axePrefab);
        }
    }

    private void EquipTool(GameObject toolPrefab)
    {
        // Destroy the current tool if it exists
        if (currentTool != null)
        {
            Destroy(currentTool);
        }

        // Instantiate the new tool at the hand's position and rotation, then apply the offsets
        currentTool = Instantiate(toolPrefab, handTransform.position, handTransform.rotation, handTransform);
        currentTool.transform.localPosition = positionOffset;
        currentTool.transform.localRotation = rotationOffset;
    }

    private void UnequipTool()
    {
        // Destroy the current tool if it exists
        if (currentTool != null)
        {
            Destroy(currentTool);
            currentTool = null; // Reset currentTool to null after destroying
        }
    }
}
