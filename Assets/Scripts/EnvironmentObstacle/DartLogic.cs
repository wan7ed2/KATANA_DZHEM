using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartLogic : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool isUp = true; // true - up, false - right

    StickableObject _stickableObject;
    SelfDestruct _selfDestruct;
    Obstacle _obstacle;

    private void Awake()
    {
        _stickableObject = GetComponent<StickableObject>();
        _selfDestruct = GetComponent<SelfDestruct>();
        _obstacle = GetComponent<Obstacle>();
    }

    private void Update()
    {
        if (_stickableObject.IsStuck)
        {
            _selfDestruct.DisableSelfDestruction();
            _obstacle.DisableDestroyingAfterCollision();

            Destroy(this);
            return;
        }

        if (isUp)
            transform.position += transform.up * _speed * Time.deltaTime;
        else
            transform.position += transform.right * _speed * Time.deltaTime;

    }
}
