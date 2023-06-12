using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter Fall State");

        _player.SetFallingGravity();
        _animator.Play("jump_down_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _player.SetXVelocity(_inputVector.x * _player.moveSpeed);

        if (_player.Controller.CollisionInfo.below)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
