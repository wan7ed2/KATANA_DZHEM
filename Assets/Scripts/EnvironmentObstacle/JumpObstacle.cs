using UnityEngine;

public class JumpObstacle : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float force;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var pushable = collision.gameObject.GetComponentInParent<IPushableByObstacle>();
        if (pushable == null)
            return;

        pushable.Push(direction, force);
    }
}
