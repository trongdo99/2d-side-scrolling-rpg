using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
	[Header("Permission")]
	public bool AbilityPermitted = true;
	[SerializeField] protected CharacterState.MovementState[] _blockingMovementStates;
	[SerializeField] protected CharacterState.CharacterCondition[] _blockingConditionStates;
	public virtual bool AbilityAuthorized
	{
		get
		{
			if (_character != null)
			{
				if (_blockingMovementStates != null && _blockingMovementStates.Length > 0)
				{
					foreach (var blockingState in _blockingMovementStates)
					{
						if (blockingState == _character.MovementStateMachine.CurrentState)
						{
							return false;
						}
					}
				}

				if (_blockingConditionStates != null && _blockingConditionStates.Length > 0)
				{
					foreach (var blockingState in _blockingConditionStates)
					{
						if (blockingState == _character.ConditionStateMachine.CurrentState)
						{
							return false;
						}
					}
				}
			}

			return AbilityPermitted;
		}
	}
	public bool AbilityInitialized { get { return _abilityInitialized; } }

	protected GameInputManager _inputManager;
	protected Character _character;
	protected Transform _characterTransform;
	protected CharacterController2D _controller;
	protected Animator _animator;
	protected SpriteRenderer _spriteRenderer;
	protected CharacterState _state;
	protected BMStateMachine<CharacterState.MovementState> _movementStateMachine;
	protected BMStateMachine<CharacterState.CharacterCondition> _conditionStateMachine;
	protected bool _abilityInitialized = false;
	protected float _verticalInput;
	protected float _horizontalInput;

	protected virtual void Start()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		_character = GetComponentInParent<Character>();
		_controller = GetComponentInParent<CharacterController2D>();
		_spriteRenderer = GetComponentInParent<SpriteRenderer>();
		BindAnimator();
		if (_character != null)
		{
			_characterTransform = _character.transform;
			_state = _character.CharacterState;
			_movementStateMachine = _character.MovementStateMachine;
			_conditionStateMachine = _character.ConditionStateMachine;
		}
		_abilityInitialized = true;
	}

	protected virtual void BindAnimator()
	{
		if (_character != null)
		{
			_animator = _character.Animator;
		}
		if (_animator != null)
		{
			InitializeAnimatorParameters();
		}
	}

	protected virtual void InitializeAnimatorParameters()
	{
		
	}

	public void SetInputManager(GameInputManager inputManager)
	{
		_inputManager = inputManager;
	}

	private void InternalHandleInput()
	{
		if (_inputManager == null) return;

		_horizontalInput = _inputManager.GetMovementVectorNormalized().x;
		_verticalInput = _inputManager.GetMovementVectorNormalized().y;

		HandleInput();
	}

	protected virtual void HandleInput()
	{
		
	}

	public virtual void EarlyProcessAbiltity()
	{
		InternalHandleInput();
	}

	public virtual void ProcessAbility()
	{
		
	}

	public virtual void LateProcessAbility()
	{
		
	}

	public virtual void UpdateAnimator()
	{
		
	}

	public virtual void Flip()
	{
		
	}

	public virtual void ResetAbility()
	{
		
	}
}
