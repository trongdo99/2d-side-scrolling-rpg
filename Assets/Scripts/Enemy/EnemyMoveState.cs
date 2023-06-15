using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(StateMachine stateMachine, Enemy enemy, Animator animator) : base(stateMachine, enemy, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animator.Play("walk_SKL");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_enemy.Controller.CollisionInfo.left || _enemy.Controller.CollisionInfo.right)
        {
            _enemy.ChangeFacingDirection();
            _stateMachine.ChangeToState(_enemy.idleState);
        }
        _enemy.SetXVelocity(_enemy.facingDirection * _enemy.MoveSpeed);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnAnimationTriggered()
    {
        base.OnAnimationTriggered();
    }
}
