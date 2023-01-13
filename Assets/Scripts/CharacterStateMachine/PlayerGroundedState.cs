using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    //constructor
    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

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
        if (Ctx.IsJumpPressed)
        {
            SwitchStates(Factory.Jumping());
        }
    }

    public override void InitialiseSubstate()
    {
        throw new System.NotImplementedException();
    }
}
