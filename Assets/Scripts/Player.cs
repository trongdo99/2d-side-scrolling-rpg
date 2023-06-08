using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _gravityScale;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform[] _groundRaycasts;

    private Rigidbody2D _rb;
    private Vector2 _inputVector;
    private Vector2 _yVelocity = Vector2.zero;
    private bool _isGrounded = false;
    private bool _isJumping = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        float jumpForce = Mathf.Sqrt(_jumpHeight * -2 * (Physics2D.gravity.y * _rb.gravityScale));
        _rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        print("Jump");
    }

    private void Update()
    {
        _isGrounded = false;
        foreach (var transform in _groundRaycasts)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, _groundLayer);
            if (hit)
            {
                _isGrounded = true;
                _isJumping = false;
                break;
            }
        }

        _inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
    }

    private void FixedUpdate()
    {
        _rb.velocity = _inputVector * _moveSpeed;
    }
}
