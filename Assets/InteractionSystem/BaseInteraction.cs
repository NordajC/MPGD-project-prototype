using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour, IInteractable
{
    [Header("Defaults")]
    [TextArea(3,10)] public string interactText;
    [HideInInspector] public Outline outline;

    private void Awake()
    {
        // If interact item has outline effect, then disable it by default.
        outline = GetComponent<Outline>();
        if(outline != null)
            outline.enabled = false;
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

    public abstract void onInteractPrimary();

    public abstract void onInteractSecondary();
}
