using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 7f;

    public void DisableSelfDestruction()
    {
        CancelDestroy();
    }
    
    private void Awake()
    {
        StartDestroyCountdown(_lifeTime);
    }

    private Coroutine destroyRoutine;

    public void StartDestroyCountdown(float delay)
    {
        if (destroyRoutine != null)
            StopCoroutine(destroyRoutine);

        destroyRoutine = StartCoroutine(DestroyAfter(delay));
    }

    public void CancelDestroy()
    {
        if (destroyRoutine != null)
        {
            StopCoroutine(destroyRoutine);
            destroyRoutine = null;
        }
    }

    private IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
