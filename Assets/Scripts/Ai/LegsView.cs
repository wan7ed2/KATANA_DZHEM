using System;
using UnityEngine;

public class LegsView : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;
    [SerializeField] private Transform _leftLeg;
    [SerializeField] private Transform _rightLeg;

    [SerializeField] private AnimationCurve _curve;

    private float _leftStartPosY;
    private float _rightStartPosY;

    private float _value;
    private bool _dir;

    private const float LEFT_BORDER = 0f;
    private const float RIGHT_BORDER = 10f;

    private void Awake()
    {
        _leftStartPosY = _leftLeg.localPosition.y;
        _rightStartPosY = _rightLeg.localPosition.y;
    }

    private void LateUpdate()
    {
        var addValue = _dir ? Time.deltaTime : -Time.deltaTime;
        _value += addValue * _speed;

        _dir = _value switch
        {
            > RIGHT_BORDER => false,
            < LEFT_BORDER => true,
            _ => _dir
        };

        var valueCurve = Mathf.Clamp(_value, LEFT_BORDER, RIGHT_BORDER);
        var evaluate = _curve.Evaluate(Mathf.InverseLerp(LEFT_BORDER, RIGHT_BORDER, valueCurve)) * _distance;

        var leftLegY = _leftStartPosY + evaluate;
        _leftLeg.localPosition = new Vector3(_leftLeg.localPosition.x, leftLegY, 0f);
        
        var rightLegY = _rightStartPosY - evaluate;
        _rightLeg.localPosition = new Vector3(_rightLeg.localPosition.x, rightLegY, 0f);
    }
}