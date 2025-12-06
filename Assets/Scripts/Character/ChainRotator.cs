using System;
using UnityEngine;

public class ChainRotator : MonoBehaviour
{
    [SerializeField] private Transform _target;
    
    private void LateUpdate()
    {
        var dir = (_target.position - transform.position).normalized;
        var targetRotation = Quaternion.FromToRotation(Vector2.right, dir);
        transform.rotation = targetRotation;
    }
}