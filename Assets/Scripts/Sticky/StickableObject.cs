using Character;
using UnityEngine;

/// <summary>
/// Base implementation of IStickable. Uses parenting to attach objects.
/// </summary>
public class StickableObject : MonoBehaviour, IStickable, IPurifiable
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float purifyDestroyTime = 3f;
    
    private Transform _originalParent;
    private bool _isStuck;
    private bool _wasReleased;
    private StickyController _stuckTo;
    private RigidbodyData _savedRigidbodyData;
    private GameObject _rigidbodyOwner;
    
    private Transform Target => _targetTransform != null ? _targetTransform : transform;
    
    public Rigidbody2D Rigidbody => _rigidbody;
    public GameObject GameObject => gameObject;
    public bool IsStuck => _isStuck;
    public bool CanStick => !_isStuck && !_wasReleased;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null)
            Debug.LogWarning($"[{nameof(StickableObject)}] Rigidbody is not assigned on {gameObject.name}", this);
    }
#endif

    private void Awake()
    {
        _originalParent = Target.parent;
        if (_rigidbody != null)
            _rigidbodyOwner = _rigidbody.gameObject;
    }
    
    public void OnStick(StickyController source, Transform parent, Vector2 stickPoint)
    {
        if (_isStuck || _wasReleased)
            return;
        
        if (parent == null)
        {
            Debug.LogWarning($"[{nameof(StickableObject)}] Cannot stick - parent is null", this);
            return;
        }
        
        _isStuck = true;
        
        if (_rigidbody != null)
        {
            _savedRigidbodyData = new RigidbodyData(_rigidbody);
            Object.Destroy(_rigidbody);
            _rigidbody = null;
        }
        
        Target.SetParent(parent);
        _stuckTo = source;
    }

    public void OnRelease()
    {
        if (!_isStuck)
            return;
            
        _isStuck = false;
        _wasReleased = true;
        
        Target.SetParent(_originalParent);
        
        if (_rigidbodyOwner != null)
        {
            _rigidbody = _rigidbodyOwner.AddComponent<Rigidbody2D>();
            _savedRigidbodyData.ApplyTo(_rigidbody);
        }
            
        _stuckTo = null;
    }
    
    public void ResetCanStick()
    {
        _wasReleased = false;
    }
    
    private void OnDestroy()
    {
        if (_isStuck && _stuckTo != null)
            _stuckTo.NotifyObjectDestroyed(this);
    }
    
    private struct RigidbodyData
    {
        public float Mass;
        public float Drag;
        public float AngularDrag;
        public float GravityScale;
        public RigidbodyType2D BodyType;
        public RigidbodyConstraints2D Constraints;
        public CollisionDetectionMode2D CollisionDetection;
        public RigidbodySleepMode2D SleepMode;
        public RigidbodyInterpolation2D Interpolation;
        public bool FreezeRotation;
        public PhysicsMaterial2D Material;
        
        public RigidbodyData(Rigidbody2D rb)
        {
            Mass = rb.mass;
            Drag = rb.drag;
            AngularDrag = rb.angularDrag;
            GravityScale = rb.gravityScale;
            BodyType = rb.bodyType;
            Constraints = rb.constraints;
            CollisionDetection = rb.collisionDetectionMode;
            SleepMode = rb.sleepMode;
            Interpolation = rb.interpolation;
            FreezeRotation = rb.freezeRotation;
            Material = rb.sharedMaterial;
        }
        
        public void ApplyTo(Rigidbody2D rb)
        {
            rb.mass = Mass;
            rb.drag = Drag;
            rb.angularDrag = AngularDrag;
            rb.gravityScale = GravityScale;
            rb.bodyType = BodyType;
            rb.constraints = Constraints;
            rb.collisionDetectionMode = CollisionDetection;
            rb.sleepMode = SleepMode;
            rb.interpolation = Interpolation;
            rb.freezeRotation = FreezeRotation;
            rb.sharedMaterial = Material;
        }
    }

    public void Purify()
    {
        if (!_isStuck)
            return;
        
        _stuckTo.Release(this);
        _collider.enabled = false;
        
        Destroy(gameObject, purifyDestroyTime);
    }

    public bool IsPureAlready()
    {
        return !_isStuck;
    }
}
