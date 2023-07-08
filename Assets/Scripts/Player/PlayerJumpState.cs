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

        _controller.gravity = _player.NormalGravity;
        _controller.Velocity.y = _player.JumpForce;
        _maxHeightReached = _player.transform.position.y;
        _animator.Play("jump_swordmaster");
    }

    public override void OnUpdate()
    {
        _controller.Velocity.x = _inputVector.x * _player.moveSpeed;

        if (_maxHeightReached > _player.transform.position.y)
        {
            _stateMachine.ChangeToState(_player.fallState);
        }

        _maxHeightReached = Mathf.Max(_player.transform.position.y, _maxHeightReached);

        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
