using UnityEngine;
using UnityEngine.EventSystems;

public class Bootstrapper : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] private string mainLevelName;
    [SerializeField] private Curtain curtainPrefab;

    [Header("Sound System")]
    [SerializeField] private SoundSource soundSourcePrefab;

    [Header("Pause menu")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private EventSystem eventSystem;

    private Transform _soundPoolContainer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(eventSystem.gameObject);

        InitializeSoundSystem();

        Systems.Add(new PauseSystem(pauseMenu));
        Systems.Add(new LevelLoadSystem(curtainPrefab, mainLevelName, this));
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
