using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    //constructor
    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) 
    {
        IsRootState = true;
        InitialiseSubstate();
    }

    public override void EnterState()
    {
        Debug.Log("grounded state entered");
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void ExitState()
    {
        Debug.Log("grounded state exited");
    }

    public override void CheckSwitchState()
    {
        //if jump is pressed change to the jump state
        if (Ctx.IsJumpPressed && !Ctx.RequiresNewJumpPress)
        {
            SwitchStates(Factory.Jumping());
        }
        else if (Ctx.IsCrouchPressed)
        {
            SwitchStates(Factory.Crouched());
        }
    }

    public override void InitialiseSubstate()
    {
        if(!Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SetSubstate(Factory.Idle());
        }
        else if(Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SetSubstate(Factory.Walk());
        }
        else
        {
            SetSubstate(Factory.Run());
        }
    }
}
