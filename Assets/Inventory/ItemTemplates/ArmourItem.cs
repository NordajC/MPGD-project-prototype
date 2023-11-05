using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum used to determine the type of armour so that it can be placed in the corresponding correct slot
public enum ArmourType
{
    Helmet,
    Vest,
    Gauntlet,
    Trousers,
    Boot
}

[CreateAssetMenu(fileName = "Armour Item", menuName = "Inventory/Item/Armour")]
public class ArmourItem : ItemTemplate
{
    public float DefenceFactor; //A factor by how much damage is reduced if this armour is currently equipped

    public ArmourType armourType;

    public void Awake()
    {
        ItemType = Type.Weaponry;
    }
    
}
