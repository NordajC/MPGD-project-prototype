using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum used to determine the type of weapon so that it can be placed in the corresponding correct slot.
public enum WeaponType
{
    Melee,
    Ranged,
    Shield
}

[CreateAssetMenu(fileName = "Weaponry Item", menuName = "Inventory/Item/Weaponry")]
public class WeaponryItem : ItemTemplate
{
    [Header("Weapon type")]
    // Min and max damage variables used so there is chance of critical hit and not the same damage amount each hit.
    public WeaponType weaponType;
    public GameObject equipWeapon;
    public int minDamageAmount;
    public int maxDamageAmount;
    public float defenceFactor; // For shield.

    public void Awake()
    {
        ItemType = Type.Weaponry;
    }

    public override void onItemUsed()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().equipUnequipItem();
    } 
}
