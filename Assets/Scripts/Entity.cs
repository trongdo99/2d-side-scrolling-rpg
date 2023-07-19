using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum FacingDirection { Right = 1, Left = -1}

    [Header("References")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;

    [Header("Move settings")]
    [SerializeField] protected float _moveSpeed;

    [Header("Jump settings")]
    [SerializeField] protected float _maxJumpHeight;
    [SerializeField] protected float _timeToJumpApex;
    [SerializeField] protected float _fallGravityMultiplier;

    [Header("Facing Direction")]
    [SerializeField] protected FacingDirection _initialFacingDirection = FacingDirection.Right;

    protected int _currentFacingDirection;

    public bool IsBusy { get; set; }
    public int CurrentFacingDirection { get => _currentFacingDirection; }

    protected CharacterController2D _controller;

    // State Machine
    protected StateMachine _stateMachine;

    public CharacterController2D Controller { get => _controller; private set => _controller = value; }
    public StateMachine StateMachine { get => _stateMachine; private set => _stateMachine = value; }

    protected virtual void Awake()
    {
        _stateMachine = new StateMachine();
        _controller = GetComponent<CharacterController2D>();
    }

    protected virtual void Start()
    {
        Face(_initialFacingDirection);
    }

    protected virtual void Update()
    {
        _stateMachine.OnUpdate();
    }

    public void OnAnimationCompleted()
    {
        _stateMachine.OnStateAnimationCompleted();
    }

    public void OnAnimationTriggered()
    {
        _stateMachine.OnStateAnimationTriggered();
    }

    public void Face(FacingDirection facingDirection)
    {
        if (_currentFacingDirection == (int)facingDirection) return;

        _currentFacingDirection = (int)facingDirection;
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.flipX = _currentFacingDirection != 1;
    }

    public IEnumerator IsBusyFor(float seconds)
    {
        IsBusy = true;

        yield return new WaitForSeconds(seconds);

        IsBusy = false;
    }
}
