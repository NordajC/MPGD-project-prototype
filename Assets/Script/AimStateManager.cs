using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimStateManager : MonoBehaviour
{
    [SerializeField] private float mouseSense = 1;
    [SerializeField] private float deadZone = 0.1f; // Dead zone for mouse input
    float xAxis, yAxis;
    [SerializeField] private Transform camFollowPos;

    public Animator animator;
    private float turnAmount;

    [SerializeField] private MovementStateManager movementStateManager; // Reference to MovementStateManager

    // Start is called before the first frame update
    void Start()
    {
        IsCharacterMoving();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSense;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSense;

        // Implement a dead zone for mouse input
        if (Mathf.Abs(mouseX) < deadZone)
            mouseX = 0;
        if (Mathf.Abs(mouseY) < deadZone)
            mouseY = 0;

        xAxis += mouseX;
        yAxis -= mouseY;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        if (!IsCharacterMoving()) // Check if the character is moving
        {
            float targetAngle = Mathf.Atan2(camFollowPos.forward.x, camFollowPos.forward.z) * Mathf.Rad2Deg;
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

            // Only consider significant rotation for turning
            if (Mathf.Abs(angleDifference) > deadZone)
            {
                turnAmount = Mathf.Clamp(angleDifference / 180.0f, -1.0f, 1.0f);
                animator.SetFloat("Turn", turnAmount);
            }
        }
        else
        {
            animator.SetFloat("Turn", 0); // Reset turn amount when moving
        }
    }

    public void LateUpdate()
    {
        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    private bool IsCharacterMoving()
    {
        // Check if the character's current move speed is above a small threshold
        return movementStateManager.currentMoveSpeed > 0.01f;
    }
}
