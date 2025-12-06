using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PatrolEnemy : MonoBehaviour
{
    public event Action OnPointArrived;
    
    [SerializeField] private float _patrolSpeed = 1f;
    [SerializeField] private List<Transform> _patrolPoints;
    [SerializeField] private Rigidbody2D _rigidbody;

    private void FixedUpdate()
    {
        if (_rigidbody == null)
            return;
        
        if (_patrolPoints.Count == 0)
            return;

        var currentPos = _rigidbody.position;
        var patrolPos = _patrolPoints[_currentPointId].position;
        
        if (Mathf.Approximately(currentPos.x, patrolPos.x) && Mathf.Approximately(currentPos.y, patrolPos.y))
        {
            _currentPointId = Random.Range(0, _patrolPoints.Count);
            OnPointArrived?.Invoke();
        }

        var dir = (_patrolPoints[_currentPointId].position - transform.position).normalized;
        var maxStep = (_patrolPoints[_currentPointId].position - transform.position).magnitude;
        var step = Mathf.Clamp(_patrolSpeed * Time.fixedDeltaTime, 0, maxStep);

        var dir2d = new Vector2(dir.x, dir.y);
        var targetPos = _rigidbody.position + dir2d * step;
        _rigidbody.MovePosition(targetPos);
    }

    // private

    private int _currentPointId = 0;
}
