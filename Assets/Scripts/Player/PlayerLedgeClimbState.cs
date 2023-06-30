using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    Vector3 _offset = new Vector2(0.401f, 0f);
    public PlayerLedgeClimbState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _controller.gravity = 0f;
        _controller.Velocity.y = 0f;
        _player.transform.position = _controller.GetBottomLedgePosition(_player.facingDirection);
        _animator.Play("ledge_climb_swordmaster");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.Velocity.y = 0f;
    }

    public override void OnExit()
    {
        base.OnExit();

        _controller.gravity = CharacterController2D.GRAVITY;
    }

    public override void OnAnimtionCompleted()
    {
        base.OnAnimtionCompleted();

        _player.transform.position += _offset * _player.facingDirection;
        _stateMachine.ChangeToState(_player.idleState);
    }
}
