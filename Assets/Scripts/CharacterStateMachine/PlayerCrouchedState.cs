using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchedState : PlayerBaseState
{
    //constructor
    public PlayerCrouchedState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

    public override void EnterState()
    {
        Debug.Log("entering crouched state");
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void CheckSwitchState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitialiseSubstate()
    {
        throw new System.NotImplementedException();
    }
}
