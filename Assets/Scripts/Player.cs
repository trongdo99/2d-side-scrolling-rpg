using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector2 _groundCheckOffSet = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private Vector2 _yVelocity = Vector2.zero;
    private bool _isGrounded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Apply gravity
        _yVelocity += Physics2D.gravity * Time.fixedDeltaTime;

        // Ground check
        Vector2 point = _rb.position + _groundCheckOffSet;
        Collider2D groundCollider = Physics2D.OverlapBox(point, _groundCheckSize, 0f, _groundLayer);
        if (groundCollider)
        {
            _isGrounded = true;
            _rb.position = Physics2D.ClosestPoint(_rb.position, groundCollider);
            _yVelocity = Vector2.zero;
        }
        else
        {
            _isGrounded = false;
        }

        Debug.DrawRay(new Vector3(point.x - _groundCheckSize.x, point.y + _groundCheckSize.y, 0f), Vector3.right * 2f * _groundCheckSize.x, Color.green);
        Debug.DrawRay(new Vector3(point.x - _groundCheckSize.x, point.y + _groundCheckSize.y, 0f), Vector3.down * 2f * _groundCheckSize.y, Color.green);
        Debug.DrawRay(new Vector3(point.x + _groundCheckSize.x, point.y - _groundCheckSize.y, 0f), Vector3.left * 2f * _groundCheckSize.x, Color.green);
        Debug.DrawRay(new Vector3(point.x + _groundCheckSize.x, point.y - _groundCheckSize.y, 0f), Vector3.up * 2f * _groundCheckSize.y, Color.green);

        // Handle movement input
        Vector2 inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
        Vector2 xVelocity = _moveSpeed * inputVector * Time.fixedDeltaTime;

        Debug.Log("Y Velocity: " + _yVelocity);
        // Apply velocity to the player
        _rb.MovePosition(_rb.position + xVelocity + _yVelocity);
    }
}
