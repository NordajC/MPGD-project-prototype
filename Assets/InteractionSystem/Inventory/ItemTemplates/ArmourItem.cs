using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum used to determine the type of armour so that it can be placed in the corresponding correct slot
public enum ArmourType
{
    Helmet,
    Vest,
    Gauntlets,
    Trousers,
    Boots
}

[CreateAssetMenu(fileName = "Armour Item", menuName = "Inventory/Item/Armour")]
public class ArmourItem : ItemTemplate
{
    [Header("Armour type")]
    public ArmourType armourType;
    public Mesh armourMesh;
    public float defenceFactor; // A factor by how much damage is reduced if this armour is currently equipped
    public float attackFactor; // So that armour can deal extra damage (for gauntlets)
    

    public void Awake()
    {
        ItemType = Type.Armour;
    }

    public override Mesh getEquipMesh()
    {
        return armourMesh;
    }

    public override float getAttackValue()
    {
        return attackFactor;
    }

    public override float getDefenceValue()
    {
        return defenceFactor;
    }

    public override void onItemUsed()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().equipUnequipItem();
    }
}
