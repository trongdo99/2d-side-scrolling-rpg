using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
using UnityEngine;

public class CharacterRoll : CharacterAbility
{
	[Header("Roll")]
	[SerializeField] private float _rollDuration;
	[SerializeField] private float _rollSpeed;
	[SerializeField] private bool _blockHorizontalInput = true;
	[SerializeField] private bool _preventDamageCollisionDuringRoll = true;

	[Header("CoolDown")]
	[SerializeField] private float _rollCooldown = 1f;

	private IEnumerator _rollCoroutine;
	private float _coolDownTimeStamp = 0f;
	private Vector2 _rollDirection;
	private float _lastRollTime = 0f;
	private float _currentDirection;
	private float _drivenInput;
	private float _originalMultiplier = 1f;

	private const string _rollingAnimationParameterName = "Rolling";
	private const string _startedRollingAnimationParameterName = "StartedRolling";
	private int _rollingAnimationParameter;
	private int _startedRollingAnimationParameter;

	protected override void HandleInput()
	{
		if (_inputManager.WasDodgeButtonPressed())
		{
			Roll();
		}
	}

	public override void ProcessAbility()
	{
		base.ProcessAbility();

	}

	private void Roll()
	{
		if (!RollAuthorized()) return;
		if (!EvaluateRollConditions()) return;

		InitiateRoll();
	}

	private bool RollAuthorized()
	{
		if (!AbilityAuthorized
			|| !_controller.State.IsGrounded
			|| _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal)
		{
			return false;
		}
		return true;
	}

	private bool EvaluateRollConditions()
	{
		if (_coolDownTimeStamp > Time.time) return false;
		return true;
	}

	private void InitiateRoll()
	{
		_movementStateMachine.ChangeState(CharacterState.MovementState.Rolling);

		_coolDownTimeStamp = Time.time + _rollCooldown;
		_lastRollTime = Time.time;
		_originalMultiplier = _characterHorizontalMovement.AbilityMovementSpeedMultiplier;

		if (_preventDamageCollisionDuringRoll)
		{
			// TODO: Disable damage to health
		}

		ComputeRollDirection();

		StartCoroutine(RollCoroutine());
	}

	private void ComputeRollDirection()
	{
		_rollDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
		_currentDirection = _rollDirection.x > 0f ? 1 : -1;
	}

	private IEnumerator RollCoroutine()
	{
		if (!AbilityAuthorized
		    || _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal)
		{
			yield break;
		}

		_characterHorizontalMovement.ReadInput = false;
		_characterHorizontalMovement.AbilityMovementSpeedMultiplier = _rollSpeed;

		if (_animator != null)
		{
			_animator.UpdateAnimatorTrigger(_startedRollingAnimationParameter, _character.AnimatorParameters, _character.PerformSanityCheck);
		}

		float rollStartAt = Time.time;

		while (Time.time - rollStartAt < _rollDuration
		       && _movementStateMachine.CurrentState == CharacterState.MovementState.Rolling)
		{
			if (!_blockHorizontalInput)
			{
				_drivenInput = _horizontalInput;
			}

			if (_drivenInput != 0f)
			{
				_currentDirection = _drivenInput < 0f ? -1f : 1f;
			}
			
			_characterHorizontalMovement.SetHorizontalMove(_currentDirection);

			yield return null;
		}
		
		StopRoll();
	}

	public void StopRoll()
	{
		_characterHorizontalMovement.ReadInput = true;
		_characterHorizontalMovement.AbilityMovementSpeedMultiplier = _originalMultiplier;

		if (_preventDamageCollisionDuringRoll)
		{
			// TODO: enable damage
		}

		if (_rollCoroutine != null)
		{
			StopCoroutine(_rollCoroutine);
		}

		if (_movementStateMachine.CurrentState == CharacterState.MovementState.Rolling)
		{
			if (_controller.State.IsGrounded)
			{
				_movementStateMachine.ChangeState(CharacterState.MovementState.Idle);
			}
			else
			{
				_movementStateMachine.RestorePreviousState();
			}
		}
	}

	protected override void InitializeAnimatorParameters()
	{
		RegisterAnimatorParameter(_rollingAnimationParameterName, AnimatorControllerParameterType.Bool, out _rollingAnimationParameter);
		RegisterAnimatorParameter(_startedRollingAnimationParameterName, AnimatorControllerParameterType.Trigger, out _startedRollingAnimationParameter);
	}

	public override void UpdateAnimator()
	{
		_animator.UpdateAnimatorBool(_rollingAnimationParameter,
			_movementStateMachine.CurrentState == CharacterState.MovementState.Rolling, _character.AnimatorParameters,
			_character.PerformSanityCheck);
	}

	public override void ResetAbility()
	{
		base.ResetAbility();
		if (_conditionStateMachine.CurrentState == CharacterState.CharacterCondition.Normal)
		{
			StopRoll();
		}

		_animator.UpdateAnimatorBool(_rollingAnimationParameter, false, _character.AnimatorParameters,
			_character.PerformSanityCheck);
	}
}
