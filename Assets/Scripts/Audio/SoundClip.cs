using UnityEngine;

[CreateAssetMenu(fileName = "SoundClip", menuName = "Audio/SoundClip")]
public class SoundClip : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0.1f, 3f)] private float pitch = 1f;
    [SerializeField, Range(0f, 0.5f)] private float pitchVariation = 0f;
    [SerializeField] private VolumeType volumeType = VolumeType.SFX;
    [SerializeField] private bool loop = false;

    public AudioClip Clip => HasClips ? clips[Random.Range(0, clips.Length)] : null;
    public float Volume => volume;
    public float Pitch => pitch + Random.Range(-pitchVariation, pitchVariation);
    public VolumeType VolumeType => volumeType;
    public bool Loop => loop;

    public bool HasClips => clips != null && clips.Length > 0;
}
