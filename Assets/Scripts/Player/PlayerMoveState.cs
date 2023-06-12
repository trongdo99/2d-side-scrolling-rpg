using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerOnGroundState
{
    public PlayerMoveState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Enter Move State");
        _animator.Play("run_FK");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_inputVector.x == 0)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }

        _player.SetXVelocity(_inputVector.x * _player.moveSpeed);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
