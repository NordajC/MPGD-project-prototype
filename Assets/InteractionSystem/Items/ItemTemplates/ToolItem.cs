using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool Item", menuName = "Inventory/Item/Tool")]
public class ToolItem : ItemTemplate
{
    public void Awake()
    {
        ItemType = Type.Tool;
    }
}
