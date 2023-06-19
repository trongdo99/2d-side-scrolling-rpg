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
        RaycastHit2D hit = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.left, _detectionRange, _targetLayer);
        if (hit)
        {
            if (hit.transform.TryGetComponent<Player>(out Player player))
            {
                _target = player;
                Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.red);
                return;
            }
            else
            {
                Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.green);
            }
        }
        else
        {
            _target = null;
            Debug.DrawRay(_boxCollider.bounds.center, Vector2.left * _detectionRange, Color.green);
        }

        hit = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.right, _detectionRange, _targetLayer);
        if (hit)
        {
            if (hit.transform.TryGetComponent<Player>(out Player player))
            {
                _target = player;
                Debug.DrawRay(_boxCollider.bounds.center, Vector2.right * _detectionRange, Color.red);
            }
            else
            {
                Debug.DrawRay(_boxCollider.bounds.center, Vector2.right * _detectionRange, Color.green); }
            }
        else
        {
            _target = null;
            Debug.DrawRay(_boxCollider.bounds.center, Vector2.right * _detectionRange, Color.green);
        }
    }
}
