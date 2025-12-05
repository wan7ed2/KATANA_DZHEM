using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadSystem : ISystem
{
    private readonly Curtain _curtain;
    private readonly string _mainSceneName;
    private readonly ICoroutineRunner _coroutineRunner;

    public LevelLoadSystem(Curtain curtainPrefab, string mainSceneName, ICoroutineRunner coroutineRunner)
    {
        _curtain = GameObject.Instantiate(curtainPrefab);
        _coroutineRunner = coroutineRunner;

        _curtain.Hide();
        GameObject.DontDestroyOnLoad(_curtain.gameObject);
    }

    public Coroutine LoadMainLevel() =>
        _coroutineRunner.StartCoroutine(LoadLevelRoutine(_mainSceneName));

    private IEnumerator LoadLevelRoutine(string levelName)
    {
        _curtain.Show();
        var loadOperation = SceneManager.LoadSceneAsync(levelName);

        while (!loadOperation.isDone)
            yield return null;

        _curtain.Hide();
    }
}
