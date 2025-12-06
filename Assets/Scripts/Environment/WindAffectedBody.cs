using UnityEngine;

/// <summary>
/// Простая реализация IWindAffected для объектов с Rigidbody2D.
/// Добавь на любой объект с Rigidbody2D, чтобы на него действовал ветер.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class WindAffectedBody : MonoBehaviour, IWindAffected
{
    [Tooltip("Множитель силы ветра для этого объекта (1 = стандартно, 0.5 = слабее, 2 = сильнее)")]
    [SerializeField] private float _windMultiplier = 1f;
    
    [Tooltip("Режим применения силы")]
    [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Force;
    
    private Rigidbody2D _rigidbody;
    
    public float WindMultiplier
    {
        get => _windMultiplier;
        set => _windMultiplier = value;
    }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    public void ApplyWind(Vector2 force)
    {
        if (_rigidbody == null || _rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;
            
        _rigidbody.AddForce(force * _windMultiplier, _forceMode);
    }
}

