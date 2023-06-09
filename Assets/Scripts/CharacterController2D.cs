using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    public struct RaycastOrigin
    {
        public Vector2 topLeft;
        public Vector2 bottomLeft;
        public Vector2 topRight;
        public Vector2 bottomRight;
    }

    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _skinWidth;
    [SerializeField] private int _horizontalRayCount;
    [SerializeField] private int _verticalRayCount;

    private RaycastOrigin _raycastOrigins;
    private BoxCollider2D _boxCollider;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();
        for (int i = 0; i < _horizontalRayCount; i++)
        {
            Debug.DrawRay(new Vector2(_raycastOrigins.bottomLeft.x + _horizontalRaySpacing * i, _raycastOrigins.bottomLeft.y), Vector2.down * 0.2f, Color.red);
        }

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Debug.DrawRay(new Vector2(_raycastOrigins.topLeft.x, _raycastOrigins.topLeft.y - _verticalRaySpacing * i), Vector2.left * 0.2f, Color.blue);
        }
    }

    private void CalculateRaySpacing()
    {
        Bounds bound = _boxCollider.bounds;
        bound.Expand(-2 * _skinWidth);

        _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
        _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bound.size.x / (_horizontalRayCount - 1);
        _verticalRaySpacing = bound.size.y / (_verticalRayCount - 1);
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
