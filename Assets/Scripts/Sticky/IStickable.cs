using UnityEngine;

/// <summary>
/// Interface for objects that can stick to a StickyController.
/// Weight is handled separately via IWeightProvider.
/// </summary>
public interface IStickable
{
    /// <summary>
    /// The Rigidbody2D of this stickable object.
    /// </summary>
    Rigidbody2D Rigidbody { get; }
    
    /// <summary>
    /// The GameObject of this stickable object.
    /// </summary>
    GameObject GameObject { get; }
    
    /// <summary>
    /// Whether this object is currently stuck to something.
    /// </summary>
    bool IsStuck { get; }
    
    /// <summary>
    /// Called when the object sticks to a target.
    /// </summary>
    /// <param name="parent">The transform to parent to</param>
    /// <param name="stickPoint">The world position where the object should stick</param>
    void OnStick(Transform parent, Vector2 stickPoint);
    
    /// <summary>
    /// Called when the object is released from the target.
    /// </summary>
    void OnRelease();
}

