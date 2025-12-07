using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public event Action OnGrounded;
    public event Action OnFly;
    
    [SerializeField] private float _checkDistance;
    [SerializeField] private float _radiusCheck;
    [SerializeField] private float _circleCheckOffset;
    [SerializeField] private LayerMask _layerMask;

    public bool IsGrounded => _isGrounded;
    
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

        var wasGrounded = _isGrounded;
        _isGrounded = _hit.collider != null || _circleHit != null;

        if (wasGrounded && !_isGrounded) OnFly?.Invoke();
        else if (!wasGrounded && _isGrounded) OnGrounded?.Invoke();
    }

    private Rigidbody2D _rigidbody;
    private RaycastHit2D _hit;
    private Collider2D _circleHit;

    private bool _isGrounded;

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_rigidbody.position, Vector2.down * _checkDistance);
        Gizmos.DrawWireSphere(_rigidbody.position + Vector2.down * _circleCheckOffset, _radiusCheck);
    }
}