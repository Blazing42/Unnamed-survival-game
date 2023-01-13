using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInjuredState : PlayerBaseState
{
    //constructor
    public PlayerInjuredState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
    public override void EnterState()
    {
        Debug.Log("entering injured state");
    }

    public override void UpdateState()
    {
        CheckSwitchState();
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
