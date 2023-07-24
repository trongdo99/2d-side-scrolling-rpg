using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector3 _offset = new Vector2(0.401f, 0f);
    private int _facingDirection;
    public PlayerLedgeClimbState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _facingDirection = _player.CurrentFacingDirection;
        _player.LedgeDetector.CheckForLedge(_facingDirection, out Vector2 ledgePosition);
        _player.transform.position = ledgePosition;
        _controller.SetForce(Vector2.zero);
        _controller.SetGravityActive(false);
        _animator.Play("ledge_climb_swordmaster");
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_isAnimationCompletedTriggered)
        {
            _stateMachine.ChangeToState(_player.idleState);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();

        _player.transform.position += _offset * _facingDirection;
        _controller.SetGravityActive(true);
    }
}
