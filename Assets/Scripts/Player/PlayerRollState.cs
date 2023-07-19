using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    private int _rollDirection;

    public PlayerRollState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _rollDirection = _player.CurrentFacingDirection;
        _stateTimer = _player.RollDuration;
        _animator.Play("roll_swordmaster");
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_stateTimer < 0f)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stateTimer > 0f)
        {
            _controller.SetHorizontalForce(_rollDirection * _player.RollSpeed);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _controller.SetHorizontalForce(0f);
        _player.lastRollTime = Time.time;
    }
}
