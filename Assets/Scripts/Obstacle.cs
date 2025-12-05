using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] private bool onlyHorizontalPush = true;
    [SerializeField] private float pushForce = 5f;

    private void OnValidate()
    {
        var collider = GetComponent<Collider2D>();
        if (collider.isTrigger)
            Debug.LogWarning($"Collider on {gameObject.name} is set as Trigger. Obstacle requires a non-trigger Collider2D to function properly.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            return;

        Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
        if (onlyHorizontalPush)
            pushDirection.y = 0;

        player.ApplyPush(pushDirection.normalized * pushForce);
    }
}
