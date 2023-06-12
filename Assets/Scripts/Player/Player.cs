using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    [Header("Attack settings")]
    [SerializeField] private float _comboWindow;
    [SerializeField] private Vector2[] _attackMovement;


    [Header("Move settings")]
    [SerializeField] private float _moveSpeed;


    [Header("Jump settings")]
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _timeToJumpApex;
    [SerializeField] private float _fallGravityMultiplier;

    [Header("Roll settings")]
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;

    public int facingDirection;
    public bool isBusy;

    private CharacterController2D _controller;

    // Movement variables
    private Vector2 _velocity;
    private Vector2 _previousVelocity;

    // Jump variables
    private float _gravity;
    private float _normalGravity;
    private float _fallingGravity;
    private float _jumpForce;

    // Visual variables
    private bool _isFacingRight = true;

    // State Machine
    private PlayerStateMachine _stateMachine;
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerRollState rollState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }


    public CharacterController2D Controller { get => _controller; private set => _controller = value; }
    public float moveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float JumpForce { get => _jumpForce; private set => _jumpForce = value; }
    public float RollDuration { get => _rollDuration; private set => _rollDuration = value; }
    public float RollSpeed { get => _rollSpeed; private set => _rollSpeed = value; }
    public float ComboWindow { get => _comboWindow; private set => _comboWindow = value; }
    public Vector2[] AttackMovement { get => _attackMovement; private set => _attackMovement = value; }

    private void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(_stateMachine, this, _animator);
        moveState = new PlayerMoveState(_stateMachine, this, _animator);
        jumpState = new PlayerJumpState(_stateMachine, this, _animator);
        fallState = new PlayerFallState(_stateMachine, this, _animator);
        rollState = new PlayerRollState(_stateMachine, this, _animator);
        primaryAttackState = new PlayerPrimaryAttackState(_stateMachine, this, _animator);

        _stateMachine.Init(idleState);

        _controller = GetComponent<CharacterController2D>();

        _normalGravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _fallingGravity = _normalGravity * _fallGravityMultiplier;
        _gravity = _normalGravity;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {_gravity}, Jump Force: {_jumpForce}");
    }

    private void Update()
    {
        _previousVelocity = _velocity;
        _velocity.y += _gravity * Time.deltaTime;

        _stateMachine.OnUpdate();

        Vector2 deltaPosition = (_previousVelocity + _velocity) * 0.5f;
        _controller.Move(deltaPosition * Time.deltaTime);

        // Remove the accumulation of gravity
        if (_controller.CollisionInfo.below)
        {
            _velocity.y = 0;
            _gravity = _normalGravity;
        }

        // Remove the collision force left/right
        if (_controller.CollisionInfo.left || _controller.CollisionInfo.right)
        {
            _velocity.x = 0;
        }
    }

    private void LateUpdate()
    {
        DetermineSpriteFacingDirection();
    }

    public void OnAnimationCompleted()
    {
        (_stateMachine.CurrentState as PlayerState).OnAnimationCompleted();
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

    public void SetNormalGravity()
    {
        _gravity = _normalGravity;
    }

    public void SetFallingGravity()
    {
        _gravity = _fallingGravity;
    }

    public IEnumerator isBusyFor(float seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(seconds);

        isBusy = false;
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
        facingDirection = _isFacingRight ? 1 : -1;
    }
}
