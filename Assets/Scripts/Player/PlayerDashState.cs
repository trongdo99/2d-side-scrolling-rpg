using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int _dashDirection;

    public PlayerDashState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _stateTimer = _player.DashDuration;
        _controller.gravity = 0f;
        _dashDirection = _player.facingDirection;
        _animator.Play("dash_swordmaster");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_player.LedgeDetector.CheckForLedge(_dashDirection, out Vector2 ledgePosition))
        {
            _stateMachine.ChangeToState(_player.ledgeClimbState);
        }

        if (_stateTimer > -0f)
        {
            _controller.Velocity.x = _dashDirection * _player.DashSpeed;
            _controller.Velocity.y = 0f;
        }
        else
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _player.lastDashTime = Time.time;
        _controller.gravity = CharacterController2D.GRAVITY;
    }
}
