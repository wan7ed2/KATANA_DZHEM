using UnityEngine;

/// <summary>
/// Debug UI для отображения активных статус-эффектов.
/// Показывает список эффектов с таймерами.
/// </summary>
public class StatusEffectDebugUI : MonoBehaviour
{
    [SerializeField] private BaseStatusEffectHandler _handler;
    [SerializeField] private Vector2 _position = new Vector2(10, 10);
    [SerializeField] private bool _showModifiers = true;
    
    private GUIStyle _boxStyle;
    private GUIStyle _headerStyle;
    private GUIStyle _effectStyle;
    private GUIStyle _modifierStyle;
    private bool _stylesInitialized;

    private void InitStyles()
    {
        if (_stylesInitialized) return;
        
        _boxStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(10, 10, 10, 10)
        };
        
        _headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14
        };
        
        _effectStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        };
        
        _modifierStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            normal = { textColor = Color.gray }
        };
        
        _stylesInitialized = true;
    }

    private void OnGUI()
    {
        if (_handler == null) return;
        
        InitStyles();
        
        GUILayout.BeginArea(new Rect(_position.x, _position.y, 250, 400));
        GUILayout.BeginVertical(_boxStyle);
        
        // Header
        GUILayout.Label("Status Effects", _headerStyle);
        
        if (!_handler.HasAnyEffect)
        {
            GUILayout.Label("No active effects", _effectStyle);
        }
        else
        {
            DrawActiveEffects();
        }
        
        if (_showModifiers)
        {
            GUILayout.Space(10);
            DrawModifiers();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    private void DrawActiveEffects()
    {
        // Используем reflection чтобы получить приватный словарь эффектов
        var field = typeof(BaseStatusEffectHandler).GetField(
            "_effects", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        
        if (field == null) return;
        
        var effects = field.GetValue(_handler) as System.Collections.Generic.Dictionary<StatusEffect, ActiveStatusEffect>;
        if (effects == null) return;
        
        foreach (var kvp in effects)
        {
            var effect = kvp.Key;
            var active = kvp.Value;
            
            // Цветная полоска прогресса
            var progress = active.Progress;
            var barColor = effect.GizmoColor;
            
            GUILayout.BeginHorizontal();
            
            // Название и таймер
            GUILayout.Label($"{effect.DisplayName}", _effectStyle, GUILayout.Width(100));
            
            // Progress bar
            var barRect = GUILayoutUtility.GetRect(100, 16);
            GUI.color = new Color(0.2f, 0.2f, 0.2f);
            GUI.DrawTexture(barRect, Texture2D.whiteTexture);
            
            var filledRect = new Rect(barRect.x, barRect.y, barRect.width * progress, barRect.height);
            GUI.color = barColor;
            GUI.DrawTexture(filledRect, Texture2D.whiteTexture);
            
            GUI.color = Color.white;
            
            // Время
            GUILayout.Label($"{active.RemainingTime:F1}s", _effectStyle, GUILayout.Width(40));
            
            GUILayout.EndHorizontal();
        }
    }
    
    private void DrawModifiers()
    {
        GUILayout.Label("Current Modifiers", _headerStyle);
        
        var mods = _handler.CurrentModifiers;
        
        DrawModifierLine("Speed", mods.SpeedMultiplier);
        DrawModifierLine("Jump", mods.JumpMultiplier);
        DrawModifierLine("Friction", mods.Friction);
        DrawModifierLine("Accel", mods.AccelerationMultiplier);
    }
    
    private void DrawModifierLine(string name, float value)
    {
        var color = value < 1f ? Color.red : (value > 1f ? Color.green : Color.gray);
        var originalColor = GUI.color;
        GUI.color = color;
        GUILayout.Label($"  {name}: {value:F2}x", _modifierStyle);
        GUI.color = originalColor;
    }
}


