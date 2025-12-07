using UnityEngine;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void EndGame()
    {
        _animator.Play("EndGame");
        var newEvilValue = PlayerPrefs.GetInt(Player.IS_EVIL_KEY, 0) == 0 ? 1 : 0;
        PlayerPrefs.SetInt(Player.IS_EVIL_KEY, newEvilValue);
    }

    public void RestartLevel()
    {
        Systems.TryGet<LevelLoadSystem>(out var levelLoadSystem);
        levelLoadSystem?.LoadMainLevel();
    }
}