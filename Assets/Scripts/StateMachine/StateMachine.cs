public abstract class StateMachine
{
    public State CurrentState { get; private set; }

    public void Init(State startState)
    {
        CurrentState = startState;
        CurrentState.OnEnter();
    }

    public void ChangeToState(State nextState)
    {
        CurrentState.OnExit();
        CurrentState = nextState;
        CurrentState.OnEnter();
    }

    public void OnUpdate()
    {
        CurrentState.OnUpdate();
    }
}
