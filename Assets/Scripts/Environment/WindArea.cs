using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Зона ветра. Можно размещать несколько на уровне.
/// Сдувает объекты, реализующие IWindAffected.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WindArea : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("Направление ветра (нормализуется автоматически)")]
    [SerializeField] private Vector2 _windDirection = Vector2.right;
    
    [Tooltip("Сила ветра")]
    [SerializeField] private float _windForce = 10f;
    
    [Header("State")]
    [Tooltip("Активна ли зона")]
    [SerializeField] private bool _isActive = true;
    
    [Header("Gizmo")]
    [SerializeField] private Color _gizmoColor = new Color(0.5f, 0.8f, 1f, 0.3f);
    [SerializeField] private Color _arrowColor = new Color(0.2f, 0.6f, 1f, 1f);
    
    // === Events ===
    /// <summary>
    /// Событие активации зоны. Параметр - направление ветра.
    /// </summary>
    public event Action<Vector2> OnActivated;
    
    /// <summary>
    /// Событие деактивации зоны.
    /// </summary>
    public event Action OnDeactivated;
    
    /// <summary>
    /// Событие изменения параметров ветра. Параметры: направление, сила.
    /// </summary>
    public event Action<Vector2, float> OnWindChanged;
    
    // === Properties ===
    /// <summary>
    /// Направление ветра (нормализованное)
    /// </summary>
    public Vector2 WindDirection
    {
        get => _windDirection.normalized;
        set
        {
            _windDirection = value.normalized;
            OnWindChanged?.Invoke(_windDirection, _windForce);
        }
    }
    
    /// <summary>
    /// Сила ветра
    /// </summary>
    public float WindForce
    {
        get => _windForce;
        set
        {
            _windForce = Mathf.Max(0f, value);
            OnWindChanged?.Invoke(_windDirection, _windForce);
        }
    }
    
    /// <summary>
    /// Активна ли зона
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            
            if (_isActive)
                OnActivated?.Invoke(WindDirection);
            else
            {
                _affectedObjects.Clear();
                OnDeactivated?.Invoke();
            }
        }
    }
    
    private HashSet<IWindAffected> _affectedObjects = new HashSet<IWindAffected>();
    private Collider2D _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
    }
    
    private void FixedUpdate()
    {
        if (!_isActive) return;
        
        var force = WindDirection * _windForce;
        
        foreach (var obj in _affectedObjects)
        {
            obj?.ApplyWind(force);
        }
    }
    
    // === Public API ===
    
    /// <summary>
    /// Включить зону
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }
    
    /// <summary>
    /// Выключить зону
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
    
    /// <summary>
    /// Переключить состояние зоны
    /// </summary>
    public void Toggle()
    {
        IsActive = !IsActive;
    }
    
    /// <summary>
    /// Установить параметры ветра
    /// </summary>
    public void SetWind(Vector2 direction, float force)
    {
        _windDirection = direction.normalized;
        _windForce = Mathf.Max(0f, force);
        OnWindChanged?.Invoke(_windDirection, _windForce);
    }
    
    /// <summary>
    /// Установить направление ветра по углу (в градусах, 0 = вправо)
    /// </summary>
    public void SetWindAngle(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        WindDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    
    // === Trigger Handling ===
    
    private IWindAffected FindWindAffected(GameObject obj)
    {
        if (obj.TryGetComponent<ComponentLink>(out var link))
        {
            if (link.TryGetLinked<IWindAffected>(out var linked))
                return linked;
        }
        
        if (obj.TryGetComponent<IWindAffected>(out var direct))
            return direct;
            
        return obj.GetComponentInParent<IWindAffected>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        var affected = FindWindAffected(other.gameObject);
        if (affected != null) _affectedObjects.Add(affected);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isActive) return;
        var affected = FindWindAffected(other.gameObject);
        if (affected != null) _affectedObjects.Add(affected);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var affected = FindWindAffected(other.gameObject);
        if (affected != null) _affectedObjects.Remove(affected);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_windDirection.sqrMagnitude < 0.01f)
            _windDirection = Vector2.right;
            
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }
    
    private void OnDrawGizmos()
    {
        DrawZone();
        DrawWindArrow();
    }
    
    private void DrawZone()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        
        float alpha = _isActive ? _gizmoColor.a : 0.1f;
        Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, alpha);
        
        if (col is BoxCollider2D box)
        {
            var center = transform.TransformPoint(box.offset);
            var size = Vector3.Scale(box.size, transform.lossyScale);
            Gizmos.DrawCube(center, size);
            Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, _isActive ? 1f : 0.3f);
            Gizmos.DrawWireCube(center, size);
        }
        else if (col is CircleCollider2D circle)
        {
            var center = transform.TransformPoint(circle.offset);
            var radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            Gizmos.DrawSphere(center, radius);
            Gizmos.color = new Color(_gizmoColor.r, _gizmoColor.g, _gizmoColor.b, _isActive ? 1f : 0.3f);
            Gizmos.DrawWireSphere(center, radius);
        }
        else if (col is PolygonCollider2D)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
    
    private void DrawWindArrow()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        
        float arrowAlpha = _isActive ? 1f : 0.3f;
        Gizmos.color = new Color(_arrowColor.r, _arrowColor.g, _arrowColor.b, arrowAlpha);
        
        Vector3 center = col.bounds.center;
        Vector3 direction = (Vector3)(WindDirection);
        float arrowLength = Mathf.Min(col.bounds.size.x, col.bounds.size.y) * 0.4f;
        arrowLength = Mathf.Max(arrowLength, 0.5f);
        
        Vector3 arrowEnd = center + direction * arrowLength;
        Gizmos.DrawLine(center, arrowEnd);
        
        float headSize = arrowLength * 0.25f;
        Vector3 right = Quaternion.Euler(0, 0, 135) * direction * headSize;
        Vector3 left = Quaternion.Euler(0, 0, -135) * direction * headSize;
        Gizmos.DrawLine(arrowEnd, arrowEnd + right);
        Gizmos.DrawLine(arrowEnd, arrowEnd + left);
        
        var style = new GUIStyle();
        style.normal.textColor = _isActive ? Color.white : Color.gray;
        UnityEditor.Handles.Label(center + Vector3.up * 0.5f, 
            $"Wind: {_windForce:F1}" + (_isActive ? "" : " [OFF]"), style);
    }
#endif
}

