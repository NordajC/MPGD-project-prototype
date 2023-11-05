using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InteractObject : MonoBehaviour
{
    [Header("Notify UI")]
    public GameObject notifyPrefab;
    public Transform notifyLocation;

    private bool isPlayerInRange;

    private Transform cameraRotation;

    public void Start()
    {
        //By default the notify UI is hidden.
        notifyPrefab.SetActive(false);
        cameraRotation = Camera.main.transform; //Get the camera transform so when visible the UI rotates to face the camera.
    }

    public void Update()
    {
        PlayerInventory playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();
        
        //If the UI is visible (player is in range), rotate it to face the camera and set can craft to true.
        if(notifyPrefab.active)
        {
            notifyPrefab.transform.LookAt(cameraRotation);
            notifyPrefab.transform.Rotate(0f, 180f, 0f);
            playerInventory.canCraft = true;
        } else {
            playerInventory.canCraft = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player")) //If player enters range, set the notify UI to enabled.
        {
            if(!isPlayerInRange)
            {
                isPlayerInRange = true;

                notifyPrefab.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if(isPlayerInRange) //If player exits range, set the notify UI to disabled.
        {
            isPlayerInRange = false;

            notifyPrefab.SetActive(false);
        }
    }
}
