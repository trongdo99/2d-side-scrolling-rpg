using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnGroundState : PlayerState
{
    public PlayerOnGroundState(StateMachine stateMachine, Player player, Animator animator) : base(stateMachine, player, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GameInputManager.Instance.WasJumpPressed())
        {
            _stateMachine.ChangeToState(_player.jumpState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
