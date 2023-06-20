using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack settings")]
    [SerializeField] private float _comboWindow;
    [SerializeField] private Vector2[] _attackMovement;

    [Header("Roll settings")]
    [SerializeField] private float _rollCooldown;
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;

    [HideInInspector] public float lastRollTime;

    // State machine
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerRollState rollState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    public float moveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float JumpForce { get => _jumpForce; private set => _jumpForce = value; }
    public float RollCooldown { get => _rollCooldown; private set => _rollCooldown = value; }
    public float RollDuration { get => _rollDuration; private set => _rollDuration = value; }
    public float RollSpeed { get => _rollSpeed; private set => _rollSpeed = value; }
    public float ComboWindow { get => _comboWindow; private set => _comboWindow = value; }
    public Vector2[] AttackMovement { get => _attackMovement; private set => _attackMovement = value; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new PlayerIdleState(_stateMachine, this, _controller, _animator);
        moveState = new PlayerMoveState(_stateMachine, this, _controller, _animator);
        jumpState = new PlayerJumpState(_stateMachine, this, _controller, _animator);
        fallState = new PlayerFallState(_stateMachine, this, _controller, _animator);
        rollState = new PlayerRollState(_stateMachine, this, _controller, _animator);
        primaryAttackState = new PlayerPrimaryAttackState(_stateMachine, this, _controller, _animator);
        _stateMachine.Init(idleState);

        lastRollTime = float.MinValue;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        DetermineSpriteFacingDirection();
    }
}
