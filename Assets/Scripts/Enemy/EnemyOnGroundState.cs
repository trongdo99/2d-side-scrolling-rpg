using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnGroundState : EnemyState
{
    public EnemyOnGroundState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_enemy.GetTargetedPlayer())
        {
            _stateMachine.ChangeToState(_enemy.moveToAttackRangeState);
        }
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
