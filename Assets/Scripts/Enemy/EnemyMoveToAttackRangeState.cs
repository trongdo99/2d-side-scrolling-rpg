using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveToAttackRangeState : EnemyBattleState
{
    private Vector2 _directionToPlayer;
    private Player _player;

    public EnemyMoveToAttackRangeState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _player = _enemy.GetTargetedPlayer();

        Debug.Log("[Enemy] Enter move to attack range state");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        Vector2 playerPosition = _player.transform.position;
        _directionToPlayer = playerPosition - (Vector2)_enemy.transform.position;
        if (_directionToPlayer.magnitude < _enemy.AttackRange)
        {
            _stateMachine.ChangeToState(_enemy.attackState);
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
