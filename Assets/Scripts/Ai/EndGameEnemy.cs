using System;
using UnityEngine;

public class EndGameEnemy : MonoBehaviour, IKillable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private EndGameController _endGameController;

    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _evilSprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        var isEvilPlayer = PlayerPrefs.GetInt(Player.IS_EVIL_KEY, 0) != 0;
        _spriteRenderer.sprite = isEvilPlayer ? _goodSprite : _evilSprite;
    }

    public void Kill()
    {
        _collider.enabled = false;
        _rigidbody.isKinematic = false;
        _endGameController.EndGame();
    }
}

public interface IKillable
{
    public void Kill();
}