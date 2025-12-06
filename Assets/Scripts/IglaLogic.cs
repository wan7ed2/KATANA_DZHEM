using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IglaLogic : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _lifeTime = 7f;

    private void Awake()
    {
        Invoke("Terminate", _lifeTime);
    }

    private void Update()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void Terminate()
    {
        Destroy(gameObject);
    }
}
