using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    //Variables used to select the target item in the prefab variant and a check variable so it is ensured that only 1 of this item can be picked up.
    public ItemTemplate item;
    private bool pickedUp = false;
    
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (pickedUp) return; //If the object for some reason has not been destroyed, then it will not readd the item to the inventory until the game object destroy.

        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInventory>().AddToInventory(item); //Calls the function to add to inventory with the parameter being the item variable we select.
            pickedUp = true;
            Destroy(gameObject);
        }
        
    }
}
