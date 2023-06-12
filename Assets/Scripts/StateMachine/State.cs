using System.Collections;
using System.Collections.Generic;

public abstract class State
{
    protected StateMachine _stateMachine;

    public void OnEnter() { }
    public void OnUpdate() { }
    public void OnExit() { }
}
