using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimStateManager : MonoBehaviour
{
    [SerializeField] private float mouseSense = 1;
    float xAxis, yAxis;
    [SerializeField] private Transform camFollowPos;

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSense;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSense;

        xAxis += mouseX;
        yAxis -= mouseY;
        yAxis = Mathf.Clamp(yAxis, -80, 80);
    }

    public void LateUpdate()
    {
        // Rotates the cam follow target so the player camera moves with the mouse.
        camFollowPos.localEulerAngles = new Vector3(yAxis, xAxis, camFollowPos.localEulerAngles.z);
    }
}
