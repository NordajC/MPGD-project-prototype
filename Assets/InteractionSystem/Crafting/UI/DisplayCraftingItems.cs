using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCraftingItems : MonoBehaviour
{
    [Header("Crafting items")]
    public GameObject craftingItemPrefab;
    public ItemTemplate[] items;

    void Start()
    {
        // To add all craftable items of the category each item is iterated through and then a slot is spawned.
        foreach(ItemTemplate item in items)
        {
            var spawnedTemplate = Instantiate(craftingItemPrefab, transform);
            spawnedTemplate.GetComponent<SetCraftableInfo>().setInfo(item); // To set visuals of the slot.
        }
    }
}
