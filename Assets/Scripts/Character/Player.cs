using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MovementsSettings _movementsSettings;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GroundChecker _groundChecker;

    public Rigidbody2D Rigidbody => _rigidbody; 
    
    private MovementController _movementController;
    
    private void Awake()
    {
        _groundChecker.Init(_rigidbody);
        _movementController = new MovementController(_rigidbody, _movementsSettings, _groundChecker);
    }

    private void Start()
    {
        _movementController.Start();
    }

    private void FixedUpdate()
    {
        _groundChecker.Update();
        _movementController.Update();
    }

    private void OnDestroy()
    {
        _movementController.Stop();
    }
}
