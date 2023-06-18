using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _controller.gravity = _player.FallingGravity;
        _animator.Play("jump_down_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.Velocity = new Vector2(_inputVector.x * _player.moveSpeed, _controller.Velocity.y);

        if (_player.Controller.CollisionInfo.below)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.gravity = _player.NormalGravity;
    }
}
