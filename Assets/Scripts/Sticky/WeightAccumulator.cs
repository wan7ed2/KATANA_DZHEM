using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accumulates weight from stuck objects and applies it to a Rigidbody2D.
/// </summary>
public class WeightAccumulator : MonoBehaviour
{
    [SerializeField] private StickyController _stickyController;
    [SerializeField] private Rigidbody2D _targetRigidbody;
    
    private float _baseMass;
    private float _accumulatedWeight;
    private readonly Dictionary<IStickable, float> _weightPerObject = new Dictionary<IStickable, float>();
    
    public float AccumulatedWeight => _accumulatedWeight;
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
            Debug.LogWarning($"[{nameof(WeightAccumulator)}] StickyController is not assigned on {gameObject.name}", this);
        if (_targetRigidbody == null)
            Debug.LogWarning($"[{nameof(WeightAccumulator)}] Target Rigidbody is not assigned on {gameObject.name}", this);
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
    
    public void SetStickyController(StickyController controller)
    {
        if (_stickyController != null)
        {
            _stickyController.OnObjectStuck -= HandleObjectStuck;
            _stickyController.OnObjectReleased -= HandleObjectReleased;
            _stickyController.OnAllReleased -= HandleAllReleased;
        }
        
        _stickyController = controller;
        
        if (_stickyController != null && enabled)
        {
            _stickyController.OnObjectStuck += HandleObjectStuck;
            _stickyController.OnObjectReleased += HandleObjectReleased;
            _stickyController.OnAllReleased += HandleAllReleased;
        }
    }
    
    private void HandleObjectStuck(IStickable stickable)
    {
        if (stickable == null || stickable.GameObject == null)
            return;
        
        if (!stickable.GameObject.TryGetComponent<IWeightProvider>(out var weightProvider))
            return;
            
        float weight = weightProvider.Weight;
        _weightPerObject[stickable] = weight;
        _accumulatedWeight += weight;
        UpdateMass();
    }
    
    private void HandleObjectReleased(IStickable stickable)
    {
        if (stickable == null)
            return;
            
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
