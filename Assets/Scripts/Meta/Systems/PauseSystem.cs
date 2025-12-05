using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : ISystem
{
    private List<IPauseHandler> _handlers = new List<IPauseHandler>() { };

    public void AddHandler(IPauseHandler handler) => 
        _handlers.Add(handler);

    public void RemoveHandler(IPauseHandler handler) => 
        _handlers.Remove(handler);

    public void Pause()
    {
        Time.timeScale = 0f;
        foreach (var handler in _handlers)
            handler.OnPause();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        foreach (var handler in _handlers)
            handler.OnResume();
    }
}
