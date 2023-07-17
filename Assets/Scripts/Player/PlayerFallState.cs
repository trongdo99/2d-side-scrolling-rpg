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

    public override void OnUpdate()
    {
        _controller.SetHorizontalForce(_inputVector.x * _player.moveSpeed);

        if (_player.Controller.State.IsGrounded)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }

        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
