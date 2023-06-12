using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
    public PlayerJumpState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter Jump State");

        _player.SetYVelocity(_player.JumpForce);
        _animator.Play("jump_up_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
