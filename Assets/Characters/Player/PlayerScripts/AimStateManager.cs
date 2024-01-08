using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimStateManager : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] private float mouseSense = 1;
    float xAxis, yAxis;
    [SerializeField] private Transform camFollowPos;
    public Crosshair crosshair;
    private float weight;

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSense;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSense;

        xAxis += mouseX;
        yAxis -= mouseY;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        // Set IK weight (for spine rotation) based on if aiming or not.
        MovementStateManager movementStateManager = GetComponent<MovementStateManager>();
        float targetWeight = movementStateManager.rotationMode == RotationMode.Aiming ? 1 : 0;
        weight = Mathf.MoveTowards(weight, targetWeight, 4f * Time.deltaTime);
        crosshair.updateOpacity(weight);
    }

    public void LateUpdate()
    {
        // Rotates the cam follow target so the player camera moves with the mouse.
        camFollowPos.localEulerAngles = new Vector3(yAxis, xAxis, camFollowPos.localEulerAngles.z);
    }
    
    void OnAnimatorIK()
    {
        GetComponent<Animator>().SetLookAtWeight(weight, weight); // Update IK weight.
        Vector3 targetPos = GameObject.Find("IKCube").transform.position;
        targetPos += gameObject.transform.right * 2; // Offset added for better aligning.
        targetPos += -gameObject.transform.up * 2;
        GetComponent<Animator>().SetLookAtPosition(targetPos); // Set the look at position so the IK rotates.
    }
}