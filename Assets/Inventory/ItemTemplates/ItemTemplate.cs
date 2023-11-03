using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

//Enum used to determine the type of item so it can be selected easily when new items are made
public enum Type
{
    Default,
    Consumable,
    Resource,
    Weaponry,
    Armour
}

public abstract class ItemTemplate : ScriptableObject
{
    //All uniue attributes the item can have
    public int ItemId;
    public string ItemName;
    public Type ItemType;
    public Texture ItemIcon;
    [TextArea(10,20)] public string ItemDescription;
    public bool IsStackable;
    public int maxItemCount;
    public bool canUse;

    [HideInInspector] public int inventorySlot;

    [HideInInspector] public int itemAmount = 1;

    public GameObject pickupPrefab;
}
