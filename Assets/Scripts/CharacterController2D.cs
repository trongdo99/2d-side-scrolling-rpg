using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    public const float GRAVITY = -9.8f;

    // Used to determine which objects to collide with
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _skinWidth;
    [SerializeField] private int _horizontalRayCount;
    [SerializeField] private int _verticalRayCount;

    [ReadOnly] public float gravity = GRAVITY;
    [ReadOnly] public Vector2 Velocity;
    [SerializeField, ReadOnly] private Vector2 _preVelocity;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;
    private BoxCollider2D _boxCollider;
    private RaycastOrigins _raycastOrigins;
    private CollisionInfo _collisionInfo;
    private Vector2 _bottomLeftLedgePosition;
    private Vector2 _bottomRightLedgePosition;

    public CollisionInfo CollisionInfo => _collisionInfo;

    // Start is called before the first frame update
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void Update()
    {
        _preVelocity = Velocity;
        Velocity.y += gravity * Time.deltaTime;
        Vector2 appliedVelocity = (_preVelocity + Velocity) * 0.5f * Time.deltaTime;

        UpdateRaycastOrigins();
        _collisionInfo.Reset();

        CheckLedgeCollisions();

        CheckBottomEdgeCollisions();

        if (appliedVelocity.x != 0)
        {
            HorizontalCollisions(ref appliedVelocity);
        }
        if (appliedVelocity.y != 0)
        {
            VerticalCollisions(ref appliedVelocity);
        }

        transform.Translate(appliedVelocity);

        if (_collisionInfo.below)
        {
            Velocity.y = 0f;
        }

        if (_collisionInfo.right || _collisionInfo.left)
        {
            Velocity.x = 0f;
        }

        // Force the physic engine to synchronize physic model after making changes in transform.
        // Prevent player from constantly sinking to the ground at microseconds.
        Physics2D.SyncTransforms();
    }

    public bool CanGrapLedge()
    {
        return (!_collisionInfo.leftTopLedge && _collisionInfo.leftTop) || (!_collisionInfo.rightTopLedge && _collisionInfo.rightTop);
    }

    public Vector2 GetBottomLedgePosition(int facingDirection)
    {
        if (facingDirection > 0)
        {
            Debug.Log("Return Bottom Left ledge: " + _bottomLeftLedgePosition);
        }
        else if (facingDirection < 0)
        {
            Debug.Log("Return Bottom Right ledge: " + _bottomRightLedgePosition);
        }
        return facingDirection > 0 ? _bottomLeftLedgePosition : _bottomRightLedgePosition;
    }

    private void CheckLedgeCollisions()
    {
        if (Velocity.x < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(_raycastOrigins.leftTopLedge, Vector2.left, 0.2f + _skinWidth, _collisionMask);
            _collisionInfo.leftTopLedge = hit ? true : false;
            if (hit)
            {
                Debug.DrawRay(_raycastOrigins.leftTopLedge, Vector2.left * (0.2f + _skinWidth), Color.red);
            }
            else
            {
                Debug.DrawRay(_raycastOrigins.leftTopLedge, Vector2.left * (0.2f + _skinWidth), Color.green);
            }

            hit = Physics2D.Raycast(_raycastOrigins.topLeft, Vector2.left, (0.2f + _skinWidth), _collisionMask);
            _collisionInfo.leftTop = hit ? true : false;
            if (hit)
            {
                Debug.DrawRay(_raycastOrigins.topLeft, Vector2.left * (0.2f + _skinWidth), Color.red);
            }
            else
            {
                Debug.DrawRay(_raycastOrigins.topLeft, Vector2.left * (0.2f + _skinWidth), Color.green);
            }
        }
        
        if (Velocity.x > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(_raycastOrigins.rightTopLedge, Vector2.right, 0.2f + _skinWidth, _collisionMask);
            _collisionInfo.rightTopLedge = hit ? true : false;
            if (hit)
            {
                Debug.DrawRay(_raycastOrigins.rightTopLedge, Vector2.right * (0.2f + _skinWidth), Color.red);
            }
            else
            {
                Debug.DrawRay(_raycastOrigins.rightTopLedge, Vector2.right * (0.2f + _skinWidth), Color.green);
            }

            hit = Physics2D.Raycast(_raycastOrigins.topRight, Vector2.right, 0.2f + _skinWidth, _collisionMask);
            _collisionInfo.rightTop = hit ? true : false;
            if (hit)
            {
                Debug.DrawRay(_raycastOrigins.topRight, Vector2.right * (0.2f + _skinWidth), Color.green);
            }
            else
            {
                Debug.DrawRay(_raycastOrigins.topRight, Vector2.right * (0.2f + _skinWidth), Color.red);
            }
        }

        if (CanGrapLedge())
        {
            if (Velocity.x > 0)
            {
                _bottomLeftLedgePosition = new Vector2(Mathf.FloorToInt(_raycastOrigins.topRight.x + 0.2f + _skinWidth), Mathf.RoundToInt(_raycastOrigins.topRight.y));
                Debug.Log("Ledge tile bottom left position: " + _bottomLeftLedgePosition);
            }
            
            if (Velocity.x < 0)
            {
                _bottomRightLedgePosition = new Vector2(Mathf.CeilToInt(_raycastOrigins.topLeft.x - 0.2f + _skinWidth), Mathf.RoundToInt(_raycastOrigins.topRight.y));
                Debug.Log("Ledge tile bottom left position: " + _bottomRightLedgePosition);
            }
        }
    }

    private void CheckBottomEdgeCollisions()
    {
        RaycastHit2D hit = Physics2D.Raycast(_raycastOrigins.leftBottomEdge, Vector2.down, 0.1f, _collisionMask);
        _collisionInfo.leftBottomEdge = hit ? true : false;
        if (hit)
        {
            Debug.DrawRay(_raycastOrigins.leftBottomEdge, Vector2.down * 0.1f, Color.red);
        }
        else
        {
            Debug.DrawRay(_raycastOrigins.leftBottomEdge, Vector2.down * 0.1f, Color.green);
        }
        hit = Physics2D.Raycast(_raycastOrigins.rightBottomEdge, Vector2.down, 0.1f, _collisionMask);
        _collisionInfo.rightBottomEdge = hit ? true : false;
        if (hit)
        {
            Debug.DrawRay(_raycastOrigins.rightBottomEdge, Vector2.down * 0.1f, Color.red);
        }
        else
        {
            Debug.DrawRay(_raycastOrigins.rightBottomEdge, Vector2.down * 0.1f, Color.green);
        }
    }

    // Changes in this method effect moveDistance Move method
    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

        for (int i = 0; i < _horizontalRayCount; i++)
        {
            // If moving down, cast from bottom left corner
            // If moving up, cast from bottom right corner
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionMask);


            // Set x moveDistance to amount needed to move from current position to the point which the ray collided with obstacle
            if (hit)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red, 0.01f);

                velocity.x = (hit.distance - _skinWidth) * directionX;
                // Set all ray lengths to the nearest hit ray
                // Avoids clipping scenario
                rayLength = hit.distance;

                _collisionInfo.left = directionX == -1;
                _collisionInfo.right = directionX == 1;
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green, 0.01f);
            }
        }
    }


    // Changes in this method effect moveDistance inside Move method
    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            // If moving down, cast from bottom left corner
            // If moving up, cast from top left corner
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionMask);


            // Set y moveDistance to amount needed to move from current position to the point which the ray collided with obstacle
            if (hit)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                velocity.y = (hit.distance - _skinWidth) * directionY;
                // Set all ray lengths to the nearest hit ray
                // Avoids clipping scenario
                rayLength = hit.distance;

                _collisionInfo.below = directionY == -1;
                _collisionInfo.above = directionY == 1;
            } else
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green);
            }
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = _boxCollider.bounds;
        bounds.Expand(_skinWidth * -2);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        _raycastOrigins.leftBottomEdge = new Vector2(_raycastOrigins.bottomLeft.x - 0.1f, _raycastOrigins.bottomLeft.y);
        _raycastOrigins.rightBottomEdge = new Vector2(_raycastOrigins.bottomRight.x + 0.1f, _raycastOrigins.bottomRight.y);
        _raycastOrigins.leftTopLedge = new Vector2(_raycastOrigins.topLeft.x, _raycastOrigins.topLeft.y + 0.1f);
        _raycastOrigins.rightTopLedge = new Vector2(_raycastOrigins.topRight.x, _raycastOrigins.topRight.y + 0.1f);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = _boxCollider.bounds;
        bounds.Expand(_skinWidth * -2);

        _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
        _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }
}

public struct RaycastOrigins
{
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
    public Vector2 middleLeft, middleRight;
    public Vector2 leftBottomEdge, rightBottomEdge;
    public Vector2 leftTopLedge, rightTopLedge;
}

// Used to remove the accumulation of gravity and collisions left/right
public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;
    public bool leftBottomEdge, rightBottomEdge;
    public bool leftTopLedge, rightTopLedge;
    public bool leftTop, rightTop;

    public void Reset()
    {
        above = below = false;
        left = right = false;
        leftBottomEdge = rightBottomEdge = false;
        leftTopLedge = rightTopLedge = false;
        leftTop = rightTop = false;
    }
}
