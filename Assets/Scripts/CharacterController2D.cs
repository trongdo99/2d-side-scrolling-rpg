using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _skinWidth;
    [SerializeField] private int _horizontalRayCount;
    [SerializeField] private int _verticalRayCount;

    private Rigidbody2D _rb;
    private RaycastOrigin _raycastOrigins;
    private CollisionInfo _collisionInfo;
    private BoxCollider2D _boxCollider;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;
    private float _lastXVelocity;

    public CollisionInfo CollisionInfo => _collisionInfo;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();
        _collisionInfo.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollision(velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollision(velocity);
        }

        _rb.velocity = velocity;
        _lastXVelocity = _rb.velocity.x;
    }

    private void VerticalCollision(Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Vector2 raycastOrigin = directionY == -1 ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            raycastOrigin += Vector2.right * (_verticalRaySpacing * i + (Mathf.Abs(velocity.x - _lastXVelocity)));

            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.up * directionY, rayLength, _collisionMask);
            if (hit)
            {
                rayLength = hit.distance;

                _collisionInfo.above = directionY == 1;
                _collisionInfo.below = directionY == -1;

                Debug.DrawRay(raycastOrigin, Vector2.up * directionY * rayLength, Color.red);
            }
            else
            {
                Debug.DrawRay(raycastOrigin, Vector2.up * directionY * rayLength, Color.green);
            }
        }
    }

    private void HorizontalCollision(Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

        for (int i = 0; i < _horizontalRayCount; i++)
        {
            Vector2 raycastOrigin = directionX == -1 ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            raycastOrigin += Vector2.up * (_horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.right * directionX, rayLength, _collisionMask);
            if (hit)
            {
                rayLength = hit.distance;

                _collisionInfo.right = directionX == 1;
                _collisionInfo.left = directionX == -1;

                Debug.DrawRay(raycastOrigin, Vector2.right * directionX * rayLength, Color.red);
            }
            else
            {
                Debug.DrawRay(raycastOrigin, Vector2.right * directionX * rayLength, Color.green);
            }
        }
    }

    private void CalculateRaySpacing()
    {
        Bounds bound = _boxCollider.bounds;
        bound.Expand(-2 * _skinWidth);

        _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
        _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bound.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bound.size.x / (_verticalRayCount - 1);
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bound = _boxCollider.bounds;
        bound.Expand(-2 * _skinWidth);

        _raycastOrigins.topLeft = new Vector2(bound.min.x, bound.max.y);
        _raycastOrigins.topRight = new Vector2(bound.max.x, bound.max.y);
        _raycastOrigins.bottomLeft = new Vector2(bound.min.x, bound.min.y);
        _raycastOrigins.bottomRight = new Vector2(bound.max.x, bound.min.y);
    }
}

public struct RaycastOrigin
{
    public Vector2 topLeft;
    public Vector2 bottomLeft;
    public Vector2 topRight;
    public Vector2 bottomRight;
}

public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;

    public void Reset()
    {
        above = below = left = right = false;
    }
}

