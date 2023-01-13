
public abstract class PlayerBaseState
{
    protected bool IsRootState = false;
    protected PlayerStateMachine Ctx;
    protected PlayerStateFactory Factory;
    protected PlayerBaseState CurrentSuperState;
    protected PlayerBaseState CurrentSubState;
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory currentStateFactory)
    {
        Ctx = currentContext;
        Factory = currentStateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitialiseSubstate();

    public void UpdateStates() 
    {
        UpdateState();
        if(CurrentSubState != null)
        {
            CurrentSubState.UpdateState();
        }
    }

    protected void SwitchStates(PlayerBaseState newState) 
    {
        //perform the exit functionality of this state
        ExitState();
        //perform the enter functionality of the new state
        newState.EnterState();
        //set the current state machine to the new state
        if (IsRootState)
        {
            Ctx.CurrentState = newState;
        }
        else if(CurrentSuperState != null)
        {
            CurrentSuperState.SetSubstate(newState);
        }
        
    }
    protected void SetSuperstate(PlayerBaseState newSuperState) 
    {
        CurrentSuperState = newSuperState;
    }
    protected void SetSubstate(PlayerBaseState newSubState) 
    {
        CurrentSubState = newSubState;
        newSubState.SetSuperstate(this);
    }
}
