using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;

    [Header("Move settings")]
    [SerializeField] protected float _moveSpeed;

    [Header("Jump settings")]
    [SerializeField] protected float _maxJumpHeight;
    [SerializeField] protected float _timeToJumpApex;
    [SerializeField] protected float _fallGravityMultiplier;

    [Header("Visual")]
    [SerializeField]
    protected bool _isFacingRight = true;

    [HideInInspector] public int facingDirection;
    [HideInInspector] public bool isBusy;

    protected CharacterController2D _controller;

    // Movement variables
    protected Vector2 _velocity;
    protected Vector2 _previousVelocity;

    // Jump variables
    protected float _gravity;
    protected float _normalGravity;
    protected float _fallingGravity;
    protected float _jumpForce;

    // State Machine
    protected StateMachine _stateMachine;

    public CharacterController2D Controller { get => _controller; private set => _controller = value; }

    protected virtual void Awake()
    {
        _stateMachine = new StateMachine();
        _controller = GetComponent<CharacterController2D>();
        _normalGravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _fallingGravity = _normalGravity * _fallGravityMultiplier;
        _gravity = _normalGravity;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {_gravity}, Jump Force: {_jumpForce}");
    }

    protected virtual void Update()
    {
        _previousVelocity = _velocity;
        _velocity.y += _gravity * Time.deltaTime;

        // Velocity calculation, animation are handled inside each state
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

    protected virtual void LateUpdate()
    {
        DetermineSpriteFacingDirection();
    }

    public void OnAnimationCompleted()
    {
        _stateMachine.OnStateAnimationTrigger();
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

    public void ChangeFacingDirection()
    {
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_isFacingRight;
        facingDirection = _isFacingRight ? 1 : -1;
    }

    protected void DetermineSpriteFacingDirection()
    {
        if (_velocity.x > 0 && !_isFacingRight)
        {
            ChangeFacingDirection();
        }

        if (_velocity.x < 0 && _isFacingRight)
        {
            ChangeFacingDirection();
        }

        // Enable updating facing direciton via Inspector
        _spriteRenderer.flipX = !_isFacingRight;
        facingDirection = _isFacingRight ? 1 : -1;
    }
}
