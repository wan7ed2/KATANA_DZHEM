using Character;
using UnityEngine;

/// <summary>
/// Interface for objects that can stick to a StickyController.
/// </summary>
public interface IStickable
{
    Rigidbody2D Rigidbody { get; }
    GameObject GameObject { get; }
    bool IsStuck { get; }
    bool CanStick { get; }
    
    void OnStick(StickyController source, Transform parent, Vector2 stickPoint);
    void OnRelease();
}
