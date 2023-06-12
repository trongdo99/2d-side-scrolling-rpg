using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventListener : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void EventAttackOver()
    {
        _player.AttackOver();
    }
}
