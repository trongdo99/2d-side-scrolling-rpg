using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyOnGroundState
{
    public EnemyIdleState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnAnimationTriggered()
    {
        base.OnAnimationTriggered();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animator.Play("idle_SKL");
        _stateTimer = _enemy.IdleTime;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.SetHorizontalForce(0f);

        if (_stateTimer < 0f)
        {
            _stateMachine.ChangeToState(_enemy.moveState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
