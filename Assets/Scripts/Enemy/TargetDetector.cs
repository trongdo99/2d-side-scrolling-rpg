using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _detectionRange;
    [SerializeField, ReadOnly] private Player _target;

    private BoxCollider2D _boxCollider;
    private RaycastOrigins _raycastOrigins;

    public Player Target { get => _target; private set => _target = value; }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        bool leftFound = FindTargetUsingRaycast(Vector2.left);
        bool rightFound = false;

        if (!leftFound)
        {
            rightFound = FindTargetUsingRaycast(Vector2.right);
        }

        if (leftFound)
        {
            Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.red);
        }
        else
        {
            Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.green);
        }

        if (rightFound)
        {
            Debug.DrawRay(_boxCollider.bounds.center, Vector2.right * _detectionRange, Color.red);
        }
        else
        {

            Debug.DrawRay(_boxCollider.bounds.center, Vector2.right * _detectionRange, Color.green);
        }
    }

    private bool FindTargetUsingRaycast(Vector2 checkDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(_boxCollider.bounds.center, checkDirection, _detectionRange, _targetLayer);
        if (hit)
        {
            if (hit.transform.TryGetComponent(out Player player))
            {
                _target = player;
                return true;
            }
            else
            {
                _target = null;
                return false;
            }
        }

        _target = null;
        Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.green);
        return false;
    }
}
