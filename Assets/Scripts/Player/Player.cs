using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private LedgeDetector _ledgeDetector;

    [Header("Attack settings")]
    [SerializeField] private float _comboDelay;
    [SerializeField] private float _comboWindow;
    [SerializeField] private int _comboInputBufferFrame;
    [SerializeField] private Vector2[] _attackMovement;

    [Header("Dash settings")]
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashSpeed;

    [Header("Roll settings")]
    [SerializeField] private float _rollCooldown;
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;

    public float LastRollTime { get; set; }
    public float LastDashTime { get; set; }
    public float LastComboTime { get; set; }
    public LedgeDetector LedgeDetector => _ledgeDetector;

    // Local variables
    private float _jumpForce;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerRollState RollState { get; private set; }
    public PlayerPrimaryAttackState PrimaryAttackState { get; private set; }

    public float MoveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float JumpForce { get => _jumpForce; private set => _jumpForce = value; }
    public float DashCooldown { get => _dashCooldown; private set => _dashCooldown = value; }
    public float DashDuration { get => _dashDuration; private set => _dashDuration = value; }
    public float DashSpeed { get => _dashSpeed; private set => _dashSpeed = value; }
    public float RollCooldown { get => _rollCooldown; private set => _rollCooldown = value; }
    public float RollDuration { get => _rollDuration; private set => _rollDuration = value; }
    public float RollSpeed { get => _rollSpeed; private set => _rollSpeed = value; }
    public float ComboDelay { get => _comboDelay; private set => _comboDelay = value; }
    public float ComboWindow { get => _comboWindow; private set => _comboWindow = value; }
    public int ComboInputBufferFrame { get => _comboInputBufferFrame; private set => _comboInputBufferFrame = value; }
    public Vector2[] AttackMovement { get => _attackMovement; private set => _attackMovement = value; }


    protected override void Awake()
    {
        base.Awake();

        // Set on ground gravity
        _controller.SetOverrideGravity(-2 * _maxJumpHeight / Mathf.Pow(_timeToJumpApex, 2));
        _jumpForce = 2 * _maxJumpHeight / _timeToJumpApex;

        // Init states
        IdleState = new PlayerIdleState(_stateMachine, this, _controller, _animator);
        MoveState = new PlayerMoveState(_stateMachine, this, _controller, _animator);
        JumpState = new PlayerJumpState(_stateMachine, this, _controller, _animator);
        FallState = new PlayerFallState(_stateMachine, this, _controller, _animator);
        LedgeClimbState = new PlayerLedgeClimbState(_stateMachine, this, _controller, _animator);
        DashState = new PlayerDashState(_stateMachine, this, _controller, _animator);
        RollState = new PlayerRollState(_stateMachine, this, _controller, _animator);
        PrimaryAttackState = new PlayerPrimaryAttackState(_stateMachine, this, _controller, _animator);
        _stateMachine.Init(IdleState);

        // Set initial variable's value
        LastRollTime = float.MinValue;
    }

    protected override void Update()
    {
        base.Update();
    }
}
