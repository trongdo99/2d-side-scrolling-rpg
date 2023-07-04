using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int _comboCounter;
    private int _combotInputBufferCounter;
    private float _lastAttackTime;

    public PlayerPrimaryAttackState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
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
                _animator.Play("attack_1_swordmaster");
                break;
            case 1:
                _animator.Play("attack_2_swordmaster");
                break;
            case 2:
                _animator.Play("attack_3_swordmaster");
                break;
        }

        float attackDirection = _player.facingDirection;
        if (_inputVector.x != 0)
        {
            attackDirection = _inputVector.x;
        }

        _controller.Velocity.x = attackDirection * _player.AttackMovement[_comboCounter].x;
        _controller.Velocity.y = _player.AttackMovement[_comboCounter].y;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GameInputManager.Instance.WasPrimaryAttackButtonPressed())
        {
            _combotInputBufferCounter = _player.ComboInputBufferFrame;
        }

        if (_isAnimationCompletedTriggered && _combotInputBufferCounter > -1)
        {
            _combotInputBufferCounter = -1;
            _stateMachine.ChangeToState(_player.primaryAttackState);
        }
        else if (_isAnimationCompletedTriggered)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }

        if (_combotInputBufferCounter > -1)
        {
            _combotInputBufferCounter -= 1;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _comboCounter++;
        _lastAttackTime = Time.time;
        _controller.Velocity.x = 0f;
        _player.StartCoroutine(_player.isBusyFor(0.15f));
    }
}
