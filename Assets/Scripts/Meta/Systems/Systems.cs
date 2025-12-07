using System;
using System.Collections.Generic;

public class Systems
{
    private static readonly Dictionary<Type, ISystem> _systems = new Dictionary<Type, ISystem>();

    public static void Add(ISystem system)
    {
        var type = system.GetType();
        if (_systems.ContainsKey(type))
            throw new InvalidOperationException($"System of type {type} is already registered.");

        _systems[type] = system;
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
