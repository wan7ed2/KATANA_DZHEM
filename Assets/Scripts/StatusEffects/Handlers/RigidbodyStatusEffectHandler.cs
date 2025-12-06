using UnityEngine;

/// <summary>
/// Обработчик статус-эффектов для физических объектов.
/// Модифицирует drag на основе SpeedMultiplier.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyStatusEffectHandler : BaseStatusEffectHandler
{
    public override string LogCategory => "StatusEffect.Rigidbody";
    
    private Rigidbody2D _rigidbody;
    private float _originalDrag;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _originalDrag = _rigidbody.drag;
    }
    
    protected override void OnModifiersChanged()
    {
        var speedMult = CurrentModifiers.SpeedMultiplier;
        
        if (speedMult < 0.01f)
            speedMult = 0.01f;
        
        _rigidbody.drag = _originalDrag / speedMult;
    }
}
