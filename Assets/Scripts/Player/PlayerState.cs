using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State
{
    protected Player _player;
    protected CharacterController2D _controller;
    protected Animator _animator;
    protected Vector2 _inputVector;
    protected float _stateTimer;
    protected bool _isAnimationCompletedTriggered;
    protected bool _isAnimationEventTriggered;

    protected PlayerState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine)
    {
        _player = player;
        _controller = controller;
        _animator = animator;
    }

    public override void OnEnter()
    {
        _isAnimationCompletedTriggered = false;
        _isAnimationCompletedTriggered = false;
    }

    public override void OnExit() { }

    public override void CheckCondition() { }

    public override void OnUpdate()
    {
        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
        _stateTimer -= Time.deltaTime;
    }

    public override void OnAnimationTriggered()
    {
        _isAnimationEventTriggered = true;
    }

    public override void OnAnimtionCompleted()
    {
        _isAnimationCompletedTriggered = true;
    }
}
