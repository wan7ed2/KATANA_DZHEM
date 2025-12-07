using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadSystem : ISystem
{
    private readonly Curtain _curtain;
    private readonly string _mainLevelName;
    private readonly ICoroutineRunner _coroutineRunner;

    public LevelLoadSystem(Curtain curtainPrefab, string mainLevelName, ICoroutineRunner coroutineRunner)
    {
        _curtain = Object.Instantiate(curtainPrefab);
        _coroutineRunner = coroutineRunner;
        _mainLevelName = mainLevelName;

        _curtain.Hide();
        Object.DontDestroyOnLoad(_curtain.gameObject);
    }

    public Coroutine LoadMainLevel() =>
        _coroutineRunner.StartCoroutine(LoadLevelRoutine(_mainLevelName));

    private IEnumerator LoadLevelRoutine(string levelName)
    {
        _curtain.Show();
        Debug.Log($"Loading level: {levelName}");
        var loadOperation = SceneManager.LoadSceneAsync(levelName);

        while (!loadOperation.isDone)
            yield return null;

        _curtain.Hide();
        var pauseSystem = Systems.Get<PauseSystem>();
        if (pauseSystem != null && pauseSystem.Paused)
            pauseSystem.Resume();
    }
}
