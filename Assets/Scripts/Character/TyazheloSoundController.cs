using System;
using UnityEngine;

public class TyazheloSoundController : MonoBehaviour
{
    public event Action OnTyazhelo; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<TyazheloCheckpoint>(out var tyazheloCheckpoint))
            return;
        
        if (tyazheloCheckpoint.ID < _lastCheckpoint)
            OnTyazhelo?.Invoke();

        _lastCheckpoint = tyazheloCheckpoint.ID;
    }

    private int _lastCheckpoint = 0;
}