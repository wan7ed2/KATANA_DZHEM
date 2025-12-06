using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBehaviour : MonoBehaviour
{
    [SerializeField] private float _patrolSpeed = 1f;
    [SerializeField] private List<Transform> _patrolPoints;

    private void Update()
    {
        if (_patrolPoints.Count == 0)
            return;

        if (transform.position.Equals(_patrolPoints[_currentPointId].position))
        {
            _currentPointId = (_currentPointId + 1) % _patrolPoints.Count;
        }

        var dir = (_patrolPoints[_currentPointId].position - transform.position).normalized;
        var maxStep = (_patrolPoints[_currentPointId].position - transform.position).magnitude;
        var step = Mathf.Clamp(_patrolSpeed * Time.deltaTime, 0, maxStep);
         
        transform.Translate(dir * step);
    }

    // private

    private int _currentPointId = 0;
}
