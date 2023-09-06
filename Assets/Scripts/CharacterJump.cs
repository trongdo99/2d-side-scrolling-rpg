using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BanhMy.Tools;
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
	[SerializeField] private float _inputBufferDuration;

	private float _timeLastJumpButtonPressed;
	private float _timeSinceLastJumpPressed { get => Time.unscaledTime - _timeLastJumpButtonPressed; }
	private float _lastTimeGrounded;
	private bool _doubleJumping;

	private const string _jumpingAnimationParameterName = "Jumping";
	private const string _doubleJumpingAnimationParameterName = "DoubleJumping";
	private const string _hitTheGroundAnimationParameterName = "HitTheGround";
	private int _jumpingAnimationParameter;
	private int _doubleJumpingAnimationParameter;
	private int _hitTheGroundAnimationParameter;

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
			_timeLastJumpButtonPressed = Time.unscaledTime;
			Jump();
		}

		if (_inputBufferDuration > 0f && _controller.State.JustGotGrounded)
		{
			if (_timeSinceLastJumpPressed < _inputBufferDuration)
			{
				_numberOfJumpsLeft = _numberOfJumps;
				_doubleJumping = false;
				Jump();
			}
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

		if (!_controller.State.IsGrounded
			&& !EvaluateJumpTimeWindow()
			&& _numberOfJumpsLeft <= 0)
		{
				return;
		}

		_movementStateMachine.ChangeState(CharacterState.MovementState.Jumping);
		_conditionStateMachine.ChangeState(CharacterState.CharacterCondition.Normal);
		_controller.SetGravityActive(true);
		_controller.EnableCollision();

		_doubleJumping = _numberOfJumpsLeft != _numberOfJumps;

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
			_doubleJumping = false;
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
		if (EvaluateJumpTimeWindow()) return true;

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

    protected override void InitializeAnimatorParameters()
    {
		RegisterAnimatorParameter(_jumpingAnimationParameterName, AnimatorControllerParameterType.Bool, out _jumpingAnimationParameter);
		RegisterAnimatorParameter(_doubleJumpingAnimationParameterName, AnimatorControllerParameterType.Bool, out _doubleJumpingAnimationParameter);
		RegisterAnimatorParameter(_hitTheGroundAnimationParameterName, AnimatorControllerParameterType.Bool, out _hitTheGroundAnimationParameter);
    }

    public override void UpdateAnimator()
    {
		_animator.UpdateAnimatorBool(_jumpingAnimationParameter, _movementStateMachine.CurrentState == CharacterState.MovementState.Jumping, _character.AnimatorParameters, _character.PerformSanityCheck);
		_animator.UpdateAnimatorBool(_doubleJumpingAnimationParameter, _doubleJumping, _character.AnimatorParameters, _character.PerformSanityCheck);
		_animator.UpdateAnimatorBool(_hitTheGroundAnimationParameter, _controller.State.JustGotGrounded, _character.AnimatorParameters, _character.PerformSanityCheck);
    }
}
