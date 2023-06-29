using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    public PlayerLedgeClimbState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnAnimationTriggered()
    {
        base.OnAnimationTriggered();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _controller.gravity = 0f;
        _player.transform.position += new Vector3(0.208f, -0.286f, 0f);
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
}
