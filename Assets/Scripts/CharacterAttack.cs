using System;
using System.Collections;
using System.Collections.Generic;
using BanhMy.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterAttack : CharacterAbility
{
    [Header("Attack configs")]
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _activeDuration;
    [SerializeField] private float _dropComboTime;

    [FormerlySerializedAs("_hitBox")]
    [Header("Damage area configs")]
    [SerializeField] private GameObject _damageArea;
    [SerializeField] private BoxCollider2D _damageAreaCollider;
    [SerializeField] private Vector2 _damageAreaOffset;
    [SerializeField] private Vector2 _damageAreaSize;
    
    [Header("Input")]
    [SerializeField] private bool _continuousPress;
    [SerializeField] private bool _faceAttackDirection;

    [Header("Buffering")]
    [SerializeField] private bool _bufferInput;
    [SerializeField] private float _bufferDuration;

    private int _comboIndex = 1;
    private int _comboLenght;
    private float _lastComboAt;
    private bool _attackInProgress;
    private float _attackTimer;
    private float _bufferEndAt;
    private bool _buffering = false;
    private bool _charHztMvmtFlipInitialSetting;
    private bool _charHztMvmtFlipIntialSettingSet = false;

    #region Animation parameters

    private const string _combo1StartedAnimationParameterName = "Combo1Started";
    private const string _combo2StartedAnimationParameterName = "Combo2Started";
    private const string _combo3StartedAnimationParameterName = "Combo3Started";
    private int _combo1StartedAnimationParameter;
    private int _combo2StartedAnimationParameter;
    private int _combo3StartedAnimationParameter;

    private Dictionary<int, int> _animationDict;

    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        if (_characterHorizontalMovement != null)
        {
            _charHztMvmtFlipInitialSetting = _characterHorizontalMovement.FlipCharacterToFaceDirection;
        }

        CreateDamageArea();
    }

    private void CreateDamageArea()
    {
        if (_damageArea == null)
        {
            _damageArea = new GameObject(_character.name + "DamageArea");
            _damageArea.transform.position = _character.transform.position;
            _damageArea.transform.rotation = _character.transform.rotation;
            _damageArea.transform.SetParent(_character.transform);
            _damageAreaCollider = _damageArea.AddComponent<BoxCollider2D>();
            _damageAreaCollider.size = _damageAreaSize;
            _damageAreaCollider.offset = _damageAreaOffset;
            _damageAreaCollider.isTrigger = true;

            var rigidbody2D = _damageArea.AddComponent<Rigidbody2D>();
            rigidbody2D.isKinematic = true;
        }
        
        DisableDamageArea();
    }

    protected override void HandleInput()
    {
        if (_inputManager.WasPrimaryAttackButtonPressed())
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!AbilityAuthorized
            || _movementStateMachine.CurrentState == CharacterState.MovementState.Rolling
            || _conditionStateMachine.CurrentState != CharacterState.CharacterCondition.Normal)
        {
            return;
        }
        
        _movementStateMachine.ChangeState(CharacterState.MovementState.Attacking);

        if (_bufferInput && _movementStateMachine.CurrentState == CharacterState.MovementState.Attacking)
        {
            if (!_buffering)
            {
                _buffering = true;
                _bufferEndAt = Time.time + _bufferDuration;
            }
        }

        if (Time.time > _lastComboAt + _dropComboTime)
        {
            _comboIndex = 1;
        }
        
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        if (_attackInProgress) yield break;

        _animator.UpdateAnimatorTrigger(_animationDict[_comboIndex], _character.AnimatorParameters,
            _character.PerformSanityCheck);
        _attackInProgress = true;
        _characterHorizontalMovement.ReadInput = false;
        _characterHorizontalMovement.SetHorizontalMove(0f);
        yield return new WaitForSeconds(_initialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(_activeDuration);
        DisableDamageArea();
        _attackInProgress = false;

        StopAttack();
    }
    
    private void EnableDamageArea()
    {
        _damageAreaCollider.enabled = true;
    }

    private void DisableDamageArea()
    {
        _damageAreaCollider.enabled = false;
    }

    private void StopAttack()
    {
        if (_comboIndex == _comboLenght)
        {
            _comboIndex = 1;
            _lastComboAt = 0f;
        }
        else
        {
            _comboIndex++;
        }

        _lastComboAt = Time.time;
        
        _characterHorizontalMovement.ReadInput = true;

        if (_movementStateMachine.CurrentState == CharacterState.MovementState.Attacking)
        {
            _movementStateMachine.RestorePreviousState();
        }
    }

    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter(_combo1StartedAnimationParameterName, AnimatorControllerParameterType.Trigger, out _combo1StartedAnimationParameter);
        RegisterAnimatorParameter(_combo2StartedAnimationParameterName, AnimatorControllerParameterType.Trigger, out _combo2StartedAnimationParameter);
        RegisterAnimatorParameter(_combo3StartedAnimationParameterName, AnimatorControllerParameterType.Trigger, out _combo3StartedAnimationParameter);
        _animationDict = new()
        {
            {1, _combo1StartedAnimationParameter},
            {2, _combo2StartedAnimationParameter},
            {3, _combo3StartedAnimationParameter},
        };
        _comboLenght = _animationDict.Count;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        Gizmos.color = Color.red;
        BMDebug.DrawGizmoRectangle(transform.position + (Vector3)_damageAreaOffset, _damageAreaSize, Color.red);
    }
}
