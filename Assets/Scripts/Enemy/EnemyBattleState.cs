using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleState : EnemyState
{
    private int _directionToPlayer;
    private Player _player;

    public EnemyBattleState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        _animator.Play("idle_SKL");
        Debug.Log("[Enemy] Eneter battle state");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_enemy.IsPlayerDetected())
        {
            _stateTimer = _enemy.BattleTime;

            if (_player.transform.position.x > _enemy.transform.position.x)
            {
                _directionToPlayer = 1;
            }
            else if (_player.transform.position.x < _enemy.transform.position.x)
            {
                _directionToPlayer = -1;
            }

            if (_enemy.facingDirection != _directionToPlayer)
            {
                _enemy.facingDirection = _directionToPlayer;
            }

            float distanceToPlayer = Vector2.Distance(_player.transform.position, _enemy.transform.position);

            if (distanceToPlayer > _enemy.AttackRange)
            {
                _animator.Play("walk_SKL");
                _controller.SetHorizontalForce(_directionToPlayer * _enemy.MoveSpeed);
            }
            else
            {
                _animator.Play("idle_SKL");
                _controller.SetHorizontalForce(0f);
            }

            if (distanceToPlayer < _enemy.AttackRange && CanAttack())
            {
                _stateMachine.ChangeToState(_enemy.attackState);
            }
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(_player.transform.position, _enemy.transform.position);
            if (_stateTimer < 0 || distanceToPlayer > _enemy.LoseAgroDistance)
            {
                _stateMachine.ChangeToState(_enemy.idleState);
            }
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

    private bool CanAttack()
    {
        if (Time.time > _enemy.lastAttackTime + _enemy.AttackCooldown)
        {
            return true;
        }

        return false;
    }
}
