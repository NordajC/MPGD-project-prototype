using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField]
    public ToolItem toolData;

    public float GetDamage()
    {
        if (toolData != null)
        {
            return toolData.getAttackValue();
        }
        return 0;
    }
}
