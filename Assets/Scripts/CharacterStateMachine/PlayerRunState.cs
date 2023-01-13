using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    //constructor
    public PlayerRunState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
    public override void EnterState()
    {
        Debug.Log("entering run state");
        Ctx.Animator.SetBool(Ctx.IsSprintingHash, true);
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        Ctx.AppliedMovementX = Ctx.CurrentRunMoveX;
        Ctx.AppliedMovementZ = Ctx.CurrentRunMoveZ;
    }

    public override void ExitState()
    {
        Debug.Log("exiting run state");
    }

    public override void CheckSwitchState()
    {
        if (!Ctx.IsMovingPressed)
        {
            SwitchStates(Factory.Idle());
        }
        else if(Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SwitchStates(Factory.Walk());
        }
        else if (Ctx.IsCrouchPressed)
        {
            SwitchStates(Factory.Walk());
        }
    }
    

    public override void InitialiseSubstate(){}
}
