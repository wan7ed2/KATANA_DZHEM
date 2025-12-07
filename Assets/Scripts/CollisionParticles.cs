using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionParticles : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _groundCollisionParticles;
    [SerializeField] private GameObject _particlesXInverse;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _cd = 1f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _velocityMin = 1f;
    [SerializeField] private Color _ledColorOverride;

    private void Awake()
    {
        prevSpawnTimeStamp = Time.timeSinceLevelLoad;
    }
    private void Update()
    {
        if (Time.timeSinceLevelLoad - prevSpawnTimeStamp < _cd)
        {
            return;
        }

        var collision = Physics2D.Raycast(gameObject.transform.position, Vector3.down, _radius, _layerMask);
        if (collision.collider == null || collision.collider.gameObject == null)
            return;

        if (Mathf.Abs(_rb.velocity.x) < _velocityMin)
            return;

        prevSpawnTimeStamp = prevSpawnTimeStamp = Time.timeSinceLevelLoad;
        GameObject obj;
        if (_rb.velocity.x > 0)
            obj = Instantiate(_particlesXInverse, collision.point, Quaternion.identity);
        else
            obj = Instantiate(_groundCollisionParticles, collision.point, Quaternion.identity);

        if (collision.collider.gameObject.tag == "Ice")
        {
            var main = obj.GetComponent<ParticleSystem>().main;
            main.startColor = _ledColorOverride;
        }
    }


    float prevSpawnTimeStamp;

}
