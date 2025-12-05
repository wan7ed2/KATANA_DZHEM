using System.Collections;
using UnityEngine;

public class Bootstrapper : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] private string mainLevelName;
    [SerializeField] private Curtain curtainPrefab;

    private LevelLoadSystem _levelLoad;

    private Coroutine _levelCoroutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Systems.Add(new LevelLoadSystem(curtainPrefab, mainLevelName, this));
        Systems.Add(new PauseSystem());
    }

    private void Start()
    {
        Systems.Get<LevelLoadSystem>().LoadMainLevel();
    }
}
