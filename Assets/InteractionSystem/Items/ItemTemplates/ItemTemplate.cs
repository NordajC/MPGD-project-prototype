using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

// Enum used to determine the type of item so it can be selected easily when new items are made
public enum Type
{
    Default,
    Consumable,
    Resource,
    Tool,
    Weaponry,
    Armour
}

// A class used so that the scriptable object can have crafting requirements. 
// An item can have a list of crafting items needed and the amount of that item needed.
[System.Serializable]
public class CraftableItem
{
    public ItemTemplate item;
    public int requiredAmount;
}

public abstract class ItemTemplate : ScriptableObject
{
    // All unique attributes the item can have
    [Header("Item data")]
    public int ItemId;
    public string ItemName;
    public Type ItemType;
    public Sprite ItemIcon;
    [TextArea(10,20)] public string ItemDescription;
    
    [Header("Quantity")]
    public bool IsStackable;
    public int maxItemCount;
    public bool canUse;

    [Header("Pickup")]
    public GameObject pickupPrefab;

    [Header("Crafting")]
    public CraftableItem[] craftingRequirements;

    // For items that will be seen visually in game like weapon and armour.
    public virtual Mesh getEquipMesh()
    {
        return null;
    }

    // To get attack/defence values.
    public virtual float getAttackValue()
    {
        return 0;
    }
    
    public virtual float getDefenceValue()
    {
        return 0;
    }
    
    // For interacting with the UI use button.
    public virtual void onItemUsed()
    {
    }
    
    // Get animation override for weapons.
    public virtual AnimatorOverrideController getEquipOverride()
    {
        return null;
    }

    // Different attack animations have different ranges so need different move to factors.
    public virtual float[] getAnimationMoveToMultipliers()
    {
        return null;
    }
}