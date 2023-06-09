using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _gravityScale;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _timeToJumpApex;
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform[] _groundRaycasts;

    private Rigidbody2D _rb;
    private Vector2 _inputVector;
    private bool _isGrounded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        float gravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _rb.gravityScale = gravity / Physics2D.gravity.y;
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        Debug.Log($"Gravity: {gravity}, Scale: {_rb.gravityScale}, Jump Force: {_jumpForce}");
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

    private void GameInputManager_OnJumpAction()
    {
        if (_isGrounded)
        {
            _rb.AddForce(new Vector2(0f, _jumpForce), ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        _isGrounded = false;
        foreach (var transform in _groundRaycasts)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, _groundLayer);
            _isGrounded = hit ? true : false;
        }

        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_inputVector.x * _moveSpeed, _rb.velocity.y);
    }
}
