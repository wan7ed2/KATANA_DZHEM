/// <summary>
/// Interface for objects that provide weight.
/// Can be used independently of IStickable for more flexibility.
/// </summary>
public interface IWeightProvider
{
    /// <summary>
    /// The weight provided by this object.
    /// </summary>
    float Weight { get; }
}

