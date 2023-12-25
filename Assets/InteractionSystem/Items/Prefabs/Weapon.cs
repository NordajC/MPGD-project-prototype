using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector3 equippedLocation;
    public Vector3 equippedRotation;
    public string secondaryEquipPoint;
    public Vector3 secondaryEquippedLocation;
    public Vector3 secondaryEquippedRotation;
    
    void Awake()
    {
        transform.localPosition = equippedLocation;
        transform.localEulerAngles = equippedRotation;
    }

    public void setWeaponSecondary(bool isUI)
    {
        string prefix = isUI ? "UI_" : "";
        transform.parent = GameObject.Find(prefix + secondaryEquipPoint).transform;
        transform.localPosition = secondaryEquippedLocation;
        transform.localEulerAngles = secondaryEquippedRotation;
    }
}
