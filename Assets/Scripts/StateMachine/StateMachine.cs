public class StateMachine
{
    private State _currentState;

    public void Init(State startState)
    {
        _currentState = startState;
        _currentState.OnEnter();
    }

    public void ChangeToState(State nextState)
    {
        _currentState.OnExit();
        _currentState = nextState;
        _currentState.OnEnter();
    }

    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }

    public void OnStateAnimationTrigger()
    {
        _currentState.OnAnimationTriggered();
    }
}
