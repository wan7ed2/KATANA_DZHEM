using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour, IPushableByObstacle
{
    [SerializeField] private float _patrolSpeed = 1f;
    [SerializeField] private List<Transform> _patrolPoints;
    
    public void Push(Vector2 direction, float force)
    {
        Debug.Log("patrol pushed");
        _isPushed = true;

        Rigidbody2D rb;
        TryGetComponent(out rb);

        if (rb != null)
        {
            rb.AddForce(direction * force);
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void Update()
    {
        if (_isPushed)
            return;

        if (_patrolPoints.Count == 0)
            return;

        if (transform.position.Equals(_patrolPoints[_currentPointId].position))
        {
            _currentPointId = Random.Range(0, _patrolPoints.Count);
        }

        var dir = (_patrolPoints[_currentPointId].position - transform.position).normalized;
        var maxStep = (_patrolPoints[_currentPointId].position - transform.position).magnitude;
        var step = Mathf.Clamp(_patrolSpeed * Time.deltaTime, 0, maxStep);

        transform.Translate(dir * step);
    }

    // private

    private int _currentPointId = 0;
    private bool _isPushed = false;
}
