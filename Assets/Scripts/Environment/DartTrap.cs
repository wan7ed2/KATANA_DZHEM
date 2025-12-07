using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartTrap : MonoBehaviour
{
    [SerializeField] GameObject dartPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] private int count;

    private bool triggered = false;
    private int _firedCount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_firedCount >= count)
            return;

        var pushable = collision.gameObject.GetComponent<IPushableByObstacle>();
        if (pushable == null)
            return;

        FireDart();
    }

    void FireDart()
    {
        GameObject dart = Instantiate(dartPrefab, shootPoint.position, shootPoint.rotation);
        _firedCount++;
    }
}
