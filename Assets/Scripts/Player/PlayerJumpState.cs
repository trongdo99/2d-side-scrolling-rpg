using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float _maxHeightReached;

    public PlayerJumpState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter Jump State");

        _player.SetNormalGravity();
        _player.SetYVelocity(_player.JumpForce);
        _maxHeightReached = _player.transform.position.y;
        _animator.Play("jump_up_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _player.SetXVelocity(_inputVector.x * _player.moveSpeed);

        if (_maxHeightReached > _player.transform.position.y)
        {
            _stateMachine.ChangeToState(_player.fallState);
        }

        _maxHeightReached = Mathf.Max(_player.transform.position.y, _maxHeightReached);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
