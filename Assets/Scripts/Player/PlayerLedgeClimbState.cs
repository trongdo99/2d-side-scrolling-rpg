using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    Vector3 _offset = new Vector2(0.401f, 0f);
    public PlayerLedgeClimbState(StateMachine stateMachine, Player player, CharacterController2D controller, Animator animator) : base(stateMachine, player, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _controller.gravity = 0f;
        _controller.Velocity = Vector2.zero;
        _player.transform.position = _controller.GetBottomLedgePosition(_player.facingDirection);
        _animator.Play("ledge_climb_swordmaster");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.Velocity = Vector2.zero;
    }

    public override void OnExit()
    {
        base.OnExit();

        // Delay setting position when exit this state for 1 frame in order to let the next state play it's animation.
        // If not, the sprite will blink forward and backward.
        _player.StartCoroutine(SetPositionAfterClimb(_player.facingDirection));
        _controller.gravity = CharacterController2D.GRAVITY;
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
