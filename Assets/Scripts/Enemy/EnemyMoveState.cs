using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyOnGroundState
{
    public EnemyMoveState(StateMachine stateMachine, Enemy enemy, CharacterController2D controller, Animator animator) : base(stateMachine, enemy, controller, animator)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animator.Play("walk_SKL");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //if (_controller.CollisionInfo.left || _controller.CollisionInfo.right ||
        //    (_enemy.facingDirection == -1 && !_controller.CollisionInfo.leftBottomEdge) ||
        //    (_enemy.facingDirection == 1 && !_controller.CollisionInfo.rightBottomEdge))
        //{
        //    _enemy.facingDirection = -_enemy.facingDirection;
        //    _stateMachine.ChangeToState(_enemy.idleState);
        //}
        _controller.SetForce(Vector2.right * _enemy.facingDirection * _enemy.MoveSpeed);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnAnimationTriggered()
    {
        base.OnAnimationTriggered();
    }
}
