using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
            Debug.LogWarning($"ServiceLocator: {type.Name} already registered, overwriting previous pair.");

        services[type] = service;
    }

    public static void Unregister<T>(T service) where T : class
    {
        Type type = typeof(T);

        if (services.TryGetValue(type, out object current) && ReferenceEquals(current, service))
            services.Remove(type);
    }

    public static T Get<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out object service))
            return (T)service;

        Debug.LogError($"ServiceLocator: There is no service of type {typeof(T).Name} registered.");
        return null;
    }
}