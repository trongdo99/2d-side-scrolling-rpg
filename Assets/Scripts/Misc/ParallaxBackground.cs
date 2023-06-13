using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] ParallaLayer[] _layerList;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        foreach (var layer in _layerList)
        {
            float distanceToMove = _camera.transform.position.x * layer.parallaxEffect;
            layer.background.position = new Vector3(distanceToMove, layer.background.position.y);
        }
    }
}

[System.Serializable]
public struct ParallaLayer
{
    public Transform background;
    public float parallaxEffect;
}
