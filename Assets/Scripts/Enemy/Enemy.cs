using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float _idleTime;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _battleTime;
    [SerializeField] private float _loseAgroDistance;

    [HideInInspector] public float lastAttackTime;

    private TargetDetector _targetDetector;

    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }
    public EnemyBattleState battleState { get; private set; }
    public EnemyAttackState attackState { get; private set; }

    public float MoveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float IdleTime { get => _idleTime; private set => _idleTime = value; }
    public float AttackRange { get => _attackRange; private set => _attackRange = value; }
    public float AttackCooldown { get => _attackCooldown; private set => _attackCooldown = value; }
    public float BattleTime { get => _battleTime; private set => _battleTime = value; }
    public float LoseAgroDistance { get => _loseAgroDistance; private set => _loseAgroDistance = value; }

    protected override void Awake()
    {
        base.Awake();

        _targetDetector = GetComponent<TargetDetector>();

        idleState = new EnemyIdleState(_stateMachine, this, _controller, _animator);
        moveState = new EnemyMoveState(_stateMachine, this, _controller, _animator);
        battleState = new EnemyBattleState(_stateMachine, this, _controller, _animator);
        attackState = new EnemyAttackState(_stateMachine, this, _controller, _animator);
        _stateMachine.Init(idleState);

        lastAttackTime = float.MinValue;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool IsPlayerDetected()
    {
        return _targetDetector.Target ? true : false;
    }
}
