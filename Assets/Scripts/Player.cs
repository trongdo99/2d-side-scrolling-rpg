using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _timeToJumpApex;
    [SerializeField] private float _fallGravityMultiplier;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform[] _groundRaycasts;

    private CharacterController2D _controller;
    private Rigidbody2D _rb;
    private Vector2 _inputVector;
    private float _jumpForce;
    private float _gravityScale;
    private float _gravityFallScale;
    private float _lastYPosition = Mathf.NegativeInfinity;
    private bool _isApexReached = false;
    private bool _isGrounded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controller = GetComponent<CharacterController2D>();

        float gravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _gravityScale = gravity / Physics2D.gravity.y;
        _gravityFallScale = _gravityScale * _fallGravityMultiplier;
        _rb.gravityScale = _gravityScale;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {gravity}, Scale: {_gravityScale}, Fall Scale: {_gravityFallScale}, Jump Force: {_jumpForce}");
    }

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody2D>();

        float gravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _gravityScale = gravity / Physics2D.gravity.y;
        _gravityFallScale = _gravityScale * _fallGravityMultiplier;
        _rb.gravityScale = _gravityScale;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;
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
        if (_controller.CollisionInfo.below)
        {
            _rb.gravityScale = _gravityScale;
        }

        //_isGrounded = false;

        //foreach (var transform in _groundRaycasts)
        //{
        //    var hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, _groundLayer);
        //    if (hit)
        //    {
        //        _isGrounded = true;
        //        _rb.gravityScale = _gravityScale;
        //    }
        //}

        if (!_isApexReached && _lastYPosition > _rb.position.y)
        {
            _isApexReached = true;
            _rb.gravityScale = _gravityFallScale;
        }

        _lastYPosition = Mathf.Max(_rb.position.y, _lastYPosition);

        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
    }

    private void FixedUpdate()
    {
        //_controller.Move(_inputVector * _moveSpeed);
        _rb.velocity *= Vector2.right * _inputVector.x * _moveSpeed;
        _rb.velocity = new Vector2(_inputVector.x * _moveSpeed, _rb.velocity.y);
    }

    private void GameInputManager_OnJumpAction()
    {
        if (_controller.CollisionInfo.below)
        {
            _rb.AddForce(new Vector2(0f, _jumpForce), ForceMode2D.Impulse);
            _lastYPosition = Mathf.NegativeInfinity;
            _isApexReached = false;
        }
    }
}
