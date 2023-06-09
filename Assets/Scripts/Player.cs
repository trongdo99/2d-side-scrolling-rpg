using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
    }

    private void FixedUpdate()
    {
        _velocity.y += _gravity * Time.fixedDeltaTime;
        float targetVelocityX = _inputVector.x * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.CollisionInfo.below) ? _accelerationTimeGrounded : _accelerationTimeAirborne);
        _controller.Move(_velocity * Time.fixedDeltaTime);

        if (_controller.CollisionInfo.below)
        {
            _velocity.y = 0;
        }

        if (_controller.CollisionInfo.left || _controller.CollisionInfo.right)
        {
            _velocity.x = 0;
        }
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
