using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundClip))]
public class SoundClipEditor : Editor
{
    private AudioSource _previewSource;

    private void OnEnable()
    {
        // Create a hidden AudioSource for preview
        var previewObj = EditorUtility.CreateGameObjectWithHideFlags(
            "SoundClip Preview",
            HideFlags.HideAndDontSave,
            typeof(AudioSource)
        );
        _previewSource = previewObj.GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        if (_previewSource != null)
        {
            DestroyImmediate(_previewSource.gameObject);
            _previewSource = null;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        var soundClip = (SoundClip)target;

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = soundClip.HasClips;

        if (GUILayout.Button("▶ Play", GUILayout.Height(30)))
        {
            StopPreview();
            PlayPreview(soundClip);
        }

        GUI.enabled = _previewSource != null && _previewSource.isPlaying;

        if (GUILayout.Button("■ Stop", GUILayout.Height(30)))
        {
            StopPreview();
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        // Show playback status
        if (_previewSource != null && _previewSource.isPlaying)
        {
            EditorGUILayout.HelpBox($"Playing: {_previewSource.clip.name}", MessageType.Info);
            Repaint();
        }
    }

    private void PlayPreview(SoundClip soundClip)
    {
        if (!soundClip.HasClips) return;

        _previewSource.clip = soundClip.Clip;
        _previewSource.volume = soundClip.Volume;
        _previewSource.pitch = soundClip.Pitch;
        _previewSource.loop = soundClip.Loop;
        _previewSource.Play();
    }

    private void StopPreview()
    {
        if (_previewSource != null)
        {
            _previewSource.Stop();
        }
    }
}

