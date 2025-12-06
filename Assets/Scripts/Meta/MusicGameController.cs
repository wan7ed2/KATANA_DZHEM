using System;
using UnityEngine;

public class MusicGameController : MonoBehaviour
{
    [SerializeField] private SoundClip _soundClip;

    private void Start()
    {
        Systems.TryGet<SoundSystem>(out var soundSystem);
        soundSystem?.Play(_soundClip);
    }

    private void OnDestroy()
    {
        Systems.TryGet<SoundSystem>(out var soundSystem);
        soundSystem?.StopAll();
    }
}