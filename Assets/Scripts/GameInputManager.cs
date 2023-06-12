using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : SingletonMonoBehaviour<GameInputManager>
{
    public event Action OnJumpActionPerformed;
    public event Action OnJumpActionCaceled;
    public event Action OnPrimaryAttackPerformed;
    public event Action OnRollActionPerformed;

    private PlayerInputActions _playerInputActions;

    protected override void OnAwake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Jump.performed += Jump_performed;
        _playerInputActions.Player.Jump.canceled += Jump_canceled;

        _playerInputActions.Player.PrimaryAttack.performed += PrimaryAttack_performed;

        _playerInputActions.Player.Roll.performed += Roll_performed;
    }

    private void PrimaryAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPrimaryAttackPerformed?.Invoke();
    }

    private void Roll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRollActionPerformed?.Invoke();
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpActionCaceled?.Invoke();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpActionPerformed?.Invoke();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }
}
