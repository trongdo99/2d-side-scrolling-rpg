using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerOnAirState
{
    public PlayerFallState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animator.Play("fall_swordmaster");
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_player.Controller.State.IsGrounded)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.SetHorizontalForce(_inputVector.x * _player.moveSpeed);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
