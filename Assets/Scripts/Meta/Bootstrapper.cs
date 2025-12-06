using System.Collections;
using UnityEngine;

public class Bootstrapper : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] private string mainLevelName;
    [SerializeField] private Curtain curtainPrefab;

    [Header("Sound System")]
    [SerializeField] private SoundSource soundSourcePrefab;

    private LevelLoadSystem _levelLoad;
    private Transform _soundPoolContainer;

    private Coroutine _levelCoroutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeSoundSystem();

        Systems.Add(new LevelLoadSystem(curtainPrefab, mainLevelName, this));
        Systems.Add(new PauseSystem());
    }

    private void InitializeSoundSystem()
    {
        _soundPoolContainer = new GameObject("SoundPool").transform;
        _soundPoolContainer.SetParent(transform);

        Systems.Add(new SoundSystem(soundSourcePrefab, _soundPoolContainer));
    }

    private void Start()
    {
        Systems.Get<LevelLoadSystem>().LoadMainLevel();
    }
}
