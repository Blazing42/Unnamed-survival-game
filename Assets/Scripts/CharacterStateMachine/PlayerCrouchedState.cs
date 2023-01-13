using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchedState : PlayerBaseState
{
    //constructor
    public PlayerCrouchedState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) 
    {
        IsRootState = true;
        InitialiseSubstate();
    }

    public override void EnterState()
    {
        Debug.Log("entering crouched state");
        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, true);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void ExitState()
    {
        Debug.Log("exiting crouched state");
        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, false);
    }

    public override void CheckSwitchState()
    {
        if (!Ctx.IsCrouchPressed)
        {
            SwitchStates(Factory.Grounded());
        }
        else if (Ctx.IsJumpPressed && !Ctx.RequiresNewJumpPress)
        {
            SwitchStates(Factory.Jumping());
        }
    }

    public override void InitialiseSubstate()
    {
        if (!Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SetSubstate(Factory.Idle());
        }
        else
        {
            SetSubstate(Factory.Walk());
        }
    }
}
