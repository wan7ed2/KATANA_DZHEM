using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VolumeUpdater : MonoBehaviour
{
    [SerializeField] private VolumeSetting volumeSetting;

    private AudioSource _audioSource;

    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void OnEnable()
    {
        volumeSetting.Changed += UpdateVolume;
        UpdateVolume();
    }

    private void OnDisable() => volumeSetting.Changed -= UpdateVolume;

    private void UpdateVolume() => _audioSource.volume = volumeSetting.Value;

}
