using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int _comboCounter;
    private int _combotInputBufferCounter;
    private float _lastAttackTime;
    private Vector2 _attackVelocity;

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

        float attackDirection = _player.CurrentFacingDirection;
        _attackVelocity.x = attackDirection * _player.AttackMovement[_comboCounter].x;
        _attackVelocity.y = _player.AttackMovement[_comboCounter].y;
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_isAnimationCompletedTriggered)
        {
            if (_comboCounter < 2 && _combotInputBufferCounter > -1 && _controller.State.IsGrounded)
            {
                _combotInputBufferCounter = -1;
                _stateMachine.ChangeToState(_player.PrimaryAttackState);
            }
            else
            {
                _stateMachine.ChangeToState(_player.IdleState);
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.SetForce(_attackVelocity);

        if (GameInputManager.Instance.WasPrimaryAttackButtonPressed())
        {
            _combotInputBufferCounter = _player.ComboInputBufferFrame;
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
        if (_comboCounter > 2)
        {
            _player.LastComboTime = Time.time;
        }
        _lastAttackTime = Time.time;
        _controller.SetHorizontalForce(0f);
        _player.StartCoroutine(_player.IsBusyFor(0.15f));
    }
}
