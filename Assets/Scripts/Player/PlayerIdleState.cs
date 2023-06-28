using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
    public PlayerIdleState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _animator.Play("idle_swordmaster");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_inputVector.x != 0 && !_player.isBusy)
        {
            _controller.Velocity.x = 0f;
            _stateMachine.ChangeToState(_player.moveState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
