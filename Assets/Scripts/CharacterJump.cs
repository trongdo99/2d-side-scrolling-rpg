using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : CharacterAbility
{
	public enum JumpRestriction
	{
		CanJumpOnGround,
		CanJumpAnyWhere
	}

	[Header("Jump")]
	[SerializeField] private int _numberOfJumps = 2;
	[ReadOnly]
	[SerializeField] private int _numberOfJumpsLeft;
	[SerializeField] private float _maxJumpHeight = 2f;
	[SerializeField] private float _timeToReachApexHeight;
	[ReadOnly]
	[SerializeField] private float _jumpForce;
	[SerializeField] private JumpRestriction _jumpRestriction = JumpRestriction.CanJumpAnyWhere;
	[SerializeField] private bool _canJumpDownOnWayPlatform = true;
	
	[Header("Quality of Life")]
	[SerializeField] private float _coyoteTime;
	[SerializeField] private int _inputBufferFrame;


	private float _lastTimeGrounded;

	protected override void Initialize()
	{
		base.Initialize();
		_controller.SetOverrideGravity(-2 * _maxJumpHeight / Mathf.Pow(_timeToReachApexHeight, 2));
		_jumpForce = 2 * _maxJumpHeight / _timeToReachApexHeight;
	}

	protected override void HandleInput()
	{
		base.HandleInput();
		if (_inputManager.WasJumpButtonPressed())
		{
			Jump();
		}
	}

	private void Jump()
	{
		if (!AbilityAuthorized
			|| !EvaluateJumpRestriction()
			|| _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal
			|| _movementStateMachine.CurrentState == CharacterState.MovementState.Dashing)
		{
			return;
		}

		_movementStateMachine.ChangeState(CharacterState.MovementState.Jumping);
		_conditionStateMachine.ChangeState(CharacterState.CharacterCondition.Normal);
		_controller.SetGravityActive(true);
		_controller.EnableCollision();

		_numberOfJumpsLeft -= 1;
		
		if (_controller.State.IsGrounded)
		{
			_controller.TimeAirborne = 0f;
		}

		_controller.SetVerticalFoce(_jumpForce);
	}

	public override void ProcessAbility()
	{
		base.ProcessAbility();

		if (!AbilityAuthorized) return;

		if (_controller.State.JustGotGrounded)
		{
			_numberOfJumpsLeft = _numberOfJumps;
		}

		if (_controller.State.IsGrounded)
		{
			_lastTimeGrounded = Time.time;
		}

		_controller.State.IsJumping = _movementStateMachine.CurrentState == CharacterState.MovementState.Jumping;
	}

	private bool EvaluateJumpRestriction()
	{
		if (_jumpRestriction == JumpRestriction.CanJumpAnyWhere) return true;
		
		if (_jumpRestriction == JumpRestriction.CanJumpOnGround)
		{
			if (_controller.State.IsGrounded)
			{
				return true;
			}
			else if (_numberOfJumpsLeft < _numberOfJumps)
			{
				return true;
			}
		}

		return false;
	}

	private bool EvaluateJumpTimeWindow()
	{
		if (_movementStateMachine.CurrentState == CharacterState.MovementState.Jumping
			|| _movementStateMachine.CurrentState == CharacterState.MovementState.DoubleJumping)
		{
			return false;
		}

		if (Time.time - _lastTimeGrounded <= _coyoteTime)
		{
			return true;
		}
		return false;
	}
}
