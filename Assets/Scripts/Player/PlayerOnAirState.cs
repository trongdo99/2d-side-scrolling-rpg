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

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (_inputVector.x > 0f)
        {
            _player.Face(Entity.FacingDirection.Right);
        }
        else if (_inputVector.x < 0f)
        {
            _player.Face(Entity.FacingDirection.Left);
        }

        if (_player.LedgeDetector.CheckForLedge(_player.CurrentFacingDirection, out Vector2 ledgePosition))
        {
            _stateMachine.ChangeToState(_player.ledgeClimbState);
        }
        else if (GameInputManager.Instance.WasDodgeButtonPressed()
            && CanDash()
            && _inputVector.x != 0f)
        {
            _stateMachine.ChangeToState(_player.dashState);
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

    private bool CanDash()
    {
        return Time.time > _player.lastDashTime + _player.DashCooldown;
    }
}
