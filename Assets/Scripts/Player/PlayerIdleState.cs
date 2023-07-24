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
        _controller.SetForce(Vector2.zero);
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_inputVector.x != 0 && !_player.IsBusy)
        {
            _stateMachine.ChangeToState(_player.MoveState);
        }
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
