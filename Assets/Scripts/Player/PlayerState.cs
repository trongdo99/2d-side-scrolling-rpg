using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State
{
    protected Player _player;
    protected Animator _animator;
    protected Vector2 _inputVector;
    protected float _stateTimer;
    protected bool _isAnimationCompletedTriggered;

    protected PlayerState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine)
    {
        _player = player;
        _animator = animator;
    }

    public override void OnEnter()
    {
        _isAnimationCompletedTriggered = false;
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
        _stateTimer -= Time.deltaTime;
    }

    public void OnAnimationCompleted()
    {
        _isAnimationCompletedTriggered = true;
    }
}
