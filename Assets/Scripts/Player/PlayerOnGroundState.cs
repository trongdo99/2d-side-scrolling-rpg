using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnGroundState : PlayerState
{
    private float _lastYPosition;

    public PlayerOnGroundState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _lastYPosition = _player.transform.position.y;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GameInputManager.Instance.WasPrimaryAttackButtonPressed())
        {
            _stateMachine.ChangeToState(_player.primaryAttackState);
        }

        if (GameInputManager.Instance.WasRollButtonPressed() && _inputVector.x != 0)
        {
            _player.facingDirection = (int)_inputVector.x;
            _stateMachine.ChangeToState(_player.rollState);
        }

        if (GameInputManager.Instance.WasJumpButtonPressed())
        {
            _stateMachine.ChangeToState(_player.jumpState);
        }

        if (!_player.Controller.CollisionInfo.below && _lastYPosition > _player.transform.position.y)
        {
            _stateMachine.ChangeToState(_player.fallState);
        }

        _lastYPosition = Mathf.Max(_player.transform.position.y, _lastYPosition);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
