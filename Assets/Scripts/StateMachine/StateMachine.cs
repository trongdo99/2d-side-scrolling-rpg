public class StateMachine
{
    private State _currentState;

    public State CurrentState { get => _currentState; private set => _currentState = value; }

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

    public void OnStateAnimationTriggered()
    {
        _currentState.OnAnimationTriggered();
    }

    public void OnStateAnimationCompleted()
    {
        _currentState.OnAnimtionCompleted();
    }
}
