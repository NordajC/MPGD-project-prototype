using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Portal : BaseInteraction
{
    public int neededId;

    public override void onInteractPrimary()
    {
        PlayerInventory playerInventory = playerRef.GetComponent<PlayerInventory>();
        int? hasItem = playerInventory.getItemSlotIndex(neededId, false);

        if(hasItem != null)
        {
            SceneManager.LoadScene("GameWon");
        }
    }

    public override void onInteractSecondary()
    {
    }
    
    public override void onInteractionEnd()
    {

    }
}
