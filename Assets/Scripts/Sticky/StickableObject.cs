using UnityEngine;

/// <summary>
/// Base implementation of IStickable for objects that can stick to StickyController.
/// Add WeightProvider component separately if weight is needed.
/// </summary>
public class StickableObject : MonoBehaviour, IStickable
{
    [SerializeField] private Rigidbody2D _rigidbody;
    
    [Tooltip("If true, the object will be positioned at the exact collision point. If false, maintains its current position relative to the parent.")]
    [SerializeField] private bool _moveToStickPoint = true;
    
    [Tooltip("If true, the object will maintain its world rotation when stuck.")]
    [SerializeField] private bool _preserveRotation = false;
    
    private Transform _originalParent;
    private Vector3 _originalLocalPosition;
    private Quaternion _originalLocalRotation;
    private bool _isStuck;
    private bool _wasSimulated;
    
    public Rigidbody2D Rigidbody => _rigidbody;
    public GameObject GameObject => gameObject;
    public bool IsStuck => _isStuck;
    
    private void Awake()
    {
        _originalParent = transform.parent;
        _originalLocalPosition = transform.localPosition;
        _originalLocalRotation = transform.localRotation;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null)
        {
            Debug.LogWarning($"[{nameof(StickableObject)}] Rigidbody is not assigned on {gameObject.name}", this);
        }
    }
#endif
    
    public void OnStick(Transform parent, Vector2 stickPoint)
    {
        if (_isStuck)
            return;
            
        _isStuck = true;
        
        // Store physics state and disable simulation
        _wasSimulated = _rigidbody.simulated;
        _rigidbody.simulated = false;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        
        // Store world rotation if we need to preserve it
        Quaternion worldRotation = transform.rotation;
        
        // Parent to the target
        transform.SetParent(parent);
        
        // Position at stick point or maintain offset
        if (_moveToStickPoint)
        {
            transform.position = stickPoint;
        }
        
        // Restore world rotation if needed
        if (_preserveRotation)
        {
            transform.rotation = worldRotation;
        }
    }
    
    public void OnRelease()
    {
        if (!_isStuck)
            return;
            
        _isStuck = false;
        
        // Restore parent
        transform.SetParent(_originalParent);
        
        // Restore physics simulation
        _rigidbody.simulated = _wasSimulated;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }
    
    /// <summary>
    /// Resets the object to its original local position and rotation (relative to original parent).
    /// </summary>
    public void ResetToOriginal()
    {
        OnRelease();
        transform.localPosition = _originalLocalPosition;
        transform.localRotation = _originalLocalRotation;
    }
}

