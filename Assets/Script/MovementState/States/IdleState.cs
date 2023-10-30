using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {

    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(movement.dir.magnitude > 0.1f) 
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movement.SwitchState(movement.run);
            }
            else
            {
                movement.SwitchState(movement.walk);
            }
        }
        if (Input.GetKey(KeyCode.C))
        {
            movement.SwitchState(movement.crouch);
        }

        //if jump
        if(Input.GetKey(KeyCode.Space)) 
        {
            movement.previousState = this;
            movement.SwitchState(movement.jump);
        }
    }
}
