using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuzhaOfPurityController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IPurifiable purifiable;
        if (!collision.gameObject.TryGetComponent<IPurifiable>(out purifiable))
            return;

        if (purifiable.IsPureAlready())
            return;

        purifiable.Purify();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IPurifiable>(out IPurifiable purifiable))
            return;

        purifiable.Purify();
    }
}
