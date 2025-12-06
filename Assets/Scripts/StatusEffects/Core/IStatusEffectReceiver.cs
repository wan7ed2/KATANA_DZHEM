using UnityEngine;

/// <summary>
/// Интерфейс для объектов, которые могут получать статус-эффекты.
/// </summary>
public interface IStatusEffectReceiver
{
    /// <summary>
    /// Применить/обновить эффект. Сбрасывает таймер на Duration.
    /// </summary>
    void ApplyEffect(StatusEffect effect);
    
    /// <summary>
    /// Применить эффект с переопределённой длительностью.
    /// </summary>
    void ApplyEffect(StatusEffect effect, float duration);
    
    /// <summary>
    /// Немедленно удалить эффект.
    /// </summary>
    void RemoveEffect(StatusEffect effect);
    
    /// <summary>
    /// Получить текущие модификаторы.
    /// </summary>
    StatusEffectModifiers GetModifiers();
    
    /// <summary>
    /// Проверить наличие эффекта.
    /// </summary>
    bool HasEffect(StatusEffect effect);
    
    GameObject GameObject { get; }
}
