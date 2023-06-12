using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
    public PlayerIdleState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Enter Idle State");
        _animator.Play("idle_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_inputVector.x != 0)
        {
            _stateMachine.ChangeToState(_player.moveState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
