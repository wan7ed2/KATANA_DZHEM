using System;
using UnityEngine;

public class IntroCinmaticController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _game;
    [SerializeField] private Animator _introAnimator;
    
    private void Start()
    {
        _introAnimator.Play("Intro");
    }

    public void StartGame()
    {
        gameObject.SetActive(false);

        var isEvil = PlayerPrefs.GetInt("IsEvil", 0);
        _player.SetEvil(isEvil != 0);
        _game.SetActive(true);
    }
}