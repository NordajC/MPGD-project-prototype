using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        if (movement.previousState == movement.idle)
        {
            movement.anim.SetTrigger("IdleJump");
        }
        else if (movement.previousState == movement.walk || movement.previousState == movement.run)
        {
            movement.anim.SetTrigger("RunJump");
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(movement.jumped && movement.IsGrounded())
        {
            movement.jumped = false;
            if (movement.hzInput == 0 && movement.vInput == 0)
            {
                movement.SwitchState(movement.idle);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                movement.SwitchState(movement.run);
            }
            else movement.SwitchState(movement.walk);
        }
    }
}
