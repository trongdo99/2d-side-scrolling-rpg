using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : SingletonMonoBehaviour<GameInputManager>
{
    public event Action OnJumpAction;

    private PlayerInputActions _playerInputActions;

    protected override void OnAwake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }
}
