using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; protected set; }

    public virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;

            OnAwake();
        }
    }

    protected virtual void OnAwake() { }

    protected virtual void OnDestroy()
    {
        if (Instance == this || Instance == null)
        {
            Instance = null;
        }
    }
}
