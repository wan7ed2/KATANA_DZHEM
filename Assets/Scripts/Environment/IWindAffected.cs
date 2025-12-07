using UnityEngine;

/// <summary>
/// Интерфейс для объектов, на которые влияет ветер.
/// Реализуйте этот интерфейс, чтобы объект реагировал на WindSystem.
/// </summary>
public interface IWindAffected
{
    /// <summary>
    /// Применить силу ветра к объекту.
    /// </summary>
    /// <param name="force">Направление и сила ветра (direction * strength)</param>
    void ApplyWind(Vector2 force);
}


