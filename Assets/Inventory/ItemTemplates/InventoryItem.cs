using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    //Uses a separate object to include the item amount so we can update it dynaically in game as it should not update the amount in the scriptable object (ItemTemplate)
    public ItemTemplate itemTemplate;
    public int itemAmount = 0;
}
