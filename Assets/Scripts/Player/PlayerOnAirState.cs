using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    private int _jumpOnAirCount;

    public PlayerOnAirState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _jumpOnAirCount = _player.CanDoubleJump ? 1 : 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _player.facingDirection = (int)_inputVector.x;

        if (_player.LedgeDetector.CheckForLedge(_player.facingDirection, out Vector2 ledgePosition))
        {
            _stateMachine.ChangeToState(_player.ledgeClimbState);
        }

        if (_player.CanDoubleJump &&
            GameInputManager.Instance.WasJumpButtonPressed() &&
            _jumpOnAirCount > 0)
        {
            _stateMachine.ChangeToState(_player.jumpState);
        }

        if (GameInputManager.Instance.WasDodgeButtonPressed()
            && CanDash()
            && _inputVector.x != 0f)
        {
            _stateMachine.ChangeToState(_player.dashState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _jumpOnAirCount = 0;
    }

    private bool CanDash()
    {
        return Time.time > _player.lastDashTime + _player.DashCooldown;
    }
}
