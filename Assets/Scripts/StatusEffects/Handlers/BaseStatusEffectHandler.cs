using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс для обработчиков статус-эффектов.
/// </summary>
public abstract class BaseStatusEffectHandler : MonoBehaviour, IStatusEffectReceiver, ILoggable
{
    public abstract string LogCategory { get; }
    
    /// <summary>
    /// Эффект впервые применён
    /// </summary>
    public event Action<StatusEffect> EffectApplied;
    
    /// <summary>
    /// Эффект обновлён (refresh)
    /// </summary>
    public event Action<StatusEffect> EffectRefreshed;
    
    /// <summary>
    /// Эффект истёк или удалён
    /// </summary>
    public event Action<StatusEffect> EffectExpired;
    
    private readonly Dictionary<StatusEffect, ActiveStatusEffect> _effects = new Dictionary<StatusEffect, ActiveStatusEffect>();
    private readonly List<StatusEffect> _toRemove = new List<StatusEffect>();
    
    public GameObject GameObject => gameObject;
    public StatusEffectModifiers CurrentModifiers { get; private set; } = StatusEffectModifiers.Default;
    public bool HasAnyEffect => _effects.Count > 0;
    
    protected virtual void Update()
    {
        if (_effects.Count == 0)
        {
            if (!CurrentModifiers.Equals(StatusEffectModifiers.Default))
            {
                CurrentModifiers = StatusEffectModifiers.Default;
                OnModifiersChanged();
            }
            return;
        }
        
        _toRemove.Clear();
        var combined = StatusEffectModifiers.Default;
        
        foreach (var kvp in _effects)
        {
            var active = kvp.Value;
            bool justExpired = active.Update(Time.deltaTime);
            
            if (active.IsExpired)
            {
                _toRemove.Add(kvp.Key);
                if (justExpired)
                {
                    EffectExpired?.Invoke(kvp.Key);
                    OnEffectExpired(kvp.Key);
                }
            }
            else
            {
                combined = StatusEffectModifiers.Combine(combined, active.GetCurrentModifiers());
            }
        }
        
        foreach (var key in _toRemove)
        {
            _effects.Remove(key);
        }
        
        if (!CurrentModifiers.Equals(combined))
        {
            CurrentModifiers = combined;
            OnModifiersChanged();
        }
    }
    
    public void ApplyEffect(StatusEffect effect)
    {
        if (effect == null) return;
        
        if (_effects.TryGetValue(effect, out var existing))
        {
            existing.Refresh();
            EffectRefreshed?.Invoke(effect);
        }
        else
        {
            _effects[effect] = new ActiveStatusEffect(effect);
            EffectApplied?.Invoke(effect);
            OnEffectApplied(effect);
        }
    }
    
    public void ApplyEffect(StatusEffect effect, float duration)
    {
        if (effect == null) return;
        
        if (_effects.TryGetValue(effect, out var existing))
        {
            existing.Refresh(duration);
            EffectRefreshed?.Invoke(effect);
        }
        else
        {
            _effects[effect] = new ActiveStatusEffect(effect, duration);
            EffectApplied?.Invoke(effect);
            OnEffectApplied(effect);
        }
    }
    
    public void RemoveEffect(StatusEffect effect)
    {
        if (effect == null) return;
        
        if (_effects.Remove(effect))
        {
            this.LogInfo($"Removed: {effect.DisplayName}");
            EffectExpired?.Invoke(effect);
            OnEffectExpired(effect);
        }
    }
    
    public StatusEffectModifiers GetModifiers() => CurrentModifiers;
    
    public bool HasEffect(StatusEffect effect) => _effects.ContainsKey(effect);
    
    protected virtual void OnEffectApplied(StatusEffect effect) { }
    protected virtual void OnEffectExpired(StatusEffect effect) { }
    protected virtual void OnModifiersChanged() { }
}
