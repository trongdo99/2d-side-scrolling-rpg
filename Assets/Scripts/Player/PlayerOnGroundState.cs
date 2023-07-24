using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnGroundState : PlayerState
{
    public PlayerOnGroundState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void CheckCondition()
    {
        base.CheckCondition();

        if (GameInputManager.Instance.WasPrimaryAttackButtonPressed() && CanCombo())
        {
            _stateMachine.ChangeToState(_player.PrimaryAttackState);
        }
        else if (GameInputManager.Instance.WasDodgeButtonPressed() && CanRoll())
        {
            _stateMachine.ChangeToState(_player.RollState);
        }
        else if (GameInputManager.Instance.WasJumpButtonPressed())
        {
            _stateMachine.ChangeToState(_player.JumpState);
        }
        else if (_controller.State.IsFalling)
        {
            _stateMachine.ChangeToState(_player.FallState);
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

    private bool CanCombo()
    {
        return Time.time > _player.LastComboTime + _player.ComboDelay;
    }

    private bool CanRoll()
    {
        return Time.time > _player.LastRollTime + _player.RollCooldown;
    }
}
