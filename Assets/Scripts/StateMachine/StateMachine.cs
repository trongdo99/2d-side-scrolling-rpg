using System.Collections;
using System.Collections.Generic;

public abstract class StateMachine
{
    protected State _currentState;

    public void ChangeStateTo(State nextState) { }

    public void Init(State startState)
    {
        _currentState = startState;
        _currentState.OnEnter();
    }
}
