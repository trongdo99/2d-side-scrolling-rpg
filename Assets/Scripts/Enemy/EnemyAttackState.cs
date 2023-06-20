using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animator.Play("attack_SKL");
        Debug.Log("[Enemy] Enter attack state");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.Velocity.x = 0f;

        if (_isAnimationCompleted)
        {
            _stateMachine.ChangeToState(_enemy.battleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _enemy.lastAttackTime = Time.time;
    }
}
