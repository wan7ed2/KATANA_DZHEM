using System;
using UnityEngine;

public class IntroCinmaticController : MonoBehaviour
{
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _evilSprite;

    [SerializeField] private SpriteRenderer _goodRenderer;
    [SerializeField] private SpriteRenderer _evilenderer;
    
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _game;
    [SerializeField] private Animator _introAnimator;
    
    private void Start()
    {
        var isEvilMyself = PlayerPrefs.GetInt(Player.IS_EVIL_KEY, 0) != 0;
        _player.SetEvil(isEvilMyself);

        _goodRenderer.sprite = isEvilMyself ? _evilSprite : _goodSprite;
        _evilenderer.sprite = isEvilMyself ? _goodSprite : _evilSprite;
        
        _introAnimator.Play("Intro");
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        _game.SetActive(true);
    }
}