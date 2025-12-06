using UnityEngine;

/// <summary>
/// Зона, которая применяет статус-эффект находящимся в ней объектам.
/// Пока объект внутри — эффект постоянно обновляется (таймер сбрасывается).
/// Вышел — таймер тикает и эффект истекает.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class StatusEffectZone : MonoBehaviour
{
    [SerializeField] private StatusEffect _effect;
    [SerializeField] private bool _useTrigger = true;
    
    public StatusEffect Effect => _effect;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_effect == null)
            Debug.LogWarning($"[{nameof(StatusEffectZone)}] Effect is not assigned on {gameObject.name}", this);
        
        var collider = GetComponent<Collider2D>();
        if (collider != null && _useTrigger)
            collider.isTrigger = true;
    }
#endif
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_useTrigger) return;
        ApplyToObject(other.gameObject);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_useTrigger) return;
        ApplyToObject(other.gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_useTrigger) return;
        ApplyToObject(collision.gameObject);
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_useTrigger) return;
        ApplyToObject(collision.gameObject);
    }
    
    private IStatusEffectReceiver FindReceiver(GameObject obj)
    {
        if (obj.TryGetComponent<ComponentLink>(out var link))
        {
            if (link.TryGetLinked<IStatusEffectReceiver>(out var linked))
                return linked;
        }
        
        if (obj.TryGetComponent<IStatusEffectReceiver>(out var direct))
            return direct;
        
        return null;
    }
    
    private void ApplyToObject(GameObject obj)
    {
        if (_effect == null) return;
        FindReceiver(obj)?.ApplyEffect(_effect);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_effect == null) return;
        
        Gizmos.color = _effect.GizmoColor;
        var col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
#endif
}
