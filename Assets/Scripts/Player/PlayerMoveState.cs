using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerOnGroundState
{
    public PlayerMoveState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _animator.Play("run_swordmaster");
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_inputVector.x == 0)
        {
            _stateMachine.ChangeToState(_player.IdleState);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.SetHorizontalForce(_inputVector.x * _player.MoveSpeed);
        if (_inputVector.x > 0f)
        {
            _player.Face(Entity.FacingDirection.Right);
        }
        else if (_inputVector.x < 0f)
        {
            _player.Face(Entity.FacingDirection.Left);
        }

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
