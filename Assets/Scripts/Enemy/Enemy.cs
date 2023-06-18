using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float _idleTime;

    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }

    public float MoveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float IdleTime { get => _idleTime; private set => _idleTime = value; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(_stateMachine, this, _controller, _animator);
        moveState = new EnemyMoveState(_stateMachine, this, _controller, _animator);
        _stateMachine.Init(idleState);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void Update()
    {
        base.Update();
    }
}
