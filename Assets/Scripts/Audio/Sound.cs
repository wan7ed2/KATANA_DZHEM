using UnityEngine;

/// <summary>
/// Static helper class for easy global access to sound playback.
/// All methods delegate to the SoundSystem registered in Systems.
/// </summary>
public static class Sound
{
    private static SoundSystem _soundSystem;

    private static SoundSystem SoundSystem
    {
        get
        {
            // Re-fetch if null or if the system was replaced (e.g., after scene reload)
            if (_soundSystem == null)
                _soundSystem = Systems.Get<SoundSystem>();
            return _soundSystem;
        }
    }

    /// <summary>
    /// Resets the cached system reference. Call this when reloading the game.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        _soundSystem = null;
    }

    /// <summary>
    /// Plays a SoundClip with all its configured settings.
    /// Returns the SoundSource for additional control (e.g., stopping).
    /// </summary>
    public static SoundSource Play(SoundClip clip) => SoundSystem.Play(clip);

    /// <summary>
    /// Plays an AudioClip with specified parameters.
    /// Returns the SoundSource for additional control.
    /// </summary>
    public static SoundSource Play(AudioClip clip, VolumeType volumeType = VolumeType.SFX,
        float volume = 1f, float pitch = 1f, bool loop = false)
        => SoundSystem.Play(clip, volumeType, volume, pitch, loop);

    /// <summary>
    /// Plays a one-shot sound (fire and forget).
    /// </summary>
    public static void PlayOneShot(AudioClip clip, VolumeType volumeType = VolumeType.SFX, float volume = 1f)
        => SoundSystem.PlayOneShot(clip, volumeType, volume);

    /// <summary>
    /// Plays a one-shot sound from SoundClip (fire and forget).
    /// </summary>
    public static void PlayOneShot(SoundClip clip) => SoundSystem.PlayOneShot(clip);

    /// <summary>
    /// Stops all currently playing sounds.
    /// </summary>
    public static void StopAll() => SoundSystem.StopAll();

    /// <summary>
    /// Stops all sounds of a specific volume type.
    /// </summary>
    public static void StopAll(VolumeType volumeType) => SoundSystem.StopAll(volumeType);

    /// <summary>
    /// Sets the master volume for all sound types.
    /// </summary>
    public static void SetMasterVolume(float value) => SoundSystem.SetMasterVolume(value);

    /// <summary>
    /// Sets the volume for a specific sound type (Music, SFX).
    /// </summary>
    public static void SetVolume(VolumeType type, float value) => SoundSystem.SetVolume(type, value);

    /// <summary>
    /// Gets the current volume for a specific sound type.
    /// </summary>
    public static float GetVolume(VolumeType type) => SoundSystem.GetVolume(type);
}
