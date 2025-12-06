using UnityEngine;

/// <summary>
/// Данные статус-эффекта (ScriptableObject).
/// Может применяться от любого источника: зона, дротик, способность и т.д.
/// </summary>
[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Status Effects/Status Effect")]
public class StatusEffect : ScriptableObject
{
    [Header("Display")]
    public string DisplayName;
    public Color GizmoColor = Color.cyan;
    
    [Header("Modifiers")]
    public StatusEffectModifiers Modifiers = new StatusEffectModifiers
    {
        SpeedMultiplier = 1f,
        JumpMultiplier = 1f,
        Friction = 1f,
        AccelerationMultiplier = 1f
    };
    
    [Header("Duration")]
    [Tooltip("Как долго эффект держится. Refresh сбрасывает таймер.")]
    [Min(0.1f)] public float Duration = 1f;
    
    [Tooltip("Если true — модификаторы плавно затухают к концу действия")]
    public bool FadeOut = false;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Защита от нулевых значений
        if (Modifiers.SpeedMultiplier <= 0f)
        {
            Debug.LogWarning($"[{DisplayName}] SpeedMultiplier is {Modifiers.SpeedMultiplier}, should be > 0. Setting to 0.01");
            var m = Modifiers;
            m.SpeedMultiplier = 0.01f;
            Modifiers = m;
        }
        
        if (Modifiers.AccelerationMultiplier <= 0f)
        {
            Debug.LogWarning($"[{DisplayName}] AccelerationMultiplier is {Modifiers.AccelerationMultiplier}, should be > 0. Setting to 0.01");
            var m = Modifiers;
            m.AccelerationMultiplier = 0.01f;
            Modifiers = m;
        }
        
        if (Modifiers.JumpMultiplier <= 0f)
        {
            Debug.LogWarning($"[{DisplayName}] JumpMultiplier is {Modifiers.JumpMultiplier}, should be > 0. Setting to 0.01");
            var m = Modifiers;
            m.JumpMultiplier = 0.01f;
            Modifiers = m;
        }
    }
    
    [ContextMenu("Reset Modifiers to Default")]
    private void ResetModifiersToDefault()
    {
        Modifiers = new StatusEffectModifiers
        {
            SpeedMultiplier = 1f,
            JumpMultiplier = 1f,
            Friction = 1f,
            AccelerationMultiplier = 1f
        };
    }
#endif
}
