using System;
using UnityEngine;

public class PositionInterpolator : MonoBehaviour
{
    [SerializeField] private float _lerpV = 0.9f;
    
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _source;

    private void LateUpdate()
    {
        _source.position = Vector2.Lerp(_source.position, _target.position, _lerpV);
    }
}