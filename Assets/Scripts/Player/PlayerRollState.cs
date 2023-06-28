using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _stateTimer = _player.RollDuration;
        //_animator.Play("roll_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stateTimer > 0f)
        {
            _controller.Velocity.x = _player.facingDirection * _player.RollSpeed;
        }
        else
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _player.lastRollTime = Time.time;
        _controller.Velocity.x = 0f;
    }
}
