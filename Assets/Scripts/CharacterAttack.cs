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

    private Animator _animator;

    private bool _attackInProgress;
    private float _attackTimer;
    private float _bufferEndAt;
    private bool _buffering = false;
    private bool _charHztMvmtFlipInitialSetting;
    private bool _charHztMvmtFlipIntialSettingSet = false;

    protected override void Initialize()
    {
        base.Initialize();
        if (_characterHorizontalMovement != null)
        {
            _charHztMvmtFlipInitialSetting = _characterHorizontalMovement.FlipCharacterToFaceDirection;
        }

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
        
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        if (_attackInProgress) yield break;

        _attackInProgress = true;
        yield return new WaitForSeconds(_initialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(_activeDuration);
        DisableDamageArea();
        _attackInProgress = false;
    }

    private void EnableDamageArea()
    {
        _damageAreaCollider.enabled = true;
    }

    private void DisableDamageArea()
    {
        _damageAreaCollider.enabled = false;
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
