using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    // eventName, delegate (is UnityAction<T>)
    private Dictionary<string, Delegate> _events = new();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void AddListener<T>(string eventName, UnityAction<T> listener)
    {
        if (!_events.ContainsKey(eventName))
            _events.Add(eventName, null);

        // Note that just the + operator is the correct syntax to add a listener to a delegate that may be null or already has listeners.
        _events[eventName] = (UnityAction<T>)_events[eventName] + listener;
    }

    public void RemoveListener<T>(string eventName, UnityAction<T> listener)
    {
        if (_events.ContainsKey(eventName))
            // Note that just the - operator is the correct syntax to remove a listener to a delegate that may be null or already has listeners.
            _events[eventName] = (UnityAction<T>)_events[eventName] - listener;
    }

    public void TriggerEvent<T>(string eventName, T arg)
    {
        if (_events.TryGetValue(eventName, out Delegate del))
            (del as UnityAction<T>)?.Invoke(arg);
    }
}