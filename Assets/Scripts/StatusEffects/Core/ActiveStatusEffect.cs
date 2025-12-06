using UnityEngine;

/// <summary>
/// Runtime-экземпляр применённого эффекта.
/// Только логика — никакого визуала.
/// </summary>
public class ActiveStatusEffect : ILoggable
{
    public string LogCategory => "StatusEffect";
    
    public StatusEffect Source { get; }
    public bool IsExpired => _remainingTime <= 0f;
    
    /// <summary>
    /// Оставшееся время действия
    /// </summary>
    public float RemainingTime => _remainingTime;
    
    /// <summary>
    /// Прогресс (1 = полная сила, 0 = истёк)
    /// </summary>
    public float Progress => _duration > 0f ? Mathf.Clamp01(_remainingTime / _duration) : 0f;
    
    private float _remainingTime;
    private readonly float _duration;
    
    public ActiveStatusEffect(StatusEffect source, float? overrideDuration = null)
    {
        Source = source;
        _duration = overrideDuration ?? source.Duration;
        _remainingTime = _duration;
        
        this.LogInfo($"{Source.DisplayName} applied, duration: {_duration}s");
    }
    
    /// <summary>
    /// Обновить эффект — сбросить таймер
    /// </summary>
    public void Refresh()
    {
        _remainingTime = _duration;
    }
    
    /// <summary>
    /// Обновить эффект с новой длительностью
    /// </summary>
    public void Refresh(float newDuration)
    {
        _remainingTime = newDuration;
    }
    
    /// <summary>
    /// Тик таймера. Возвращает true если эффект только что истёк.
    /// </summary>
    public bool Update(float deltaTime)
    {
        if (IsExpired) return false;
        
        _remainingTime -= deltaTime;
        
        if (IsExpired)
        {
            _remainingTime = 0f;
            this.LogInfo($"{Source.DisplayName} expired");
            return true; // Только что истёк
        }
        
        return false;
    }
    
    /// <summary>
    /// Текущие модификаторы с учётом fade out
    /// </summary>
    public StatusEffectModifiers GetCurrentModifiers()
    {
        if (IsExpired) 
            return StatusEffectModifiers.Default;
        
        if (!Source.FadeOut)
            return Source.Modifiers;
        
        return StatusEffectModifiers.Lerp(in StatusEffectModifiers.Default, Source.Modifiers, Progress);
    }
}
