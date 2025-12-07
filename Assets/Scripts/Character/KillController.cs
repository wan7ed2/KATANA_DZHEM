using System;
using UnityEngine;

public class KillController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent<IKillable>(out var killable))
            return;
        
        killable.Kill();
    }
}