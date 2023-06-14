using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyIdleState idleState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(_stateMachine, this, _animator);
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
