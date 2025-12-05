using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SoundSystem : ISystem
{
    private readonly List<VolumeSetting> _volumeSettings;

    public SoundSystem()
    {
        VolumeSetting[] volumeSettings = Resources.LoadAll<VolumeSetting>("Settings/SoundVolumes");
        _volumeSettings = new List<VolumeSetting>(volumeSettings);
    }

    public void SetMasterVolume(float value)
    {
        foreach (var volumeSetting in _volumeSettings)
            volumeSetting.Value = value;
    }

    public void SetVolume(VolumeType type, float value)
    {
        foreach (var volumeSetting in _volumeSettings)
            if (volumeSetting.Type == type)
                volumeSetting.Value = value;
    }

}
