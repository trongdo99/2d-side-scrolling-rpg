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
        _animator.Play("run_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_inputVector.x == 0)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }

        _controller.Velocity = new Vector2(_inputVector.x * _player.moveSpeed, _controller.Velocity.y);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
