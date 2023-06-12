using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    [Header("Attack settings")]
    [SerializeField] private float _comboWindow;

    [Header("Move settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _accelerationTimeGrounded;
    [SerializeField] private float _accelerationTimeAirborne;


    [Header("Jump settings")]
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _timeToJumpApex;
    [SerializeField] private float _fallGravityMultiplier;

    [Header("Roll settings")]
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;

    private CharacterController2D _controller;

    // Movement variables
    private Vector2 _inputVector;
    private Vector2 _velocity;
    private Vector2 _previousVelocity;
    private float _velocityXSmoothing;

    // Jump variables
    private float _gravity;
    private float _normalGravity;
    private float _fallingGravity;
    private float _jumpForce;
    private float _maxHeightReached = Mathf.NegativeInfinity;
    private bool _isApexReached = false;

    // Roll variables
    private float _rollTimer;
    private int _rollDirection;

    // Visual variables
    private bool _isFacingRight = true;

    // Attack variables
    private bool _isAttacking;
    private int _comboCounter = 1;
    private float _comboWindowTimer;

    // State Machine
    private PlayerStateMachine _stateMachine;
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }

    public float moveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float JumpForce { get => _jumpForce; private set => _jumpForce = value; }
    public CharacterController2D Controller { get => _controller; private set => _controller = value; }

    // Debug variable;
    private float _startJumpHeight;

    private void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(_stateMachine, this, _animator);
        moveState = new PlayerMoveState(_stateMachine, this, _animator);
        jumpState = new PlayerJumpState(_stateMachine, this, _animator);

        _stateMachine.Init(idleState);

        _controller = GetComponent<CharacterController2D>();

        _normalGravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _fallingGravity = _normalGravity * _fallGravityMultiplier;
        _gravity = _normalGravity;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {_gravity}, Jump Force: {_jumpForce}");
    }

    private void Start()
    {
        GameInputManager.Instance.OnJumpActionPerformed += GameInputManager_OnJumpActionPerformed;
        GameInputManager.Instance.OnJumpActionCaceled += GameInputManager_OnJumpActionCaceled;
        GameInputManager.Instance.OnPrimaryAttackPerformed += GameInputManager_OnPrimaryAttackPerformed;
        GameInputManager.Instance.OnRollActionPerformed += GameInputManager_OnRollActionPerformed;
    }

    private void OnDestroy()
    {
        if (GameInputManager.Instance)
        {
            GameInputManager.Instance.OnJumpActionPerformed -= GameInputManager_OnJumpActionPerformed;
            GameInputManager.Instance.OnJumpActionCaceled -= GameInputManager_OnJumpActionCaceled;
            GameInputManager.Instance.OnPrimaryAttackPerformed -= GameInputManager_OnPrimaryAttackPerformed;
            GameInputManager.Instance.OnRollActionPerformed -= GameInputManager_OnRollActionPerformed;
        }
    }

    private void Update()
    {
        _previousVelocity = _velocity;
        _velocity.y += _gravity * Time.deltaTime;

        _stateMachine.OnUpdate();

        Vector2 deltaPosition = (_previousVelocity + _velocity) * 0.5f;
        _controller.Move(deltaPosition * Time.deltaTime);
    }

    //private void Update()
    //{
    //    _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
    //    if (_isAttacking)
    //    {
    //        _inputVector.x = 0;
    //    }

    //    // Check if reached apex jump height, then set gravity to falling gravity
    //    if (!_isApexReached && _maxHeightReached > transform.position.y)
    //    {
    //        _isApexReached = true;
    //        _gravity = _fallingGravity;

    //        // Debug only
    //        float jumpHeight = _maxHeightReached - _startJumpHeight;
    //        print("Jump height: " + jumpHeight);
    //    }

    //    _maxHeightReached = Mathf.Max(transform.position.y, _maxHeightReached);

    //    Vector2 deltaPosition = CalculateDeltaPosition();

    //    // Move the character
    //    _controller.Move(deltaPosition * Time.deltaTime);

    //    // Remove the accumulation of gravity
    //    if (_controller.CollisionInfo.below)
    //    {
    //        _velocity.y = 0;
    //        _gravity = _normalGravity;
    //    }

    //    // Remove the collision force left/right
    //    if (_controller.CollisionInfo.left || _controller.CollisionInfo.right)
    //    {
    //        _velocity.x = 0;
    //    }

    //    _comboWindowTimer -= Time.deltaTime;
    //}

    private void LateUpdate()
    {
        DetermineSpriteFacingDirection();
        return;

        if (_rollTimer > 0f)
        {
            _animator.Play("roll_FK");
            return;
        }

        if (_isAttacking)
        {
            if (_comboCounter == 1)
            {
                Debug.Log("1-Attack");
                _animator.Play("attack_1_FK");
            }
            else if (_comboCounter == 2)
            {
                Debug.Log("2-Attack");
                _animator.Play("attack_2_FK");
            }
            else
            {
                Debug.Log("3-Attack");
                _animator.Play("attack_3_FK");
            }

            return;
        }

        if (_velocity.x == 0 && _controller.CollisionInfo.below)
        {
            _animator.Play("idle_FK");
        }
        else if (_velocity.x != 0 && _controller.CollisionInfo.below)
        {
            _animator.Play("run_FK");
        }
        else if (_velocity.y > 0 && !_controller.CollisionInfo.below)
        {
            _animator.Play("jump_up_FK");
        }
        else if (_velocity.y < 0 && !_controller.CollisionInfo.below)
        {
            _animator.Play("jump_down_FK");
        }
    }

    public void AttackOver()
    {
        _isAttacking = false;
        _comboWindowTimer = _comboWindow;
        _comboCounter = _comboCounter < 3 ? _comboCounter + 1 : 1;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    public void SetXVelocity(float xVelocity)
    {
        _velocity.x = xVelocity;
    }

    public void SetYVelocity(float yVelocity)
    {
        _velocity.y = yVelocity;
    }

    private Vector2 CalculateDeltaPosition()
    {
        _previousVelocity = _velocity;

        // Apply gravity
        _velocity.y += _gravity * Time.deltaTime;

        // Roll
        if (_rollTimer > 0)
        {
            _rollTimer -= Time.deltaTime;
            _velocity.x = _rollDirection * _rollSpeed;
        }
        // Walk
        else
        {
            _velocity.x = _inputVector.x * _moveSpeed;
        }

        Vector2 deltaPosition = (_previousVelocity + _velocity) * 0.5f;
        return deltaPosition;
    }

    private void DetermineSpriteFacingDirection()
    {
        if (_velocity.x > 0 && !_isFacingRight)
        {
            Flip();
        }

        if (_velocity.x < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_isFacingRight;
    }

    private void GameInputManager_OnJumpActionPerformed()
    {
        return;
        if (_isAttacking) return;

        if (_controller.CollisionInfo.below)
        {
            _velocity.y = _jumpForce;
            _maxHeightReached = Mathf.NegativeInfinity;
            _isApexReached = false;

            // Debug
            _startJumpHeight = transform.position.y;
        }
    }

    private void GameInputManager_OnJumpActionCaceled()
    {
        return;
        if (!_controller.CollisionInfo.below && _velocity.y > 0f)
        {
            _velocity.y = 0;
        }
    }

    private void GameInputManager_OnPrimaryAttackPerformed()
    {
        return;
        if (!_controller.CollisionInfo.below || _isAttacking) return;

        _isAttacking = true;

        if (_comboWindowTimer <= 0f)
        {
            _comboWindowTimer = _comboWindow;
            _comboCounter = 1;
        }
    }

    private void GameInputManager_OnRollActionPerformed()
    {
        if (_isAttacking) return;

        if (_rollTimer <= 0f && _controller.CollisionInfo.below && _inputVector.x != 0)
        {
            _rollTimer = _rollDuration;
            _rollDirection = _isFacingRight ? 1 : -1;
        }
    }
}
