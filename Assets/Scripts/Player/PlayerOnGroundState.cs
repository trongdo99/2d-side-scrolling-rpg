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
            _stateMachine.ChangeToState(_player.primaryAttackState);
        }
        else if (GameInputManager.Instance.WasDodgeButtonPressed() && CanRoll())
        {
            _stateMachine.ChangeToState(_player.rollState);
        }
        else if (GameInputManager.Instance.WasJumpButtonPressed())
        {
            _stateMachine.ChangeToState(_player.jumpState);
        }
        else if (_controller.State.IsFalling)
        {
            _stateMachine.ChangeToState(_player.fallState);
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
        return Time.time > _player.lastComboTime + _player.ComboDelay;
    }

    private bool CanRoll()
    {
        return Time.time > _player.lastRollTime + _player.RollCooldown;
    }
}
