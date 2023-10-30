using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParentScript : MovementStateManager
{
    private MovementStateManager parentScript;
    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.GetComponent<MovementStateManager>();
    }

    
}
