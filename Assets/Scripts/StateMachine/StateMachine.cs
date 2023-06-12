public abstract class StateMachine
{
    protected State _currentState;

    public void Init(State startState)
    {
        _currentState = startState;
        _currentState.OnEnter();
    }

    public void ChangeStateTo(State nextState)
    {
        _currentState.OnExit();
        _currentState = nextState;
        _currentState.OnEnter();
    }

    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }
}
