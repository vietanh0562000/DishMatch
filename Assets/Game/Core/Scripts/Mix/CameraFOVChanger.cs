using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Camera)), DisallowMultipleComponent]
public class CameraFOVChanger : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private static readonly float BaseLineAspect = 0.5625f;
    
    private void OnValidate()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        var baseFov              = _camera.fieldOfView;
        var baseOrthographicSize = _camera.orthographicSize;
        var aspectRatio          = _camera.aspect;

        if (aspectRatio <= 0f) return;
        
        if (aspectRatio <= BaseLineAspect)
        {
            float changes = BaseLineAspect / aspectRatio;
            if (_camera.orthographic)
            {
                _camera.orthographicSize = baseOrthographicSize * changes;
            }
            else
            {
                _camera.fieldOfView = baseFov * changes;
            }
        }
    }
}
