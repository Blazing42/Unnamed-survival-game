using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    //constructor
    public PlayerJumpingState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) 
    {
        IsRootState = true;
        InitialiseSubstate();
    }

    public override void EnterState()
    {
        Debug.Log("entering jump state");
        HandleJump();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        float groundedGravity = -0.05f;
        Ctx.AppliedMovementY = groundedGravity;
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequiresNewJumpPress = true;
        }
        
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        Debug.Log("exiting jump state");
    }

    public override void CheckSwitchState()
    {
        if (Ctx.CharController.isGrounded && Ctx.IsCrouchPressed)
        {
            SwitchStates(Factory.Crouched());
        }
        else if (Ctx.CharController.isGrounded)
        {
            SwitchStates(Factory.Grounded());
        }
    }

    public override void InitialiseSubstate()
    {
        if (!Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SetSubstate(Factory.Idle());
        }
        else if (Ctx.IsMovingPressed && !Ctx.IsSprintingPressed)
        {
            SetSubstate(Factory.Walk());
        }
        else
        {
            SetSubstate(Factory.Run());
        }
    }

    void HandleJump()
    {
        Ctx.IsJumping = true;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
    }

    void HandleGravity()
    {
        bool isFalling = Ctx.AppliedMovementY <= 0.0f;
        if (isFalling)
        {
            Ctx.AppliedMovementY += Mathf.Min(Ctx.Gravity * Ctx.FallMultiplier * Time.deltaTime, -20.0f);
        }
        else
        {
            Ctx.AppliedMovementY += Ctx.Gravity * Time.deltaTime;
        }
    }
}
