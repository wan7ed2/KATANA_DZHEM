using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 7f;
    
    private void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }
}
