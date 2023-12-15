using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParentScript : MovementStateManager
{
    private MovementStateManager parentScript;

    void Start()
    {
        parentScript = transform.parent.GetComponent<MovementStateManager>();
    }
}
