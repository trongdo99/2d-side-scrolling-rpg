using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : State
{
    protected Enemy _enemy;
    protected CharacterController2D _controller;
    protected Animator _animator;
    protected float _stateTimer;
    protected bool _isAnimationCompleted;

    public EnemyState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine)
    {
        _enemy = enemy;
        _controller = controller; ;
        _animator = animator;
    }

    public override void OnEnter()
    {
        _isAnimationCompleted = false;
    }

    public override void OnUpdate()
    {
        _stateTimer -= Time.deltaTime;
    }

    public override void OnExit()
    {
    }

    public override void OnAnimationTriggered()
    {
        _isAnimationCompleted = true;
    }

    public override void CheckCondition()
    {
        throw new System.NotImplementedException();
    }
}
