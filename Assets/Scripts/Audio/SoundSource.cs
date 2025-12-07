using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;
    private VolumeSetting _volumeSetting;
    private float _baseVolume = 1f;

    public bool IsPlaying => _audioSource != null && _audioSource.isPlaying;
    public bool IsLooping => _audioSource != null && _audioSource.loop;
    public VolumeType VolumeType { get; private set; }

    public event Action<SoundSource> Finished;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (!_audioSource.loop && !_audioSource.isPlaying)
        {
            Finished?.Invoke(this);
            enabled = false;
        }
    }

    public void Initialize(VolumeSetting volumeSetting, VolumeType volumeType)
    {
        // Unsubscribe from previous setting to avoid memory leak
        if (_volumeSetting != null)
            _volumeSetting.Changed -= UpdateVolume;

        _volumeSetting = volumeSetting;
        VolumeType = volumeType;

        if (_volumeSetting != null)
        {
            _volumeSetting.Changed += UpdateVolume;
            UpdateVolume();
        }
    }

    public void Play(AudioClip clip, float volume = 1f, bool loop = false, float pitch = 1f)
    {
        _baseVolume = volume;
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.pitch = pitch;
        UpdateVolume();
        _audioSource.Play();
        enabled = true;
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        _baseVolume = volume;
        _audioSource.pitch = pitch;
        UpdateVolume();
        _audioSource.PlayOneShot(clip);
        enabled = true;
    }

    public void Stop()
    {
        if (_audioSource != null) _audioSource.Stop();
        Finished?.Invoke(this);
        enabled = false;
    }

    public void SetVolume(float volume)
    {
        _baseVolume = volume;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        float settingVolume = _volumeSetting != null ? _volumeSetting.Value : 1f;
        _audioSource.volume = _baseVolume * settingVolume;
    }

    private void OnDisable()
    {
        if (_volumeSetting != null)
            _volumeSetting.Changed -= UpdateVolume;
    }

    public void ResetSource()
    {
        if (_volumeSetting != null)
            _volumeSetting.Changed -= UpdateVolume;

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.loop = false;
            _audioSource.pitch = 1f;
        }

        _baseVolume = 1f;
        _volumeSetting = null;
        enabled = false;
    }
}
