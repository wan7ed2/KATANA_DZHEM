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
    
    [Header("Particles")]
    [Tooltip("Дочерняя система частиц для визуализации ветра")]
    [SerializeField] private ParticleSystem _windParticles;
    
    [Tooltip("Базовая скорость частиц при силе ветра = 1")]
    [SerializeField] private float _particleSpeedMultiplier = 0.05f;
    
    [Tooltip("Базовый rate частиц")]
    [SerializeField] private float _particleEmissionRate = 20f;
    
    [Tooltip("Базовый угол поворота частиц (при направлении влево)")]
    [SerializeField] private float _particleBaseRotation = -45f;
    
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
            SyncParticleSystem();
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
            SyncParticleSpeed();
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
            
            UpdateParticleState();
        }
    }
    
    private HashSet<IWindAffected> _affectedObjects = new HashSet<IWindAffected>();
    private Collider2D _collider;
    
    // Кэшированные модули частиц
    private ParticleSystem.ShapeModule _particleShape;
    private ParticleSystem.MainModule _particleMain;
    private ParticleSystem.EmissionModule _particleEmission;
    private Transform _particleTransform;
    private bool _particlesInitialized;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
        
        InitializeParticleSystem();
    }
    
    private void Start()
    {
        // Синхронизируем частицы с начальными настройками
        SyncParticleSystem();
        UpdateParticleState();
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
    
    // === Particle System Integration ===
    
    /// <summary>
    /// Инициализация системы частиц
    /// </summary>
    private void InitializeParticleSystem()
    {
        // Если не указана явно, ищем в дочерних объектах
        if (_windParticles == null)
            _windParticles = GetComponentInChildren<ParticleSystem>();
            
        if (_windParticles != null)
        {
            _particleShape = _windParticles.shape;
            _particleMain = _windParticles.main;
            _particleEmission = _windParticles.emission;
            _particleTransform = _windParticles.transform;
            _particlesInitialized = true;
        }
    }
    
    /// <summary>
    /// Синхронизирует систему частиц с настройками зоны ветра
    /// </summary>
    public void SyncParticleSystem()
    {
        if (!_particlesInitialized) return;
        
        SyncParticleShape();
        SyncParticleDirection();
        SyncParticleSpeed();
        SyncParticleRotation();
    }
    
    /// <summary>
    /// Синхронизирует размер и позицию эмиттера с размером зоны
    /// </summary>
    private void SyncParticleShape()
    {
        if (!_particlesInitialized || _collider == null) return;
        
        if (_collider is BoxCollider2D box)
        {
            var size = box.size;
            var offset = box.offset;
            var direction = WindDirection;
            
            // Определяем размер эмиттера - по высоте зоны (перпендикулярно ветру)
            // Для Edge shape: scale.x = длина линии эмиттера
            float edgeLength;
            Vector3 emitterOffset;
            
            // Вычисляем, какой размер использовать для эмиттера
            // Если ветер горизонтальный - используем высоту, если вертикальный - ширину
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            
            if (absX >= absY)
            {
                // Преимущественно горизонтальный ветер
                edgeLength = size.y;
                // Эмиттер располагается на стороне, откуда дует ветер
                float offsetX = (direction.x > 0 ? -size.x : size.x) / 2f;
                emitterOffset = new Vector3(offset.x + offsetX, offset.y, 0);
            }
            else
            {
                // Преимущественно вертикальный ветер
                edgeLength = size.x;
                float offsetY = (direction.y > 0 ? -size.y : size.y) / 2f;
                emitterOffset = new Vector3(offset.x, offset.y + offsetY, 0);
            }
            
            // Устанавливаем scale для Edge shape
            _particleShape.scale = new Vector3(edgeLength, 1f, 1f);
            
            // Позиционируем эмиттер
            _particleTransform.localPosition = emitterOffset;
        }
        else if (_collider is CircleCollider2D circle)
        {
            // Для круглого коллайдера используем Arc или Edge по диаметру
            var radius = circle.radius;
            var offset = circle.offset;
            var direction = WindDirection;
            
            _particleShape.scale = new Vector3(radius * 2f, 1f, 1f);
            
            // Позиция эмиттера - на краю круга, откуда дует ветер
            Vector3 emitterOffset = new Vector3(
                offset.x - direction.x * radius,
                offset.y - direction.y * radius,
                0
            );
            _particleTransform.localPosition = emitterOffset;
        }
    }
    
    /// <summary>
    /// Синхронизирует направление частиц с направлением ветра
    /// </summary>
    private void SyncParticleDirection()
    {
        if (!_particlesInitialized) return;
        
        var direction = WindDirection;
        
        // Вычисляем угол направления ветра
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Для Edge shape rotation.z определяет ориентацию линии эмиттера
        // А rotation.x/y определяют направление вылета частиц
        // Edge shape испускает частицы перпендикулярно линии, нам нужно развернуть так,
        // чтобы частицы летели в направлении ветра
        
        // Линия эмиттера должна быть перпендикулярна направлению ветра
        // Частицы вылетают "вверх" от линии (по локальной Y)
        // Значит нам нужно повернуть shape так, чтобы его локальная Y смотрела в направлении ветра
        _particleShape.rotation = new Vector3(0, 0, angle - 90f);
    }
    
    /// <summary>
    /// Синхронизирует скорость частиц с силой ветра
    /// </summary>
    private void SyncParticleSpeed()
    {
        if (!_particlesInitialized) return;
        
        // Скорость частиц пропорциональна силе ветра
        float speed = _windForce * _particleSpeedMultiplier;
        _particleMain.startSpeed = Mathf.Max(0.5f, speed);
        
        // Emission rate - больше частиц для более сильного ветра
        float emissionMultiplier = Mathf.Clamp(_windForce / 100f, 0.5f, 3f);
        _particleEmission.rateOverTime = _particleEmissionRate * emissionMultiplier;
        
        // Время жизни частицы - чтобы она пролетала через зону
        // Рассчитываем на основе размера зоны
        float zoneSize = 10f; // дефолт
        if (_collider is BoxCollider2D box)
        {
            var direction = WindDirection;
            zoneSize = Mathf.Abs(direction.x) >= Mathf.Abs(direction.y) ? box.size.x : box.size.y;
        }
        else if (_collider is CircleCollider2D circle)
        {
            zoneSize = circle.radius * 2f;
        }
        
        float lifetime = zoneSize / Mathf.Max(0.1f, speed);
        _particleMain.startLifetime = Mathf.Clamp(lifetime, 0.5f, 10f);
    }
    
    /// <summary>
    /// Синхронизирует поворот частиц с направлением ветра
    /// </summary>
    private void SyncParticleRotation()
    {
        if (!_particlesInitialized) return;
        
        var direction = WindDirection;
        
        // Вычисляем угол направления ветра (0 = вправо, 180/-180 = влево)
        float windAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Базовый угол задан для направления влево (180 градусов)
        // Смещаем относительно этого базового направления
        // windAngle = 180 (влево) -> rotation = _particleBaseRotation
        // windAngle = 0 (вправо) -> rotation = _particleBaseRotation + 180
        float rotation = _particleBaseRotation + (windAngle - 180f);
        
        // Нормализуем угол
        while (rotation > 180f) rotation -= 360f;
        while (rotation < -180f) rotation += 360f;
        
        _particleMain.startRotation = rotation * Mathf.Deg2Rad;
    }
    
    /// <summary>
    /// Обновляет состояние системы частиц (вкл/выкл)
    /// </summary>
    private void UpdateParticleState()
    {
        if (!_particlesInitialized) return;
        
        if (_isActive)
        {
            if (!_windParticles.isPlaying)
                _windParticles.Play();
        }
        else
        {
            if (_windParticles.isPlaying)
                _windParticles.Stop();
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
        SyncParticleSystem();
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
            
        // Синхронизируем частицы в редакторе
        if (Application.isPlaying && _particlesInitialized)
        {
            SyncParticleSystem();
            UpdateParticleState();
        }
        else if (!Application.isPlaying)
        {
            // В редакторе тоже пытаемся синхронизировать для превью
            EditorSyncParticles();
        }
    }
    
    /// <summary>
    /// Синхронизация частиц в режиме редактора (для превью)
    /// </summary>
    private void EditorSyncParticles()
    {
        var ps = _windParticles;
        if (ps == null)
            ps = GetComponentInChildren<ParticleSystem>();
            
        if (ps == null) return;
        
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        
        var shape = ps.shape;
        var main = ps.main;
        var direction = _windDirection.normalized;
        
        if (col is BoxCollider2D box)
        {
            var size = box.size;
            var offset = box.offset;
            
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            
            float edgeLength;
            Vector3 emitterOffset;
            
            if (absX >= absY)
            {
                edgeLength = size.y;
                float offsetX = (direction.x > 0 ? -size.x : size.x) / 2f;
                emitterOffset = new Vector3(offset.x + offsetX, offset.y, 0);
            }
            else
            {
                edgeLength = size.x;
                float offsetY = (direction.y > 0 ? -size.y : size.y) / 2f;
                emitterOffset = new Vector3(offset.x, offset.y + offsetY, 0);
            }
            
            shape.scale = new Vector3(edgeLength, 1f, 1f);
            ps.transform.localPosition = emitterOffset;
            
            // Расчёт скорости и времени жизни
            float speed = _windForce * _particleSpeedMultiplier;
            main.startSpeed = Mathf.Max(0.5f, speed);
            
            // Emission rate
            var emission = ps.emission;
            float emissionMultiplier = Mathf.Clamp(_windForce / 100f, 0.5f, 3f);
            emission.rateOverTime = _particleEmissionRate * emissionMultiplier;
            
            float zoneSize = absX >= absY ? size.x : size.y;
            float lifetime = zoneSize / Mathf.Max(0.1f, speed);
            main.startLifetime = Mathf.Clamp(lifetime, 0.5f, 10f);
        }
        else if (col is CircleCollider2D circle)
        {
            var radius = circle.radius;
            var offset = circle.offset;
            
            shape.scale = new Vector3(radius * 2f, 1f, 1f);
            
            Vector3 emitterOffset = new Vector3(
                offset.x - direction.x * radius,
                offset.y - direction.y * radius,
                0
            );
            ps.transform.localPosition = emitterOffset;
            
            float speed = _windForce * _particleSpeedMultiplier;
            main.startSpeed = Mathf.Max(0.5f, speed);
            
            var emission = ps.emission;
            float emissionMultiplier = Mathf.Clamp(_windForce / 100f, 0.5f, 3f);
            emission.rateOverTime = _particleEmissionRate * emissionMultiplier;
            
            float lifetime = (radius * 2f) / Mathf.Max(0.1f, speed);
            main.startLifetime = Mathf.Clamp(lifetime, 0.5f, 10f);
        }
        
        // Направление shape
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shape.rotation = new Vector3(0, 0, angle - 90f);
        
        // Start rotation частиц
        float particleRotation = _particleBaseRotation + (angle - 180f);
        while (particleRotation > 180f) particleRotation -= 360f;
        while (particleRotation < -180f) particleRotation += 360f;
        main.startRotation = particleRotation * Mathf.Deg2Rad;
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


