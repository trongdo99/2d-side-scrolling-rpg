using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int _comboCounter;
    private float _lastAttackTime;

    public PlayerPrimaryAttackState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_comboCounter > 2 || Time.time - _lastAttackTime > _player.ComboWindow)
        {
            _comboCounter = 0;
        }

        switch (_comboCounter)
        {
            case 0:
                _animator.Play("attack_1_FK");
                break;
            case 1:
                _animator.Play("attack_2_FK");
                break;
            case 2:
                _animator.Play("attack_3_FK");
                break;
        }

        float attackDirection = _player.facingDirection;
        if (_inputVector.x != 0)
        {
            attackDirection = _inputVector.x;
        }

        _player.SetVelocity(new Vector2(attackDirection * _player.AttackMovement[_comboCounter].x, _player.AttackMovement[_comboCounter].y));

        _stateTimer = 0.2f;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stateTimer < 0)
        {
            _player.SetXVelocity(0f);
        }

        if (_isAnimationCompletedTriggered)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _comboCounter++;
        _lastAttackTime = Time.time;
        _player.SetXVelocity(0f);
        _player.StartCoroutine(_player.isBusyFor(0.15f));
    }
}
