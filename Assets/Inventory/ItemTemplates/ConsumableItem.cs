using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Inventory/Item/Consumable")]
public class ConsumableItem : ItemTemplate
{
    //Random minimum and max values for the health and hunger amount to restore
    public float MinRestoreHealth;
    public float MaxRestoreHealth;
    public float MinRestoreHunger;
    public float MaxRestoreHunger;

    public void Awake()
    {
        ItemType = Type.Consumable;
    }
    
}
