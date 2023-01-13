using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    //constructor
    public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
    public override void EnterState()
    {
        Debug.Log("entering idle state");
        Ctx.Animator.SetBool(Ctx.IsSprintingHash, false);
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.AppliedMovementX = 0f;
        Ctx.AppliedMovementZ = 0f;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void ExitState()
    {
        Debug.Log("exiting idle state");
    }

    public override void CheckSwitchState()
    {
        if(Ctx.IsMovingPressed && Ctx.IsSprintingPressed && !Ctx.IsCrouchPressed)
        {
            SwitchStates(Factory.Run());
        }
        else if (Ctx.IsMovingPressed)
        {
            SwitchStates(Factory.Walk());
        }
    }

    public override void InitialiseSubstate(){}
}
