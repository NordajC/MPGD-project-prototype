using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Crouching", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ExitState(movement, movement.run);
        }
        if (Input.GetKey(KeyCode.C))
        {
            if(movement.dir.magnitude < 0.1)
            {
                ExitState(movement, movement.idle);
            }
            else
            {
                ExitState(movement, movement.walk);
            }
        }

        if (movement.vInput < 0)
        {
            movement.currentMoveSpeed = movement.crouchBackSpeed;
        }
        else
        {
            movement.currentMoveSpeed = movement.crouchSpeed;
        }
    }
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Crouching", false);
        movement.SwitchState(state);

    }
}
