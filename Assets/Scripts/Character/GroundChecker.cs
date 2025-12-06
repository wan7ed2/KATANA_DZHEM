using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float _checkDistance;
    [SerializeField] private float _radiusCheck;
    [SerializeField] private float _circleCheckOffset;
    [SerializeField] private LayerMask _layerMask;

    public bool IsGrounded => _hit.collider != null || _circleHit != null;
    
    public bool RayHit => _hit;
    public bool CircleHit => _circleHit;
    
    public void Init(Rigidbody2D rigidbody)
    {
        _rigidbody = rigidbody;
    }

    public void Tick()
    {
        if (_rigidbody == null)
            return;

        _hit = Physics2D.Raycast(_rigidbody.position, Vector2.down, _checkDistance, _layerMask);

        var circlePos = _rigidbody.position + Vector2.down * _circleCheckOffset;
        _circleHit = Physics2D.OverlapCircle(circlePos, _radiusCheck, _layerMask);
    }

    private Rigidbody2D _rigidbody;
    private RaycastHit2D _hit;
    private Collider2D _circleHit;

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_rigidbody.position, Vector2.down * _checkDistance);
        Gizmos.DrawWireSphere(_rigidbody.position + Vector2.down * _circleCheckOffset, _radiusCheck);
    }
}