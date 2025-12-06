using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for objects that can have IStickable objects attached to them.
/// </summary>
public class StickyController : MonoBehaviour
{
    [SerializeField] private Transform _stickParent;
    
    private readonly List<IStickable> _stuckObjects = new List<IStickable>();
    
    public event Action<IStickable> OnObjectStuck;
    public event Action<IStickable> OnObjectReleased;
    public event Action OnAllReleased;
    
    public int StuckCount => _stuckObjects.Count;
    public IReadOnlyList<IStickable> StuckObjects => _stuckObjects;

    private void Awake()
    {
        if (_stickParent == null)
            _stickParent = transform;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TryFindStickable(other.gameObject, out var stickable))
            Stick(stickable, other.transform.position);
    }
    
    private void TryStick(Collision2D collision)
    {
        if (!TryFindStickable(collision.gameObject, out var stickable))
            return;
            
        Vector2 stickPoint = collision.GetContact(0).point;
        Stick(stickable, stickPoint);
    }
    
    private bool TryFindStickable(GameObject obj, out IStickable stickable)
    {
        if (obj.TryGetComponent(out stickable))
            return true;
            
        if (obj.TryGetComponent<ComponentLink>(out var link))
            return link.TryGetLinked(out stickable);
            
        stickable = null;
        return false;
    }
    
    public void Stick(IStickable stickable, Vector2 stickPoint)
    {
        if (!stickable.CanStick || _stuckObjects.Contains(stickable))
            return;
        
        _stuckObjects.Add(stickable);
        stickable.OnStick(this, _stickParent, stickPoint);
        OnObjectStuck?.Invoke(stickable);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stickParent == null)
            _stickParent = transform;
    }
    
    [ContextMenu("Release All Stuck Objects")]
    private void EditorReleaseAll()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can only release objects in Play mode");
            return;
        }
        Reset();
        Debug.Log($"[StickyController] Released all stuck objects");
    }
#endif
    
    public void Release(IStickable stickable)
    {
        if (!_stuckObjects.Contains(stickable))
            return;
            
        _stuckObjects.Remove(stickable);
        stickable.OnRelease();
        OnObjectReleased?.Invoke(stickable);
    }
    
    public void Reset()
    {
        if (_stuckObjects.Count == 0)
            return;
        
        for (int i = _stuckObjects.Count - 1; i >= 0; i--)
        {
            var stickable = _stuckObjects[i];
            stickable.OnRelease();
            OnObjectReleased?.Invoke(stickable);
        }
        
        _stuckObjects.Clear();
        OnAllReleased?.Invoke();
    }
    
    public void NotifyObjectDestroyed(IStickable stickable)
    {
        if (_stuckObjects.Remove(stickable))
            OnObjectReleased?.Invoke(stickable);
    }
}
