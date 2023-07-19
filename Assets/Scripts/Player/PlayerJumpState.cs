using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
    private float _maxHeightReached;

    public PlayerJumpState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _controller.SetVerticalFoce(_player.JumpForce);
        _maxHeightReached = _player.transform.position.y;
        _animator.Play("jump_swordmaster");
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_controller.State.IsFalling)
        {
            _stateMachine.ChangeToState(_player.fallState);
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
