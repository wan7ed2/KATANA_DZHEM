using UnityEngine;

/// <summary>
/// Applies a boost to the player based on mace swing momentum.
/// Triggers when the mace completes a swing (angular velocity changes direction).
/// </summary>
public class MaceSwingBooster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _playerRigidbody;
    [SerializeField] private Rigidbody2D _maceRigidbody;
    [SerializeField] private Transform _maceTransform;
    [SerializeField] private Transform _playerTransform;
    
    [Tooltip("The stick rigidbody - rotation happens here via HingeJoint2D")]
    [SerializeField] private Rigidbody2D _stickRigidbody;
    
    [Header("Swing Boost Settings")]
    [Tooltip("Minimum angular velocity of the stick to consider as a swing (degrees/sec)")]
    [SerializeField] private float _minAngularVelocityThreshold = 100f;
    
    [Tooltip("Minimum mace velocity to trigger boost")]
    [SerializeField] private float _minMaceVelocity = 2f;
    
    [Tooltip("Force multiplier applied to player on swing end")]
    [SerializeField] private float _swingBoostForce = 2f;
    
    [Tooltip("How much of the mace velocity contributes to the boost")]
    [SerializeField] private float _velocityContribution = 0.3f;
    
    [Tooltip("Maximum boost force that can be applied")]
    [SerializeField] private float _maxBoostForce = 10f;
    
    [Tooltip("Cooldown between boosts (seconds)")]
    [SerializeField] private float _boostCooldown = 0.2f;
    
    [Header("Jump Compensation")]
    [Tooltip("Reduces the downward pull from mace during jumps")]
    [SerializeField] private bool _compensateMaceWeight = true;
    
    [Tooltip("How much to compensate mace weight (0-1, 1 = full compensation)")]
    [SerializeField] private float _weightCompensation = 0.5f;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug = false;
    
    private float _lastStickAngularVelocity;
    private float _lastMaceSpeed;
    private float _cooldownTimer;
    private bool _wasSwinging;
    private Vector2 _lastMaceVelocity;
    private GroundChecker _groundChecker;

    private void Start()
    {
        _groundChecker = _playerRigidbody.GetComponent<GroundChecker>();
        
        // Try to find stick rigidbody if not assigned
        if (_stickRigidbody == null)
        {
            var stickGO = transform.Find("Stick");
            if (stickGO != null)
                _stickRigidbody = stickGO.GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        if (_maceRigidbody == null || _playerRigidbody == null)
            return;

        _cooldownTimer -= Time.fixedDeltaTime;
        
        ProcessSwingBoost();
        
        if (_compensateMaceWeight)
            CompensateMaceWeight();
        
        // Track stick angular velocity for rotation detection
        if (_stickRigidbody != null)
            _lastStickAngularVelocity = _stickRigidbody.angularVelocity;
        
        _lastMaceVelocity = _maceRigidbody.velocity;
        _lastMaceSpeed = _lastMaceVelocity.magnitude;
    }

    private void ProcessSwingBoost()
    {
        // Get current velocities
        float currentMaceSpeed = _maceRigidbody.velocity.magnitude;
        float currentStickAngular = _stickRigidbody != null ? _stickRigidbody.angularVelocity : 0f;
        float absStickAngular = Mathf.Abs(currentStickAngular);
        
        // Check if we're currently swinging (either by stick rotation or mace movement)
        bool isSwinging = absStickAngular > _minAngularVelocityThreshold || currentMaceSpeed > _minMaceVelocity;
        
        // Detect swing end: was swinging fast, now slowing down significantly OR direction changed
        bool swingEnded = _wasSwinging && 
                          (_cooldownTimer <= 0) &&
                          (
                              // Stick rotation direction changed
                              SignChanged(currentStickAngular, _lastStickAngularVelocity) ||
                              // Mace slowing down significantly
                              (_lastMaceSpeed > _minMaceVelocity && currentMaceSpeed < _lastMaceSpeed * 0.6f)
                          );
        
        if (_showDebug)
        {
            Debug.Log($"[MaceBoost] StickAngular: {currentStickAngular:F1}, MaceSpeed: {currentMaceSpeed:F2}, " +
                      $"IsSwinging: {isSwinging}, WasSwinging: {_wasSwinging}, SwingEnded: {swingEnded}");
        }
        
        if (swingEnded)
        {
            ApplySwingBoost();
            _cooldownTimer = _boostCooldown;
        }
        
        _wasSwinging = isSwinging;
    }

    private void ApplySwingBoost()
    {
        // Calculate boost direction from mace velocity
        Vector2 maceVelocity = _lastMaceVelocity;
        
        if (maceVelocity.sqrMagnitude < 0.1f)
        {
            if (_showDebug) Debug.Log("[MaceBoost] Skipped - mace velocity too low");
            return;
        }
        
        // Direction is based on mace's movement direction
        Vector2 boostDirection = maceVelocity.normalized;
        
        // Calculate boost magnitude based on mace speed
        float maceMagnitude = maceVelocity.magnitude;
        float boostMagnitude = maceMagnitude * _velocityContribution * _swingBoostForce;
        boostMagnitude = Mathf.Min(boostMagnitude, _maxBoostForce);
        
        // Apply boost to player
        Vector2 boost = boostDirection * boostMagnitude;
        _playerRigidbody.AddForce(boost, ForceMode2D.Impulse);
        
        if (_showDebug)
        {
            Debug.Log($"[MaceBoost] BOOST APPLIED! Direction: {boostDirection}, Magnitude: {boostMagnitude:F2}, " +
                      $"MaceVelocity: {maceVelocity}");
        }
        
        Debug.DrawRay(_playerTransform.position, boost, Color.yellow, 0.5f);
    }

    private void CompensateMaceWeight()
    {
        // Only compensate when player is in the air (not grounded)
        if (_groundChecker != null && _groundChecker.IsGrounded)
            return;
        
        // Check if mace is below player
        Vector2 maceRelativePos = _maceTransform.position - _playerTransform.position;
        
        if (maceRelativePos.y < 0)
        {
            // Mace is below player - it's pulling us down
            // Apply upward force to compensate
            float maceWeight = _maceRigidbody.mass * Physics2D.gravity.magnitude;
            float compensation = maceWeight * _weightCompensation;
            
            _playerRigidbody.AddForce(Vector2.up * compensation * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private bool SignChanged(float a, float b)
    {
        return (a > 0 && b < 0) || (a < 0 && b > 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_maceTransform == null || _playerTransform == null)
            return;
        
        // Draw line from player to mace
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_playerTransform.position, _maceTransform.position);
        
        // Draw mace velocity if playing
        if (Application.isPlaying && _maceRigidbody != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_maceTransform.position, _maceRigidbody.velocity * 0.2f);
        }
    }
#endif
}

