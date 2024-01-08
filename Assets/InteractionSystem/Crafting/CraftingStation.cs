using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CraftingStation : BaseInteraction
{
    [Header("Crafting")]
    public GameObject craftScreen;
    public Transform craftCamera;
    public Transform moveToPosition;

    public AudioSource audioSource;
    public AudioClip[] craftSounds;
    
    public override void onInteractPrimary()
    {
        // Setting camera view to look at anvil.
        CinemachineVirtualCamera cinemachineVirtualCamera = GameObject.Find("CharacterCamera").GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = craftCamera;

        // Move player to face anvil.
        GameObject.FindWithTag("Player").GetComponent<MovementStateManager>().disableMovementScript();
        GameObject.FindWithTag("Player").GetComponent<Interaction>().lockInteraction = true;
        GameObject.FindWithTag("Player").GetComponent<Interaction>().interactedItem = this;

        GameObject.Find("PlayerMain").transform.position = moveToPosition.position;
        
        GameObject.Find("Player").transform.rotation = moveToPosition.rotation;
        
        // Call function to toggle crafting.
        PlayerInventory playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        playerInventory.OnToggleCrafting(craftScreen);
    }

    public override void onInteractSecondary()
    {
    }

    public override void onInteractionEnd()
    {
    }
}
