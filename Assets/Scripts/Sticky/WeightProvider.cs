using UnityEngine;

/// <summary>
/// Simple component that provides weight value.
/// </summary>
public class WeightProvider : MonoBehaviour, IWeightProvider
{
    [SerializeField] private float _weight = 1f;
    
    public float Weight => _weight;
    
    /// <summary>
    /// Set the weight value at runtime.
    /// </summary>
    public void SetWeight(float weight)
    {
        _weight = weight;
    }
}
