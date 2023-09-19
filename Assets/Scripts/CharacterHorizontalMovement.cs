using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterHorizontalMovement : CharacterAbility
{
	[Header("Speed")]
	[SerializeField] private float _moveSpeed;
	[SerializeField] private bool _flipCharacterToFaceDirection = true;
	[ReadOnly]
	public float AbilityMovementSpeedMultiplier = 1f;

	[FormerlySerializedAs("_readInput")]
	[Header("Input")]
	public bool ReadInput;
	[SerializeField] private float _inputThreshold = 0.1f;
	[Range(0f, 1f)]
	[SerializeField] private float _airControl = 1f;
	[SerializeField] private bool _allowFlipInTheAir = true;
	
	[Header("Wall")]
	[SerializeField] private bool _stopWalkingWHenCollidingWithAWall = false;

	public float MovementSpeed { get; set; }
	public bool FlipCharacterToFaceDirection => _flipCharacterToFaceDirection;
	
	private float _horizontalMovement;
	private float _lastGroundedHorizontalMovement;
	private float _normalizedHorizontalMovement;
	private float _lastTimeGrounded;

	private const string _speedAnimationParameterName = "Speed";
	private const string _movingAnimationParamterName = "Moving";
	private int _speedAnimationParameter;
	private int _movingAnimationParamter;

	protected override void Initialize()
	{
		base.Initialize();
		MovementSpeed = _moveSpeed;
	}

	public override void ProcessAbility()
	{
		base.ProcessAbility();
		HandleHorizontalMovement();
		DetectWall();
	}

	protected override void HandleInput()
	{
		if (!ReadInput) return;

		_horizontalMovement = _horizontalInput;
		
		if (_airControl < 1f && !_controller.State.IsGrounded)
		{
			_horizontalMovement = Mathf.Lerp(_lastGroundedHorizontalMovement, _horizontalInput, _airControl);
		}
	}

	public void SetAirControlDirection(float value)
	{
		_lastGroundedHorizontalMovement = value;
	}

	public void SetHorizontalMove(float value)
	{
		_horizontalMovement = value;
	}

	private void HandleHorizontalMovement()
	{
		if (!AbilityAuthorized
			|| _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal)
		{
			return;
		}

		CheckJustGotGrounded();
		StoreLastTimeGrounded();

		bool canFlip = true;

		if (!_controller.State.IsGrounded && !_allowFlipInTheAir)
		{
			canFlip = false;
		}

		if (_horizontalMovement > _inputThreshold)
		{
			_normalizedHorizontalMovement = _horizontalMovement;
			if (!_character.IsFacingRight && canFlip && _flipCharacterToFaceDirection)
			{
				_character.Flip();
			}
		}
		else if (_horizontalMovement < -_inputThreshold)
		{
			_normalizedHorizontalMovement = _horizontalMovement;
			if (_character.IsFacingRight && canFlip && _flipCharacterToFaceDirection)
			{
				_character.Flip();
			}
		}
		else
		{
			_normalizedHorizontalMovement = 0f;
		}

		if (_movementStateMachine.CurrentState == CharacterState.MovementState.Dashing) return;

		if (_controller.State.IsGrounded
			&& _normalizedHorizontalMovement != 0f
			&& (_movementStateMachine.CurrentState == CharacterState.MovementState.Idle
				|| _movementStateMachine.CurrentState == CharacterState.MovementState.Falling))
		{
			_movementStateMachine.ChangeState(CharacterState.MovementState.Moving);
		}

		if (_controller.State.IsGrounded
			&& _movementStateMachine.CurrentState == CharacterState.MovementState.Jumping
			&& _controller.TimeAirborne >= _character.AirborneMinimumtime)
		{
			_movementStateMachine.ChangeState(CharacterState.MovementState.Idle);
		}

		if (_movementStateMachine.CurrentState == CharacterState.MovementState.Moving
			&& _normalizedHorizontalMovement == 0f)
		{
			_movementStateMachine.ChangeState(CharacterState.MovementState.Idle);
		}

		if (_controller.State.IsFalling
			&& (_movementStateMachine.CurrentState == CharacterState.MovementState.Moving
				|| _movementStateMachine.CurrentState == CharacterState.MovementState.Idle))
		{
			_movementStateMachine.ChangeState(CharacterState.MovementState.Falling);
		}

		float movementSpeed = _normalizedHorizontalMovement * MovementSpeed * AbilityMovementSpeedMultiplier;
		_controller.SetHorizontalForce(movementSpeed);
		
		if (_controller.State.IsGrounded)
		{
			_lastGroundedHorizontalMovement = _horizontalMovement;
		}
	}

	private void DetectWall()
	{
		if (!_stopWalkingWHenCollidingWithAWall) return;
		
		if (_movementStateMachine.CurrentState == CharacterState.MovementState.Moving)
		{
			if (_controller.State.IsCollidingLeft || _controller.State.IsCollidingRight)
			{
				_movementStateMachine.ChangeState(CharacterState.MovementState.Idle);
			}
		}
	}

	private void CheckJustGotGrounded()
	{
		if (_controller.State.JustGotGrounded)
		{
			if (_movementStateMachine.CurrentState != CharacterState.MovementState.Jumping
				&& _movementStateMachine.CurrentState != CharacterState.MovementState.Rolling
				&& _movementStateMachine.CurrentState != CharacterState.MovementState.Dashing)
			{
				_movementStateMachine.ChangeState(CharacterState.MovementState.Idle);
			}
		}
	}
	
	private void StoreLastTimeGrounded()
	{
		if (_controller.State.IsGrounded)
		{
			_lastTimeGrounded = Time.time;
		}
	}

	protected override void InitializeAnimatorParameters()
	{
		RegisterAnimatorParameter(_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
		RegisterAnimatorParameter(_movingAnimationParamterName, AnimatorControllerParameterType.Bool, out _movingAnimationParamter);
	}

	public override void UpdateAnimator()
	{
		_animator.UpdateAnimatorFloat(_speedAnimationParameter, Mathf.Abs(_normalizedHorizontalMovement), _character.AnimatorParameters, _character.PerformSanityCheck);
		_animator.UpdateAnimatorBool(_movingAnimationParamter, _movementStateMachine.CurrentState == CharacterState.MovementState.Moving, _character.AnimatorParameters, _character.PerformSanityCheck);
	}
}
