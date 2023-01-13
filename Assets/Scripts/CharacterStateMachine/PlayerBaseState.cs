
public abstract class PlayerBaseState
{
    protected PlayerStateMachine Ctx;
    protected PlayerStateFactory Factory;
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

    void UpdateStates() { }
    protected void SwitchStates(PlayerBaseState newState) 
    {
        //perform the exit functionality of this state
        ExitState();
        //perform the enter functionality of the new state
        newState.EnterState();
        //set the current state machine to the new state
        Ctx.CurrentState = newState;
    }
    protected void SetSuperstate() { }
    protected void SetSubstate() { }
}
