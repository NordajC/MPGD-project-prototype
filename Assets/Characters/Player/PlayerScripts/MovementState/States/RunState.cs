using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            ExitState(movement, movement.walk);
        }
        else if (movement.dir.magnitude < 0.1) 
        {
            ExitState(movement, movement.idle);
        }

        if (movement.verticalInput < 0 && movement.rotationMode == RotationMode.Aiming)
        {
            movement.currentMoveSpeed = movement.runBackSpeed;
        }
        else
        {
            movement.currentMoveSpeed = movement.runSpeed;
        }
        //if jump
        if (Input.GetKey(KeyCode.Space))
        {
            movement.previousState = this;
            ExitState(movement, movement.jump);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Running", false);
        movement.SwitchState(state);

    }
}
