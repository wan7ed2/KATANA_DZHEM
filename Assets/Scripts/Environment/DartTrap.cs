using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartTrap : MonoBehaviour
{
    [SerializeField] GameObject dartPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] bool shouldTriggerOnlyOnce = false;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        var pushable = collision.gameObject.GetComponent<IPushableByObstacle>();
        if (pushable == null)
            return;

        FireDart();

        triggered = shouldTriggerOnlyOnce;
    }

    void FireDart()
    {
        GameObject dart = Instantiate(dartPrefab, shootPoint.position, shootPoint.rotation);
    }
}
