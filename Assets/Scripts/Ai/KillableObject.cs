using UnityEngine;

public class KillableObject : MonoBehaviour, IKillable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    
    public void Kill()
    {
        _collider.enabled = false;
        _rigidbody.isKinematic = false;
        Destroy(gameObject, 5f);
    }
}