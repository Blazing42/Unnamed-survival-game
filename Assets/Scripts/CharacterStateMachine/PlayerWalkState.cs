using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    //constructor
    public PlayerWalkState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) {}
    public override void EnterState()
    {
        Debug.Log("entering walk state");
        Ctx.Animator.SetBool(Ctx.IsSprintingHash, false);
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        Ctx.AppliedMovementX = Ctx.CurrentMoveX;
        Ctx.AppliedMovementZ = Ctx.CurrentMoveZ;
    }

    public override void ExitState()
    {
        Debug.Log("exiting walk state");
    }

    public override void CheckSwitchState()
    {
        if (!Ctx.IsMovingPressed)
        {
            SwitchStates(Factory.Idle());
        } 
        else if(Ctx.IsSprintingPressed && Ctx.IsMovingPressed)
        {
            SwitchStates(Factory.Run());
        }
    }

    public override void InitialiseSubstate(){}
}
