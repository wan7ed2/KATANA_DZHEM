using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartLogic : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool isUp = true; // true - up, false - right

    private void Update()
    {
        if (isUp)
            transform.position += transform.up * _speed * Time.deltaTime;
        else
            transform.position += transform.right * _speed * Time.deltaTime;

    }
}
