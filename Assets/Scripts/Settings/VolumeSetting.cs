using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VolumeSetting", menuName = "Settings/VolumeSetting")]
public class VolumeSetting : ScriptableObject
{
    [field: SerializeField] public VolumeType Type { get; private set; }

    [SerializeField, Range(0f, 1f)] private float currentValue;

    public event Action Changed;

    public float Value
    {
        get => currentValue;
        set
        {
            if (Math.Abs(currentValue - value) > Mathf.Epsilon)
            {
                currentValue = Mathf.Clamp01(value);
                Changed?.Invoke();
            }
        }
    }
}
