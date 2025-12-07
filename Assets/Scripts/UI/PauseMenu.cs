using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        UpdateSliders();
    }

    private void OnEnable()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void OnMusicChanged(float value)
    {
        Systems.Get<SoundSystem>().SetVolume(VolumeType.Music, value);
    }

    private void OnSfxChanged(float value)
    {
        Systems.Get<SoundSystem>().SetVolume(VolumeType.SFX, value);
    }

    private void OnRestartClicked()
    {
        Systems.Get<LevelLoadSystem>().LoadMainLevel();
    }

    private void UpdateSliders()
    {
        var soundSystem = Systems.Get<SoundSystem>();
        musicSlider.value = soundSystem.GetVolume(VolumeType.Music);
        sfxSlider.value = soundSystem.GetVolume(VolumeType.SFX);
    }
}
