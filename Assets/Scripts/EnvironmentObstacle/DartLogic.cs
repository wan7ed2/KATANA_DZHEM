using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartLogic : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool isUp = true; // true - up, false - right
    [SerializeField] private LayerMask effectiveWayToCheckGeound;
    [SerializeField] private float groundCheckDistance = 0.1f;

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
        Vector3 dir;
        if (isUp)
            dir = transform.up;
        else
            dir = transform.right;

        if (Physics2D.Raycast(gameObject.transform.position, dir, groundCheckDistance, effectiveWayToCheckGeound))
        {
            Destroy(gameObject, 0.1f);
        }

        if (_stickableObject.IsStuck)
        {
            _selfDestruct.DisableSelfDestruction();
            _obstacle.DisableDestroyingAfterCollision();

            Destroy(this);
            return;
        }

        transform.position += dir * _speed * Time.deltaTime;
    }
}
