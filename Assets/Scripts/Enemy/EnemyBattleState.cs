using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleState : EnemyState
{
    public EnemyBattleState(StateMachine stateMachine, Enemy enemy, Animator animator) : base(stateMachine, enemy, animator)
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
