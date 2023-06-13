public abstract class State
{
    protected StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
    public virtual void OnAnimationTriggered() { }
}
