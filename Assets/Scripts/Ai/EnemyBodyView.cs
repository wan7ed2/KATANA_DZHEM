using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBodyView : MonoBehaviour
{
    [SerializeField] private PatrolEnemy _patrolEnemy;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Vector3 _ccheScl;

    private void Awake()
    {
        _ccheScl = transform.localScale;
        _patrolEnemy.OnPointArrived += PatrolEnemyOnOnPointArrived;
    }

    private void OnDestroy()
    {
        _patrolEnemy.OnPointArrived -= PatrolEnemyOnOnPointArrived;
    }

    private void PatrolEnemyOnOnPointArrived()
    {
        if (!_patrolEnemy.IsGoingLeft)
        {
            transform.localScale = new Vector3(-_ccheScl.x, _ccheScl.y, _ccheScl.z);
        } else
        {
            transform.localScale = _ccheScl;
        }
        
    }
}