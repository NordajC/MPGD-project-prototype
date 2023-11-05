using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum used to determine the type of weapon so that it can be placed in the corresponding correct slot
public enum WeaponType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "Weaponry Item", menuName = "Inventory/Item/Weaponry")]
public class WeaponryItem : ItemTemplate
{
    //Min and max damage variables used so there is chance of critical hit and not the same damage amount each hit
    public int minDamageAmount;
    public int maxDamageAmount;

    public WeaponType weaponType;

    public void Awake()
    {
        ItemType = Type.Weaponry;
    }
    
}
