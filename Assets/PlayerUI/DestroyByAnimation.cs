using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByAnimation : MonoBehaviour
{
    // Called in animation events to destroy object.
    public void destroyObject()
    {
        Destroy(gameObject);
    }
}
