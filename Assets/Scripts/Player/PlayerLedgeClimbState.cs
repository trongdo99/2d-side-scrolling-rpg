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

        _facingDirection = _player.facingDirection;
        _player.LedgeDetector.CheckForLedge(_facingDirection, out Vector2 ledgePosition);
        _player.transform.position = ledgePosition;
        _controller.SetForce(Vector2.zero);
        _controller.SetGravityActive(false);
        _animator.Play("ledge_climb_swordmaster");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();

        // Delay setting position when exit this state for 1 frame in order to let the next state play it's animation.
        // If not, the sprite will blink forward and backward.
        _player.StartCoroutine(SetPositionAfterClimb(_facingDirection));
        _controller.SetGravityActive(true);
    }

    public override void OnAnimtionCompleted()
    {
        base.OnAnimtionCompleted();

        _stateMachine.ChangeToState(_player.idleState);
    }

    private IEnumerator SetPositionAfterClimb(int facingDirection)
    {
        yield return null;
        _player.transform.position += _offset * facingDirection;
    }
}
