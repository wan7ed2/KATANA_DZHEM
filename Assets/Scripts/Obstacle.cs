using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private bool onlyHorizontalPush = true;
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private bool shouldDestroyAfterCollision = false;

    Collider2D _collider;

    public void DisableDestroyingAfterCollision()
    {
        shouldDestroyAfterCollision = false;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var pushable = collision.gameObject.GetComponent<IPushableByObstacle>();
        if (pushable == null)
            return;

        Vector2 pushDirection = (collision.transform.position - transform.position);
        if (onlyHorizontalPush)
            pushDirection.y = 0;

        pushable.Push(pushDirection.normalized, pushForce);

        if (shouldDestroyAfterCollision && pushable != null)
        {
            _collider.enabled = false;
            Destroy(this);
        }
    }

}