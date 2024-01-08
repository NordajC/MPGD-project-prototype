using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface so different interactable items can have different functionality using the same methods.
public interface IInteractable
{
    void onInteractPrimary();

    void onInteractSecondary();
}