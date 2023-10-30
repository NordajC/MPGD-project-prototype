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
        else if (Input.GetKey(KeyCode.C))
        {
            ExitState(movement, movement.crouch);
        }
        else if (movement.dir.magnitude < 0.1) 
        {
            ExitState(movement, movement.idle);
        }

        if(movement.vInput < 0)
        {
            movement.currentMoveSpeed = movement.walkBackSpeed;
        }
        else
        {
            movement.currentMoveSpeed = movement.walkSpeed;
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
        movement.anim.SetBool("Walking", false);
        movement.SwitchState(state);

    }
}
