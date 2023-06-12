using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAirState : PlayerState
{
    public PlayerOnAirState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

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
