using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that accumulates weight from stuck objects and applies it to a Rigidbody2D.
/// Listens to StickyController events and checks for IWeightProvider on stuck objects.
/// </summary>
public class WeightAccumulator : MonoBehaviour
{
    [SerializeField] private StickyController _stickyController;
    [SerializeField] private Rigidbody2D _targetRigidbody;
    
    private float _baseMass;
    private float _accumulatedWeight;
    private readonly Dictionary<IStickable, float> _weightPerObject = new Dictionary<IStickable, float>();
    
    /// <summary>
    /// Total accumulated weight from all stuck objects with IWeightProvider.
    /// </summary>
    public float AccumulatedWeight => _accumulatedWeight;
    
    /// <summary>
    /// Current total mass (base + accumulated).
    /// </summary>
    public float TotalMass => _baseMass + _accumulatedWeight;
    
    private void Awake()
    {
        if (_targetRigidbody != null)
            _baseMass = _targetRigidbody.mass;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stickyController == null)
        {
            Debug.LogWarning($"[{nameof(WeightAccumulator)}] StickyController is not assigned on {gameObject.name}", this);
        }
        if (_targetRigidbody == null)
        {
            Debug.LogWarning($"[{nameof(WeightAccumulator)}] Target Rigidbody is not assigned on {gameObject.name}", this);
        }
    }
#endif
    
    private void OnEnable()
    {
        if (_stickyController != null)
        {
            _stickyController.OnObjectStuck += HandleObjectStuck;
            _stickyController.OnObjectReleased += HandleObjectReleased;
            _stickyController.OnAllReleased += HandleAllReleased;
        }
    }
    
    private void OnDisable()
    {
        if (_stickyController != null)
        {
            _stickyController.OnObjectStuck -= HandleObjectStuck;
            _stickyController.OnObjectReleased -= HandleObjectReleased;
            _stickyController.OnAllReleased -= HandleAllReleased;
        }
    }
    
    /// <summary>
    /// Set the StickyController to listen to at runtime.
    /// </summary>
    public void SetStickyController(StickyController controller)
    {
        // Unsubscribe from old controller
        if (_stickyController != null)
        {
            _stickyController.OnObjectStuck -= HandleObjectStuck;
            _stickyController.OnObjectReleased -= HandleObjectReleased;
            _stickyController.OnAllReleased -= HandleAllReleased;
        }
        
        _stickyController = controller;
        
        // Subscribe to new controller
        if (_stickyController != null && enabled)
        {
            _stickyController.OnObjectStuck += HandleObjectStuck;
            _stickyController.OnObjectReleased += HandleObjectReleased;
            _stickyController.OnAllReleased += HandleAllReleased;
        }
    }
    
    private void HandleObjectStuck(IStickable stickable)
    {
        // Check if the stuck object has a weight provider
        if (!stickable.GameObject.TryGetComponent<IWeightProvider>(out var weightProvider))
            return;
            
        float weight = weightProvider.Weight;
        _weightPerObject[stickable] = weight;
        _accumulatedWeight += weight;
        
        UpdateMass();
    }
    
    private void HandleObjectReleased(IStickable stickable)
    {
        if (!_weightPerObject.TryGetValue(stickable, out float weight))
            return;
            
        _weightPerObject.Remove(stickable);
        _accumulatedWeight -= weight;
        
        UpdateMass();
    }
    
    private void HandleAllReleased()
    {
        _weightPerObject.Clear();
        _accumulatedWeight = 0f;
        
        UpdateMass();
    }
    
    /// <summary>
    /// Reset accumulated weight without releasing objects.
    /// </summary>
    public void ResetWeight()
    {
        _weightPerObject.Clear();
        _accumulatedWeight = 0f;
        UpdateMass();
    }
    
    private void UpdateMass()
    {
        if (_targetRigidbody == null)
            return;
            
        _targetRigidbody.mass = _baseMass + _accumulatedWeight;
    }
}

