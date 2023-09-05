using System;
using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
using UnityEditor.MPE;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum CharacterType { Player, AI }
	public enum FacingDirection { Left, Right }
	public enum SpawnFacingDirection { Default, Left, Right }

	[SerializeField] private CharacterType _characterType = CharacterType.AI;
	public CharacterState CharacterState { get; private set; }

	[Header("Direction")]
	[SerializeField] private FacingDirection _initialFacingDirection = FacingDirection.Right;
	[SerializeField] private SpawnFacingDirection _directionOnSpawn = SpawnFacingDirection.Default;
	public bool IsFacingRight { get; set; }
	public bool CanFlip { get; set; }
	
	[Header("Animator")]
	[SerializeField] private Animator _animator;
	[SerializeField] private bool _performSanityCheck;
	[SerializeField] private bool _disableAnimatorLog;
	
	public Animator Animator { get => _animator; }

	[Header("Model")]
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private GameObject _cameraTarget;
	[SerializeField] private float _cameraTargetSpeed = 5f;
	[SerializeField] private bool _flipModelOnDirectionChange = true;

	[Header("Abilities")]
	[SerializeField] private GameObject _abilityNode;
	
	[Header("Airborne")]
	[SerializeField] private float _airborneDistance = 0.5f;
	public float AirborneMinimumtime = 0.1f;
	public bool Airborne 
	{
		get 
		{
			return _controller.DistanceToTheGround > _airborneDistance || _controller.DistanceToTheGround == -1;
		}
	}

	public GameInputManager InputManager { get; private set; }
	public BMStateMachine<CharacterState.MovementState> MovementStateMachine { get; private set; }
	public BMStateMachine<CharacterState.CharacterCondition> ConditionStateMachine { get; private set; }
	public HashSet<int> _animatorParameters { get; private set; }

	private const string _groundedAnimationParameterName = "Grounded";
	private const string _airborneAnimationParameterName = "Airborne";
	private const string _xSpeedAnimationParameterName = "xSpeed";
	private const string _ySpeedAnimationParamterName = "ySpeed";
	private const string _worldXSpeedAnimationParamterName = "worldXSpeed";
	private const string _worldYSpeedAnimationParameterName = "worldYSpeed";
	private const string _idleAnimationParameterName = "Idle";
	private const string _aliveAnimationParameterName = "Alive";
	private const string _facingRightAnimationParameterName = "FacingRight";
	private const string _flipAnimationParameterName = "Flip";
	
	private int _groundedAnimationParameter;
	private int _airborneAnimationParamter;
	private int _xSpeedAnimationParamter;
	private int _ySpeedAnimationParamter;
	private int _worldXSpeedAnimationParamter;
	private int _worldYSpeedAnimationParameter;
	private int _idleAnimationParameter;
	private int _aliveAnimationParameter;
	private int _facingRightAnimationParameter;
	private int _flipAnimationParameter;
	
	private CharacterController2D _controller;
	private CharacterAbility[] _characterAbilities;
	private bool _abilitiesCachedOnce = false;
	private Vector3 _cameraTargetInitialPosition;

	private void Awake()
	{
		MovementStateMachine = new BMStateMachine<CharacterState.MovementState>();
		ConditionStateMachine = new BMStateMachine<CharacterState.CharacterCondition>();
		MovementStateMachine.ChangeState(CharacterState.MovementState.Idle);

		IsFacingRight = _initialFacingDirection == FacingDirection.Right;

		if (_cameraTarget == null)
		{
			_cameraTarget = new GameObject();
			_cameraTarget.transform.SetParent(transform);
			_cameraTarget.transform.localPosition = Vector3.zero;
			_cameraTarget.name = "CameraTarget";
		}
		_cameraTargetInitialPosition = _cameraTarget.transform.localPosition;

		CharacterState = new CharacterState();
		_controller = GetComponent<CharacterController2D>();
		CachedAbilities();
		SetInputManger();
		BindAnimator();
		CanFlip = true;
	}

	private void CachedAbilities()
	{
		if (_abilitiesCachedOnce) return;

		if (_abilityNode != null)
		{
			_characterAbilities = _abilityNode.GetComponentsInChildren<CharacterAbility>();
		}
		_abilitiesCachedOnce = true;
	}

	public T FindAbility<T>() where T : CharacterAbility
	{
		CachedAbilities();

		Type searchedAbilityType = typeof(T);

		foreach (var ability in _characterAbilities)
		{
			if (ability is T characterAbility)
			{
				return characterAbility;
			}
		}

		return null;
	}

	private void SetInputManger()
	{
		if (_characterType == CharacterType.AI)
		{
			InputManager = null;
			UpdateInputManagerInAbilities();
			return;
		}

		InputManager = GameInputManager.Instance;
		UpdateInputManagerInAbilities();
	}

	private void UpdateInputManagerInAbilities()
	{
		if (_characterAbilities == null) return;

		foreach (var ability in _characterAbilities)
		{
			ability.SetInputManager(InputManager);
		}
	}

	private void BindAnimator()
	{
		if (_animator != null)
		{
			InitializeAnimatorParameters();
			_animator.logWarnings = _disableAnimatorLog;
		}
	}

	private void InitializeAnimatorParameters()
	{
		_animatorParameters = new HashSet<int>();
		
		_animator.AddAnimatorParameterIfExists(_groundedAnimationParameterName, out _groundedAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_airborneAnimationParameterName, out _airborneAnimationParamter, AnimatorControllerParameterType.Bool, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_xSpeedAnimationParameterName, out _xSpeedAnimationParamter, AnimatorControllerParameterType.Float, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_ySpeedAnimationParamterName, out _ySpeedAnimationParamter, AnimatorControllerParameterType.Float, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_worldXSpeedAnimationParamterName, out _worldXSpeedAnimationParamter, AnimatorControllerParameterType.Float, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_worldYSpeedAnimationParameterName, out _worldYSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_idleAnimationParameterName, out _idleAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_aliveAnimationParameterName, out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_facingRightAnimationParameterName, out _facingRightAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
		_animator.AddAnimatorParameterIfExists(_flipAnimationParameterName, out _flipAnimationParameter, AnimatorControllerParameterType.Trigger, _animatorParameters);
	}

	private void UpdateAnimator()
	{
		if (_animator == null) return;

		_animator.UpdateAnimatorBool(_groundedAnimationParameter, _controller.State.IsGrounded, _animatorParameters, _performSanityCheck);
		_animator.UpdateAnimatorBool(_airborneAnimationParamter, Airborne, _animatorParameters, _performSanityCheck);
		_animator.UpdateAnimatorBool(_idleAnimationParameter, MovementStateMachine.CurrentState == CharacterState.MovementState.Idle, _animatorParameters, _performSanityCheck);
		_animator.UpdateAnimatorBool(_facingRightAnimationParameter, IsFacingRight, _animatorParameters, _performSanityCheck);
	}

	private void Update()
	{
		EarlyProcessAbilities();

		if (Time.timeScale != 0)
		{
			ProcessAbilities();
			LateProcessAbilities();
		}

	}

	private void EarlyProcessAbilities()
	{
		foreach (var ability in _characterAbilities)
		{
			if (ability.enabled && ability.AbilityInitialized)
			{
				ability.EarlyProcessAbiltity();
			}
		}
	}

	private void ProcessAbilities()
	{
		foreach (var ability in _characterAbilities)
		{
			if (ability.enabled && ability.AbilityInitialized)
			{
				ability.ProcessAbility();
			}
		}
	}

	private void LateProcessAbilities()
	{
		foreach (var ability in _characterAbilities)
		{
			if (ability.enabled && ability.AbilityInitialized)
			{
				ability.ProcessAbility();
			}
		}
	}

	public void Flip(bool ignoreFlipOnDirectionChagngeFalseValue = false)
	{
		if (!_flipModelOnDirectionChange && !ignoreFlipOnDirectionChagngeFalseValue) return;

		if (!CanFlip) return;

		if (!_flipModelOnDirectionChange && ignoreFlipOnDirectionChagngeFalseValue)
		{
			_spriteRenderer.flipX = !_spriteRenderer.flipX;
		}

		FlipModel();

		IsFacingRight = !IsFacingRight;

		foreach (var ability in _characterAbilities)
		{
			if (ability.enabled)
			{
				ability.Flip();
			}
		}
	}

	public void FlipModel()
	{
		if (_flipModelOnDirectionChange)
		{
			_spriteRenderer.flipX = !_spriteRenderer.flipX;
		}
	}
}
