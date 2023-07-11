using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _rightAbovePoint;
    [SerializeField] private Transform _rightBelowPoint;
    [SerializeField] private Transform _leftAbovePoint;
    [SerializeField] private Transform _leftBelowPoint;

    [SerializeField] private float _checkDistance;

    public bool CheckForLedge(int facingDirection, out Vector2 ledgePosition)
    {
        switch (facingDirection)
        {
            case 1:
                 // Right ledge check
                RaycastHit2D rightRaycastHit = Physics2D.Raycast(_rightAbovePoint.position, Vector2.right, _checkDistance, _layerMask);
                if (!rightRaycastHit)
                {
                    rightRaycastHit = Physics2D.Raycast(_rightBelowPoint.position, Vector2.right, _checkDistance, _layerMask);
                    if (rightRaycastHit)
                    {
                        // Right ledge detected
                        var rightEdgePosition = new Vector2(Mathf.RoundToInt(_rightAbovePoint.position.x + _checkDistance), Mathf.RoundToInt(_rightAbovePoint.position.y));
                        Debug.Log("Right ledge detected, position: " + rightEdgePosition);
                        ledgePosition = rightEdgePosition;
                        return true;
                    }
                }
                ledgePosition = Vector2.zero;
                return false;

            case -1:
                RaycastHit2D leftRaycastHit = Physics2D.Raycast(_leftAbovePoint.position, Vector2.left, _checkDistance, _layerMask);
                if (!leftRaycastHit)
                {
                    leftRaycastHit = Physics2D.Raycast(_leftBelowPoint.position, Vector2.left, _checkDistance, _layerMask);
                    if (leftRaycastHit)
                    {
                        // Left ledge detected
                        var leftEdgePosition = new Vector2(Mathf.RoundToInt(_leftAbovePoint.position.x - _checkDistance), Mathf.RoundToInt(_leftAbovePoint.position.y));
                        Debug.Log("Left ledge detected, position: " + leftEdgePosition);
                        ledgePosition = leftEdgePosition;
                        return true;
                    }
                }
                ledgePosition = Vector2.zero;
                return false;

            default:
                Debug.LogError("Invalid facing direction: " + facingDirection);
                ledgePosition = Vector2.zero;
                return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(_rightAbovePoint.position, Vector3.right * _checkDistance);
        Gizmos.DrawRay(_rightBelowPoint.position, Vector3.right * _checkDistance);
        Gizmos.DrawRay(_leftAbovePoint.position, Vector3.left * _checkDistance);
        Gizmos.DrawRay(_leftBelowPoint.position, Vector3.left * _checkDistance);
    }
}
