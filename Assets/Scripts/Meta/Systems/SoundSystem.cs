using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : ISystem
{
    private const int InitialPoolSize = 5;
    private const int MaxPoolSize = 20;

    private readonly List<VolumeSetting> _volumeSettings;
    private readonly Dictionary<VolumeType, VolumeSetting> _volumeSettingsByType;
    private readonly List<SoundSource> _pool;
    private readonly List<SoundSource> _activeSources;
    private readonly Transform _poolContainer;
    private readonly SoundSource _soundSourcePrefab;

    public SoundSystem(SoundSource soundSourcePrefab, Transform poolContainer)
    {
        if (soundSourcePrefab == null)
            throw new System.ArgumentNullException(nameof(soundSourcePrefab), 
                "SoundSystem requires a SoundSource prefab. Please assign it in the Bootstrapper.");

        _soundSourcePrefab = soundSourcePrefab;
        _poolContainer = poolContainer;
        _pool = new List<SoundSource>();
        _activeSources = new List<SoundSource>();
        _volumeSettingsByType = new Dictionary<VolumeType, VolumeSetting>();

        VolumeSetting[] volumeSettings = Resources.LoadAll<VolumeSetting>("Settings/SoundVolumes");
        _volumeSettings = new List<VolumeSetting>(volumeSettings);

        foreach (var setting in _volumeSettings)
            _volumeSettingsByType[setting.Type] = setting;

        // Pre-warm the pool
        for (int i = 0; i < InitialPoolSize; i++)
            CreatePooledSource();
    }

    #region Volume Control

    public void SetVolume(VolumeType type, float value)
    {
        foreach (var volumeSetting in _volumeSettings)
            if (volumeSetting.Type == type)
                volumeSetting.Value = value;
    }

    public float GetVolume(VolumeType type)
    {
        return _volumeSettingsByType.TryGetValue(type, out var setting) ? setting.Value : 1f;
    }

    #endregion

    #region Sound Playback

    /// <summary>
    /// Plays a SoundClip (ScriptableObject) with all its settings
    /// </summary>
    public SoundSource Play(SoundClip soundClip)
    {
        if (soundClip == null || !soundClip.HasClips)
        {
            Debug.LogWarning("SoundSystem: Attempted to play null or empty SoundClip");
            return null;
        }

        return PlayInternal(
            soundClip.Clip,
            soundClip.VolumeType,
            soundClip.Volume,
            soundClip.Pitch,
            soundClip.Loop
        );
    }

    /// <summary>
    /// Plays an AudioClip with specified parameters
    /// </summary>
    public SoundSource Play(AudioClip clip, VolumeType volumeType = VolumeType.SFX,
        float volume = 1f, float pitch = 1f, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundSystem: Attempted to play null AudioClip");
            return null;
        }

        return PlayInternal(clip, volumeType, volume, pitch, loop);
    }

    /// <summary>
    /// Plays a one-shot sound (fire and forget)
    /// </summary>
    public void PlayOneShot(AudioClip clip, VolumeType volumeType = VolumeType.SFX, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundSystem: Attempted to play null AudioClip");
            return;
        }

        var source = GetOrCreateSource(volumeType);
        source.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Plays a one-shot sound from SoundClip
    /// </summary>
    public void PlayOneShot(SoundClip soundClip)
    {
        if (soundClip == null || !soundClip.HasClips)
        {
            Debug.LogWarning("SoundSystem: Attempted to play null or empty SoundClip");
            return;
        }

        var source = GetOrCreateSource(soundClip.VolumeType);
        source.PlayOneShot(soundClip.Clip, soundClip.Volume, soundClip.Pitch);
    }

    /// <summary>
    /// Stops all currently playing sounds
    /// </summary>
    public void StopAll()
    {
        for (int i = _activeSources.Count - 1; i >= 0; i--)
        {
            var source = _activeSources[i];
            source.Stop();
        }
    }

    /// <summary>
    /// Stops all sounds of a specific volume type
    /// </summary>
    public void StopAll(VolumeType volumeType)
    {
        for (int i = _activeSources.Count - 1; i >= 0; i--)
        {
            var source = _activeSources[i];
            if (source.VolumeType == volumeType)
                source.Stop();
        }
    }

    #endregion

    #region Pool Management

    private SoundSource PlayInternal(AudioClip clip, VolumeType volumeType, float volume, float pitch, bool loop)
    {
        var source = GetOrCreateSource(volumeType);
        source.Play(clip, volume, loop, pitch);
        return source;
    }

    private SoundSource GetOrCreateSource(VolumeType volumeType)
    {
        SoundSource source;

        // Try to get from pool
        if (_pool.Count > 0)
        {
            source = _pool[_pool.Count - 1];
            _pool.RemoveAt(_pool.Count - 1);
        }
        else if (_activeSources.Count < MaxPoolSize)
        {
            source = CreateNewSource();
        }
        else
        {
            // Reuse oldest active source
            source = _activeSources[0];
            source.Finished -= OnSourceFinished;
            _activeSources.RemoveAt(0);
            source.ResetSource();
        }

        // Initialize with correct volume setting
        _volumeSettingsByType.TryGetValue(volumeType, out var volumeSetting);
        source.Initialize(volumeSetting, volumeType);

        source.gameObject.SetActive(true);
        _activeSources.Add(source);
        source.Finished += OnSourceFinished;

        return source;
    }

    private SoundSource CreatePooledSource()
    {
        var source = CreateNewSource();
        _pool.Add(source);
        return source;
    }

    private SoundSource CreateNewSource()
    {
        var source = Object.Instantiate(_soundSourcePrefab, _poolContainer);
        source.gameObject.SetActive(false);
        return source;
    }

    private void OnSourceFinished(SoundSource source)
    {
        source.Finished -= OnSourceFinished;
        _activeSources.Remove(source);
        source.ResetSource();
        source.gameObject.SetActive(false);

        if (_pool.Count < MaxPoolSize)
            _pool.Add(source);
        else
            Object.Destroy(source.gameObject);
    }

    #endregion
}
