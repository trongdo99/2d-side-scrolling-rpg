using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerOnGroundState
{
    public PlayerMoveState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _animator.Play("run_assasin");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_inputVector.x == 0)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }

        _controller.Velocity.x = _inputVector.x * _player.moveSpeed;
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
