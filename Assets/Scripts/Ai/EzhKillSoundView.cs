using System;
using UnityEngine;

public class EzhKillSoundView : MonoBehaviour
{
    [SerializeField] private SoundClip _soundClip;
    [SerializeField] private StickableObject _stickable;

    private void Start()
    {
        _stickable.OnStickAction += Play;
    }

    private void OnDisable()
    {
        _stickable.OnStickAction -= Play;
    }

    private void Play()
    {
        Systems.TryGet<SoundSystem>(out var soundSystem);
        soundSystem?.PlayOneShot(_soundClip);
    }
}