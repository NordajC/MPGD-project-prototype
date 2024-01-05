using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Item", menuName = "Inventory/Item/Default")]
public class DefaultItem : ItemTemplate
{
    [Header("Animations")]
    public float[] animationMoveToMultipliers;
    
    public void Awake()
    {
        ItemType = Type.Default;
    }
}
