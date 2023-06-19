using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEventListener : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void EventAnimationCompleted()
    {
        _enemy.OnAnimationCompleted();
    }
}
