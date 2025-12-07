using System;
using System.Collections.Generic;
using UnityEngine;

public class PalkaFixer : MonoBehaviour
{
    
    [SerializeField] private List<Rigidbody2D> _chains;
    [SerializeField] private Player _player;
    [SerializeField] private float _timeDisable = 0.1f;
    [SerializeField] private float _massDisable = 0.1f;

    [SerializeField] private Transform _cutoffPoint;
    [SerializeField] private Transform _chainPoint;

    private List<float> _chainsMass = new();
    
    private void Start()
    {
        foreach (var chain in _chains)
        {
            _chainsMass.Add(chain.mass);
        }
        
        _player.MovementController.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        _player.MovementController.OnJump -= HandleJump;
    }

    private void HandleJump()
    {
        if (!_player.GroundChecker.IsGrounded)
            return;

        if (_cutoffPoint.position.y > _chainPoint.position.y)
            return;
        
        DisableJoints();
        Invoke(nameof(EnableJoints), _timeDisable);
    }

    private void EnableJoints()
    {
        var idx = 0;
        foreach (var chain in _chains)
        {
            chain.mass = _chainsMass[idx];
            idx += 1;
        }
    }
    
    private void DisableJoints()
    {
        foreach (var chain in _chains)
        {
            chain.mass = _massDisable;
        }
    }
}