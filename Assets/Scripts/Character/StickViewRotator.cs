using System;
using UnityEngine;

public class StickViewRotator : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _lerpV; 

    private void LateUpdate()
    {
        var dir = (_target.position - transform.position).normalized;
        var targetRotation = Quaternion.FromToRotation(Vector2.right, dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lerpV);
    }
}