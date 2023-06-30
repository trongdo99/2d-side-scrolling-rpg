using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack settings")]
    [SerializeField] private float _comboWindow;
    [SerializeField] private Vector2[] _attackMovement;

    [Header("Dash settings")]
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashSpeed;

    [Header("Roll settings")]
    [SerializeField] private float _rollCooldown;
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollSpeed;

    [HideInInspector] public float lastRollTime;
    [HideInInspector] public float lastDashTime;

    // State machine
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerLedgeClimbState ledgeClimbState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    public float moveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }
    public float JumpForce { get => _jumpForce; private set => _jumpForce = value; }
    public float DashCooldown { get => _dashCooldown; private set => _dashCooldown = value; }
    public float DashDuration { get => _dashDuration; private set => _dashDuration = value; }
    public float DashSpeed { get => _dashSpeed; private set => _dashSpeed = value; }
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
        ledgeClimbState = new PlayerLedgeClimbState(_stateMachine, this, _controller, _animator);
        dashState = new PlayerDashState(_stateMachine, this, _controller, _animator);
        primaryAttackState = new PlayerPrimaryAttackState(_stateMachine, this, _controller, _animator);
        _stateMachine.Init(idleState);

        lastRollTime = float.MinValue;
    }

    protected override void Update()
    {
        base.Update();

        CheckForDashInput();
    }

    protected override void LateUpdate()
    {
        DetermineSpriteFacingDirection();
    }

    private void CheckForDashInput()
    {
        Vector2 inputVector = GameInputManager.Instance.GetMovementVectorNormalized();
        if (GameInputManager.Instance.WasDashButtonPressed() && CanDash() && inputVector.x != 0)
        {
            facingDirection = (int)inputVector.x;
            _stateMachine.ChangeToState(dashState);
        }
    }

    private bool CanDash()
    {
        return Time.time > lastDashTime + DashCooldown;
    }
}
