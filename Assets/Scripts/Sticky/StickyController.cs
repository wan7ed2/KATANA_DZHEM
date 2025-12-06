using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for objects that can have other IStickable objects attached to them.
/// Only handles sticking logic. Weight is managed separately by WeightAccumulator.
/// </summary>
public class StickyController : MonoBehaviour
{
    private readonly List<IStickable> _stuckObjects = new List<IStickable>();
    
    /// <summary>
    /// Event fired when an object sticks.
    /// </summary>
    public event Action<IStickable> OnObjectStuck;
    
    /// <summary>
    /// Event fired when an object is released.
    /// </summary>
    public event Action<IStickable> OnObjectReleased;
    
    /// <summary>
    /// Event fired when all objects are reset.
    /// </summary>
    public event Action OnAllReleased;
    
    /// <summary>
    /// Number of currently stuck objects.
    /// </summary>
    public int StuckCount => _stuckObjects.Count;
    
    /// <summary>
    /// Read-only access to stuck objects.
    /// </summary>
    public IReadOnlyList<IStickable> StuckObjects => _stuckObjects;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IStickable>(out var stickable))
        {
            // For triggers, use the object's position as the stick point
            Stick(stickable, other.transform.position);
        }
    }
    
    /// <summary>
    /// Try to stick an object from a collision.
    /// </summary>
    private void TryStick(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IStickable>(out var stickable))
            return;
            
        // Use the first contact point as the stick position
        Vector2 stickPoint = collision.GetContact(0).point;
        Stick(stickable, stickPoint);
    }
    
    /// <summary>
    /// Stick an object at the specified point.
    /// </summary>
    public void Stick(IStickable stickable, Vector2 stickPoint)
    {
        if (stickable.IsStuck || _stuckObjects.Contains(stickable))
            return;
            
        _stuckObjects.Add(stickable);
        stickable.OnStick(transform, stickPoint);
        
        OnObjectStuck?.Invoke(stickable);
    }
    
    /// <summary>
    /// Release a specific stuck object.
    /// </summary>
    public void Release(IStickable stickable)
    {
        if (!_stuckObjects.Contains(stickable))
            return;
            
        _stuckObjects.Remove(stickable);
        stickable.OnRelease();
        
        OnObjectReleased?.Invoke(stickable);
    }
    
    /// <summary>
    /// Release all stuck objects and reset the controller.
    /// </summary>
    public void Reset()
    {
        if (_stuckObjects.Count == 0)
            return;
            
        // Release in reverse order to handle any dependencies
        for (int i = _stuckObjects.Count - 1; i >= 0; i--)
        {
            _stuckObjects[i].OnRelease();
        }
        
        _stuckObjects.Clear();
        OnAllReleased?.Invoke();
    }
}

