using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button restartButton;

    private void Awake() => 
        Systems.Get<PauseSystem>().Resume();

    private void OnEnable()
    {
        Systems.Get<PauseSystem>().Pause();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        restartButton.onClick.RemoveListener(OnRestartClicked);
        Systems.Get<PauseSystem>().Resume();
    }

    private void OnVolumeChanged(float value)
    {
        Systems.Get<SoundSystem>().SetMasterVolume(value);
    }

    private void OnRestartClicked()
    {
        Systems.Get<LevelLoadSystem>().LoadMainLevel();
    }
}
