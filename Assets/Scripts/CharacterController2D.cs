using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    public const float GRAVITY = -9.8f;

    // Used to determine which objects to collide with
    [SerializeField] private LayerMask _platformMask;
    [SerializeField] private LayerMask _oneWayPlatformMask;
    [SerializeField] private float _skinWidth;
    [SerializeField] private int _horizontalRayCount;
    [SerializeField] private int _verticalRayCount;
    [SerializeField] private float _fallMultiplier;

    public CharacterControllerState State;

    private Vector2 _velocity;
    private Vector2 _lastFrameVelocity;
    private float _gravity = GRAVITY;
    private float _currentGravity;
    private float _overrideGravity;
    private bool _hasOverrideGravity = false;
    private bool _isGravityActive = true;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;
    private BoxCollider2D _boxCollider;
    private RaycastOrigins _raycastOrigins;
    LayerMask _savePlatformMask;

    public bool IsGravityActive { get => _isGravityActive; }
    public float Gravity { get { return _hasOverrideGravity ? _overrideGravity : _gravity; } }

    // Start is called before the first frame update
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        State = new CharacterControllerState();
        CalculateRaySpacing();

        _savePlatformMask = _platformMask;
        _platformMask |= _oneWayPlatformMask;
    }

    public void SetGravityActive(bool active)
    {
        _isGravityActive = active;
    }

    public void SetOverrideGravity(float value)
    {
        _hasOverrideGravity = true;
        _overrideGravity = value;
    }

    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetHorizontalForce(float force)
    {
        _velocity.x = force;
    }

    public void SetVerticalFoce(float force)
    {
        _velocity.y = force;
    }

    public IEnumerator DisableCollisionFor(float duration)
    {
        DisableCollision();
        yield return new WaitForSeconds(duration);
        EnableCollision();
    }

    public void EnableCollision()
    {
        _platformMask = _savePlatformMask;
        _platformMask |= _oneWayPlatformMask;
    }

    public void DisableCollision()
    {
        _platformMask = 0;
    }

    private void Update()
    {
        FrameInitialization();
        ApplyGravity();

        // Calculate the velocity that will be applied to the transform in this update
        Vector2 appliedVelocity = (_lastFrameVelocity + _velocity) * 0.5f * Time.deltaTime;

        UpdateRaycastOrigins();

        // Cast rays on all sides to check for collisions
        if (appliedVelocity.x != 0)
        {
            HorizontalCollisions(ref appliedVelocity);
        }
        if (appliedVelocity.y != 0)
        {
            VerticalCollisions(ref appliedVelocity);
        }

        // Move the transform
        transform.Translate(appliedVelocity);

        UpdateRaycastOrigins();

        // Set state
        if (!State.WasGroundedLastFrame && State.IsCollidingBelow)
        {
            State.JustGotGrounded = true;
        }

        // Force the physic engine to synchronize physic model after making changes in transform.
        // Prevent player from constantly sinking to the ground at microseconds.
        Physics2D.SyncTransforms();
    }

    private void FrameInitialization()
    {
        _lastFrameVelocity = _velocity;
        State.WasGroundedLastFrame = State.IsCollidingBelow;
        State.WasTouchingTheCeilingLastFrame = State.IsCollidingAbove;
        State.Reset();
    }

    private void ApplyGravity()
    {
        _currentGravity = Gravity;
        if (_velocity.y < 0)
        {
            _currentGravity *= _fallMultiplier;
        }

        if (_isGravityActive)
        {
            _velocity.y += _currentGravity * Time.deltaTime;
        }
    }

    //private void CheckBottomEdgeCollisions()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(_raycastOrigins.leftBottomEdge, Vector2.down, 0.1f, _collisionMask);
    //    _collisionInfo.leftBottomEdge = hit ? true : false;
    //    if (hit)
    //    {
    //        Debug.DrawRay(_raycastOrigins.leftBottomEdge, Vector2.down * 0.1f, Color.red);
    //    }
    //    else
    //    {
    //        Debug.DrawRay(_raycastOrigins.leftBottomEdge, Vector2.down * 0.1f, Color.green);
    //    }
    //    hit = Physics2D.Raycast(_raycastOrigins.rightBottomEdge, Vector2.down, 0.1f, _collisionMask);
    //    _collisionInfo.rightBottomEdge = hit ? true : false;
    //    if (hit)
    //    {
    //        Debug.DrawRay(_raycastOrigins.rightBottomEdge, Vector2.down * 0.1f, Color.red);
    //    }
    //    else
    //    {
    //        Debug.DrawRay(_raycastOrigins.rightBottomEdge, Vector2.down * 0.1f, Color.green);
    //    }
    //}

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
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _platformMask);

            // Set x moveDistance to amount needed to move from current position to the point which the ray collided with obstacle
            if (hit)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red, 0.01f);

                velocity.x = (hit.distance - _skinWidth) * directionX;
                // Set all ray lengths to the nearest hit ray
                // Avoids clipping scenario
                rayLength = hit.distance;

                State.IsCollidingLeft = directionX == -1;
                State.IsCollidingRight = directionX == 1;
                _velocity.x = 0f;
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

        State.IsFalling = directionY == -1;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            // If moving down, cast from bottom left corner
            // If moving up, cast from top left corner
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _platformMask);

            // Set y moveDistance to amount needed to move from current position to the point which the ray collided with obstacle
            if (hit)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                velocity.y = (hit.distance - _skinWidth) * directionY;
                // Set all ray lengths to the nearest hit ray
                // Avoids clipping scenario
                rayLength = hit.distance;

                State.IsCollidingBelow = directionY == -1;
                State.IsCollidingAbove = directionY == 1;
                State.IsFalling = !State.IsCollidingBelow;
                _velocity.y = 0f;
            }
            else 
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
}
