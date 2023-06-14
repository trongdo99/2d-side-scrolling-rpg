using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : State
{
    protected Enemy _enemy;
    protected Animator _animator;
    protected float _stateTimer;
    protected bool _isAnimationCompleted;

    public EnemyState(StateMachine stateMachine, Enemy enemy, Animator animator) : base(stateMachine)
    {
        _enemy = enemy;
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
        throw new System.NotImplementedException();
    }

    public override void OnAnimationTriggered()
    {
        _isAnimationCompleted = true;
    }
}
