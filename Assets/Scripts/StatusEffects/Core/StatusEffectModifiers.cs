using System;
using UnityEngine;

/// <summary>
/// Модификаторы, которые эффект применяет к объекту.
/// </summary>
[Serializable]
public struct StatusEffectModifiers
{
    [Range(0f, 2f)] public float SpeedMultiplier;
    [Range(0f, 2f)] public float JumpMultiplier;
    [Range(0f, 1f)] public float Friction;
    [Range(0f, 2f)] public float AccelerationMultiplier;
    
    private static readonly StatusEffectModifiers _default = new StatusEffectModifiers
    {
        SpeedMultiplier = 1f,
        JumpMultiplier = 1f,
        Friction = 1f,
        AccelerationMultiplier = 1f
    };
    
    public static ref readonly StatusEffectModifiers Default => ref _default;
    
    public static StatusEffectModifiers Combine(in StatusEffectModifiers a, in StatusEffectModifiers b)
    {
        return new StatusEffectModifiers
        {
            SpeedMultiplier = a.SpeedMultiplier * b.SpeedMultiplier,
            JumpMultiplier = a.JumpMultiplier * b.JumpMultiplier,
            Friction = Mathf.Min(a.Friction, b.Friction),
            AccelerationMultiplier = a.AccelerationMultiplier * b.AccelerationMultiplier
        };
    }
    
    public static StatusEffectModifiers Lerp(in StatusEffectModifiers from, in StatusEffectModifiers to, float t)
    {
        return new StatusEffectModifiers
        {
            SpeedMultiplier = Mathf.Lerp(from.SpeedMultiplier, to.SpeedMultiplier, t),
            JumpMultiplier = Mathf.Lerp(from.JumpMultiplier, to.JumpMultiplier, t),
            Friction = Mathf.Lerp(from.Friction, to.Friction, t),
            AccelerationMultiplier = Mathf.Lerp(from.AccelerationMultiplier, to.AccelerationMultiplier, t)
        };
    }
}

