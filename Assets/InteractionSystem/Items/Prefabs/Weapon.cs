using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Defaults")]
    public Vector3 equippedLocation;
    public Vector3 equippedRotation;
    public string secondaryEquipPoint;
    public Vector3 secondaryEquippedLocation;
    public Vector3 secondaryEquippedRotation;
    
    void Awake()
    {
        // Set default local position so item is in hand.
        transform.localPosition = equippedLocation;
        transform.localEulerAngles = equippedRotation;
    }

    public void setWeaponSecondary(bool isUI)
    {
        // Set position of item if it a secondary weapon.
        string prefix = isUI ? "UI_" : "";
        transform.parent = GameObject.Find(prefix + secondaryEquipPoint).transform;
        transform.localPosition = secondaryEquippedLocation;
        transform.localEulerAngles = secondaryEquippedRotation;
    }
}
