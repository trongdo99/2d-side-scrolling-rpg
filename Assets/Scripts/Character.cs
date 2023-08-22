using System;
using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
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
	public Animator Animator { get => _animator; }

	[Header("Model")]
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private GameObject _cameraTarget;
	[SerializeField] private float _cameraTargetSpeed = 5f;

	[Header("Abilities")]
	[SerializeField] private GameObject _abilityNode;
	
	[Header("Airborne")]
	[SerializeField] private float _airborneDistance = 0.5f;
	[SerializeField] private float _airborneMinimumTime = 0.1f;
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
		_controller = new CharacterController2D();
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
		}
	}

	private void InitializeAnimatorParameters()
	{
		
	}
}
