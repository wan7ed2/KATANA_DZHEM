using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartLogic : MonoBehaviour
{

    [SerializeField] float _velocity = 0.03f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(_velocity, 0, 0);
    }
}
