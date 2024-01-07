using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool Item", menuName = "Inventory/Item/Tool")]
public class ToolItem : ItemTemplate
{
    public float minDamageAmount;
    public float maxDamageAmount;

    public void Awake()
    {
        ItemType = Type.Tool;
    }

    public override float getAttackValue()
    {
        return Random.Range(minDamageAmount, maxDamageAmount);
    }
}
