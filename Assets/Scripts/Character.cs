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

	[Header("Model")]
	[SerializeField] private SpriteRenderer _spriteRenderer ;
	[SerializeField] private GameObject _cameraTarget;
	[SerializeField] private float _cameraTargetSpeed = 5f;

	[Header("Abilities")]
	[SerializeField] private List<GameObject> _abilityNodes;
	
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

	public BMStateMachine<CharacterState.MovementState> _movementStateMachine { get; private set; }
	public BMStateMachine<CharacterState.CharacterCondition> _conditionStateMachine { get; private set; }
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
}
