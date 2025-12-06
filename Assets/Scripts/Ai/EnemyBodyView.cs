using System;
using UnityEngine;

public class EnemyBodyView : MonoBehaviour
{
    [SerializeField] private PatrolEnemy _patrolEnemy;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _patrolEnemy.OnPointArrived += PatrolEnemyOnOnPointArrived;
    }

    private void OnDestroy()
    {
        _patrolEnemy.OnPointArrived -= PatrolEnemyOnOnPointArrived;
    }

    private void PatrolEnemyOnOnPointArrived()
    {
        _spriteRenderer.flipX = !_patrolEnemy.IsGoingLeft;
    }
}