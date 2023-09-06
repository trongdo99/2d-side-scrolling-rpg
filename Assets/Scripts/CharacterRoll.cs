using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoll : CharacterAbility
{
	[Header("Roll")]
	private float _rollDuration;
	private float _rollSpeed;
	private bool _blockHorizontalInput = true;
	private bool _preventDamageCollisionDuringRoll = true;

	[Header("CoolDown")]
	private float _rollCooldown = 1f;

	private float _coolDownTimeStamp = 0f;
	private Vector2 _rollDirection;
	private float _lastRollTime = 0f;
	private int _currentDirection;

	private const string _rollingAnimationParameterName = "Rolling";
	private const string _startedRollingAnimationParameterName = "StartedRolling";
	private int _rollAnimationParameter;
	private int _startedRollingAnimationParameter;

	protected override void HandleInput()
	{
		if (_inputManager.WasDodgeButtonPressed())
		{
			
		}
	}

	public override void ProcessAbility()
	{
		base.ProcessAbility();

	}

	private void Roll()
	{
		if (!RollAuthorized()) return;
		if (!EvalueateRollConditions()) return;

	}

	private bool RollAuthorized()
	{
		if (!AbilityAuthorized
			|| _controller.State.IsGrounded
			|| _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal)
		{
			return false;
		}
		return true;
	}

	private bool EvalueateRollConditions()
	{
		if (_coolDownTimeStamp > Time.time) return false;
		return true;
	}
}
