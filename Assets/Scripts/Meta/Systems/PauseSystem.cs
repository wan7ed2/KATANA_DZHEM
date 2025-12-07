using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : ISystem
{
    private readonly List<IPauseHandler> _handlers = new() { };
    private readonly PauseMenu _pauseMenu;

    public PauseSystem(PauseMenu pauseMenu)
    {
        _pauseMenu = pauseMenu;
        Object.DontDestroyOnLoad(_pauseMenu.gameObject);
        _pauseMenu.gameObject.SetActive(false);
    }

    public bool Paused { get; private set; }

    public void AddHandler(IPauseHandler handler) => 
        _handlers.Add(handler);

    public void RemoveHandler(IPauseHandler handler) => 
        _handlers.Remove(handler);

    public void Pause()
    {
        Debug.Log("Pause enabled");
        Time.timeScale = 0f;
        _pauseMenu.gameObject.SetActive(true);
        Paused = true;
        foreach (var handler in _handlers)
            handler.OnPause();
    }

    public void Resume()
    {
        Debug.Log("Pause disabled");
        Time.timeScale = 1f;
        _pauseMenu.gameObject.SetActive(false);
        Paused = false;
        foreach (var handler in _handlers)
            handler.OnResume();
    }
}
