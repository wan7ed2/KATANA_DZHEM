using UnityEngine;

public class RotationInterpolator : MonoBehaviour
{
    [SerializeField] private float _lerpV = 0.9f;
    
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _source;

    private void LateUpdate()
    {
        _source.rotation = Quaternion.Lerp(_source.rotation, _target.rotation, _lerpV);
    }
}