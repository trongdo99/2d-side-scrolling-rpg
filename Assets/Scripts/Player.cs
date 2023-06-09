using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _accelerationTimeGrounded;
    [SerializeField] private float _accelerationTimeAirborne;
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _timeToJumpApex;
    [SerializeField] private float _fallGravityMultiplier;

    private CharacterController2D _controller;
    private Vector2 _inputVector;
    private Vector2 _velocity;
    private float _velocityXSmoothing;
    private float _gravity;
    private float _normalGravity;
    private float _fallingGravity;
    private float _jumpForce;
    private float _lastYPosition = Mathf.NegativeInfinity;
    private bool _isApexReached = false;
    private bool _isFacingRight = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController2D>();

        _normalGravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _fallingGravity = _normalGravity * _fallGravityMultiplier;
        _gravity = _normalGravity;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {_gravity}, Jump Force: {_jumpForce}");
    }

    private void Start()
    {
        GameInputManager.Instance.OnJumpAction += GameInputManager_OnJumpAction;
    }

    private void OnDestroy()
    {
        if (GameInputManager.Instance)
        {
            GameInputManager.Instance.OnJumpAction -= GameInputManager_OnJumpAction;
        }
    }

    private void Update()
    {
        if (!_isApexReached && _lastYPosition > transform.position.y)
        {
            _isApexReached = true;
            _gravity = _fallingGravity;
        }

        _lastYPosition = Mathf.Max(transform.position.y, _lastYPosition);

        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();

        if (_inputVector.x > 0 && !_isFacingRight)
        {
            Flip();
        }

        if (_inputVector.x < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Apply gravity
        _velocity.y += _gravity * Time.fixedDeltaTime;

        // Smooth out velocity x
        float targetVelocityX = _inputVector.x * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.CollisionInfo.below) ? _accelerationTimeGrounded : _accelerationTimeAirborne);
        // Set velocity x to 0 to make sure the character stop completely when there is no input
        if (Mathf.Abs(_velocity.x - targetVelocityX) < 1f && _inputVector.x == 0)
        {
            _velocity.x = 0;
        }

        // Apply velocity
        _controller.Move(_velocity * Time.fixedDeltaTime);

        // Remove accumulate gravity
        if (_controller.CollisionInfo.below)
        {
            _velocity.y = 0;
        }

        if (_controller.CollisionInfo.left || _controller.CollisionInfo.right)
        {
            _velocity.x = 0;
        }
    }

    private void LateUpdate()
    {
        Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("run_FK"));
        if (Mathf.Approximately(_velocity.x, 0f) && _controller.CollisionInfo.below)
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
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_isFacingRight;
    }

    private void GameInputManager_OnJumpAction()
    {
        if (_controller.CollisionInfo.below)
        {
            _velocity.y = _jumpForce;
            _lastYPosition = Mathf.NegativeInfinity;
            _isApexReached = false;
        }
    }
}
