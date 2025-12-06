using UnityEngine;

/// <summary>
/// Base implementation of IStickable. Uses FixedJoint2D to attach while keeping physics active.
/// </summary>
public class StickableObject : MonoBehaviour, IStickable
{
    [SerializeField] private Rigidbody2D _rigidbody;
    
    private FixedJoint2D _joint;
    private bool _isStuck;
    private StickyController _stuckTo;
    
    public Rigidbody2D Rigidbody => _rigidbody;
    public GameObject GameObject => gameObject;
    public bool IsStuck => _isStuck;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null)
            Debug.LogWarning($"[{nameof(StickableObject)}] Rigidbody is not assigned on {gameObject.name}", this);
    }
#endif
    
    public void OnStick(Rigidbody2D targetRigidbody, Vector2 stickPoint)
    {
        if (_isStuck)
            return;
        
        if (targetRigidbody == null)
        {
            Debug.LogWarning($"[{nameof(StickableObject)}] Cannot stick - target Rigidbody2D is null", this);
            return;
        }
        
        _stuckTo = targetRigidbody.GetComponentInChildren<StickyController>();
        _isStuck = true;
        
        _joint = _rigidbody.gameObject.AddComponent<FixedJoint2D>();
        _joint.connectedBody = targetRigidbody;
        _joint.breakForce = Mathf.Infinity;
        _joint.breakTorque = Mathf.Infinity;
        _joint.autoConfigureConnectedAnchor = true;
    }
    
    public void OnRelease()
    {
        if (!_isStuck)
            return;
            
        _isStuck = false;
        _stuckTo = null;
        
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }
    }
    
    private void OnDestroy()
    {
        if (_isStuck && _stuckTo != null)
            _stuckTo.NotifyObjectDestroyed(this);
        
        if (_joint != null)
            Destroy(_joint);
    }
}
