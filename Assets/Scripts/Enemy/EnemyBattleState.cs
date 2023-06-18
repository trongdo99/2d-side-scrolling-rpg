using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleState : EnemyState
{
    public EnemyBattleState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
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
