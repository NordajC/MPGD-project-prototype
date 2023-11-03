using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Item", menuName = "Inventory/Item/Resource")]
public class ResourceItem : ItemTemplate
{
    public void Awake()
    {
        ItemType = Type.Resource;
    }
    
}
