using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public abstract class BaseInteraction : MonoBehaviour, IInteractable
{
    [Header("Defaults")]
    [TextArea(3,10)] public string interactText;
    public GameObject interactBar;
    [HideInInspector] public Outline outline;
    [HideInInspector] public GameObject playerRef;

    private void Awake()
    {
        // If interact item has outline effect, then disable it by default.
        outline = GetComponent<Outline>();
        if(outline != null)
            outline.enabled = false;

        playerRef = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.CompareTag("Player"))
        {
            // If player enters triggers, then add this item to the interaction list.
            other.gameObject.GetComponent<Interaction>().addItem(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        if(other.gameObject.CompareTag("Player"))
        {
            // If player exits triggers, then remove this item to the interaction list.
            other.gameObject.GetComponent<Interaction>().removeItem(this);
        }
    }

    // Same functions as in combat. To move towards interaction objects.
    public IEnumerator SmoothMoveTo(GameObject targetObject, Vector3 targetLocation, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = targetObject.transform.position;
        
        while (elapsedTime < duration)
        {
            targetObject.transform.position = Vector3.Lerp(startingPos, targetLocation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        targetObject.transform.position = targetLocation;
    }
    
    public IEnumerator SmoothRotateTo(GameObject targetObject, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Quaternion startingRot = targetObject.transform.rotation;
        
        while (elapsedTime < duration)
        {
            targetObject.transform.rotation = Quaternion.Lerp(startingRot, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.transform.rotation = targetRotation;
    }

    public void moveAndRotateTo(Transform moveToPosition, float moveToFactor)
    {
        Vector3 direction = transform.position - playerRef.transform.position;
        direction.Normalize();
        direction.y = 0f;
        Vector3 targetPosition = moveToPosition.transform.position + (direction * moveToFactor);

        StartCoroutine(SmoothMoveTo(GameObject.Find("PlayerMain"), targetPosition, 0.2f));
        StartCoroutine(SmoothRotateTo(playerRef, Quaternion.LookRotation(direction), 0.2f));
    }
    
    public void interactionAnimation(string animationName, string progressText)
    {
        togglePlayerMovement(false);

        playerRef.GetComponent<Animator>().CrossFade(animationName, 0.1f);
        
        if(progressText != "")
        {
            GameObject barObj = Instantiate(interactBar, GameObject.Find("MainCanvas").transform); // Add progress bar for interaction status.
            barObj.transform.Find("InteractionText").GetComponent<TextMeshProUGUI>().text = progressText;
        }

        playerRef.GetComponent<Interaction>().interactedItem = this; // So that the base interaction can be disabled after interacted with.
    }

    public void togglePlayerMovement(bool enable)
    {
        MovementStateManager movementStateManager = playerRef.GetComponent<MovementStateManager>();
        movementStateManager.canMove = enable;
        playerRef.GetComponent<Interaction>().lockInteraction = !enable;
    }

    public abstract void onInteractPrimary();

    public abstract void onInteractSecondary();

    public abstract void onInteractionEnd();
}
