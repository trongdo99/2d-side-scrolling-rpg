using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    public PlayerOnAirState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _player.facingDirection = (int)_inputVector.x;

        if (_controller.CanGrapLedge())
        {
            _stateMachine.ChangeToState(_player.ledgeClimbState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
