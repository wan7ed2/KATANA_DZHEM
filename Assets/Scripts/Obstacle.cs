using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] private bool onlyHorizontalPush = true;
    [SerializeField] private float pushForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IPushableByObstacle>(out IPushableByObstacle pushable))
            return;

        Vector2 pushDirection = (collision.transform.position - transform.position);
        if (onlyHorizontalPush)
            pushDirection.y = 0;

        pushable.Push(pushDirection.normalized, pushForce);
    }
}