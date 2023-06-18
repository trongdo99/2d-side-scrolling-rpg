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
    public float NormalGravity { get => _normalGravity; set => _normalGravity = value; }
    public float FallingGravity { get => _fallingGravity; set => _fallingGravity = value; }

    protected virtual void Awake()
    {
        _stateMachine = new StateMachine();
        _controller = GetComponent<CharacterController2D>();
        _normalGravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _fallingGravity = _normalGravity * _fallGravityMultiplier;
        _controller.gravity = _normalGravity;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {_gravity}, Jump Force: {_jumpForce}");
    }

    protected virtual void Update()
    {
        _stateMachine.OnUpdate();
    }

    protected virtual void LateUpdate()
    {
        DetermineSpriteFacingDirection();
    }

    public void OnAnimationCompleted()
    {
        _stateMachine.OnStateAnimationTrigger();
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
        if (_controller.Velocity.x > 0 && !_isFacingRight)
        {
            ChangeFacingDirection();
        }

        if (_controller.Velocity.x < 0 && _isFacingRight)
        {
            ChangeFacingDirection();
        }

        // Enable updating facing direciton via Inspector
        _spriteRenderer.flipX = !_isFacingRight;
        facingDirection = _isFacingRight ? 1 : -1;
    }
}
