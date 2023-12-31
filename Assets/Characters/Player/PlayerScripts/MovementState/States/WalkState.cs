using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(Input.GetKey(KeyCode.LeftShift)) 
        {
            ExitState(movement, movement.run);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ExitState(movement, movement.crouch);
        }
        else if (movement.dir.magnitude < 0.1) 
        {
            ExitState(movement, movement.idle);
        }

        if(movement.vInput < 0 && movement.rotationMode == RotationMode.Aiming)
        {
            movement.currentMoveSpeed = movement.walkBackSpeed;
        }
        else
        {
            movement.currentMoveSpeed = movement.walkSpeed;
        }

        UpdateMagnitude(movement);

        //if jump
        if (Input.GetButtonDown("Jump"))
        {
            movement.previousState = this;
            ExitState(movement, movement.jump);
        }
    }
    void UpdateMagnitude(MovementStateManager movement)
    {
        float magnitude = Mathf.Abs(movement.dir.magnitude);
        magnitude = Mathf.Clamp(magnitude, 0f, 1f); // Ensures the magnitude is within the 0-1 range
        movement.anim.SetFloat("magnitude", magnitude); // Sets the magnitude parameter in the Animator
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Walking", false);
        movement.SwitchState(state);

    }
}
