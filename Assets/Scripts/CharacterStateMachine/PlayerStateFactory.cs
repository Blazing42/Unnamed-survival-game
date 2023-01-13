
public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Grounded() 
    {
        return new PlayerGroundedState(_context,this);
    }
    public PlayerBaseState Idle() 
    {
        return new PlayerIdleState(_context,this);
    }
    public PlayerBaseState Walk() 
    {
        return new PlayerWalkState(_context, this);
    }
    public PlayerBaseState Run() 
    {
        return new PlayerRunState(_context, this);
    }
    public PlayerBaseState Jumping() 
    {
        return new PlayerJumpingState(_context, this);
    }
    public PlayerBaseState Injured() 
    {
        return new PlayerInjuredState(_context, this);
    }
    public PlayerBaseState Crouched() 
    {
        return new PlayerCrouchedState(_context, this);
    }

}
