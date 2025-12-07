using System;
using System.Collections.Generic;
using UnityEngine;

public class Systems
{
    private static readonly Dictionary<Type, ISystem> _systems = new();

    public static void Add<T>(T system) where T : ISystem
    {
        var type = typeof(T);
        if (_systems.ContainsKey(type))
            throw new InvalidOperationException($"System of type {type} is already registered.");

        _systems[type] = system;
        Debug.Log($"Added system: {type}");
    }

    public static T Get<T>() where T : ISystem
    {
        var type = typeof(T);
        if (_systems.TryGetValue(type, out var system))
            return (T)system;

        throw new KeyNotFoundException($"System of type {type} is not registered.");
    }

    public static bool TryGet<T>(out T outSystem) where T : ISystem
    {
        var type = typeof(T);
        outSystem = default;

        if (!_systems.ContainsKey(type))
            return false;

        outSystem = (T)_systems[type];
        return true;
    }
}
