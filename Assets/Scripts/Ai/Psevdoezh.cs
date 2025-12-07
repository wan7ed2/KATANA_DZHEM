using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psevdoezh : MonoBehaviour
{
    [SerializeField] private GameObject _iglaPrefab;
    [SerializeField] private List<Transform> _shootTransforms;
    [SerializeField] private float _zalpsInBurst = 3f; 
    [SerializeField] private float _zalpCooldown = 1.5f;
    [SerializeField] private float _burstCooldown = 5f;
    [SerializeField] private StickableObject stickableObject;

    private float prevBurstTimeStamp;

    private void Awake()
    {
        prevBurstTimeStamp = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        if (stickableObject != null && stickableObject.IsStuck)
        {
            return;
        }

        if (Time.timeSinceLevelLoad - prevBurstTimeStamp > _burstCooldown)
        {
            for (var i = 0; i < _zalpsInBurst - 1; i++)
            {
                Invoke("ShootZalp", i * _zalpCooldown);
            }

            Invoke("ShootEndZalp", (_zalpsInBurst - 1) * _zalpCooldown);
            prevBurstTimeStamp = Time.timeSinceLevelLoad;
        }
    }

    private void ShootZalp()
    {
        foreach (Transform trs in _shootTransforms)
        {
            var igla = Instantiate(_iglaPrefab, trs.position, trs.rotation);
        }
    }

    private void ShootEndZalp()
    {
        ShootZalp();
    }
}
