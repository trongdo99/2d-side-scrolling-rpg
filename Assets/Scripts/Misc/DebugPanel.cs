using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private Entity _entity;
    [SerializeField] private TMP_Text _textTMP;

    private void Update()
    {
        _textTMP.text = _entity.StateMachine.CurrentState.ToString();
    }
}
