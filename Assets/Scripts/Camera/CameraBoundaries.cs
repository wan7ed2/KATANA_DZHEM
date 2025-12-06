using UnityEngine;

[System.Serializable]
public struct CameraBoundaries
{
    [field: SerializeField] public float Height { get; private set; }
    [field: SerializeField] public float LeftBound { get; private set; }
    [field: SerializeField] public float RightBound { get; private set; }
}
