using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = GameInputManager.Instance.GetMovementVectorNormalized();

        Vector2 velocity = _moveSpeed * inputVector * Time.fixedDeltaTime;

        _rb.MovePosition(_rb.position + velocity);
    }
}
