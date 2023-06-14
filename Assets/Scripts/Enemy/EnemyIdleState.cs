using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(StateMachine stateMachine, Enemy enemy, Animator animator) : base(stateMachine, enemy, animator)
    {
    }

    public override void OnAnimationTriggered()
    {
        base.OnAnimationTriggered();
        _animator.Play("idle_NB");
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
