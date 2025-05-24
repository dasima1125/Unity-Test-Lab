using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationCore 
{
    private readonly Dictionary<string, Dictionary<string, Action<Notification>>> _listeners = new();
    private readonly Action<string, Notification ,bool> _debugLog; 
    public NotificationCore(Action<string, Notification ,bool> debugLog = null)
    {
        _debugLog = debugLog;
    }
    public void Register(string key, string eventName, Action<Notification> listener)
    {   
        if(!_listeners.TryGetValue(key, out var eventDict))
        {
            eventDict = new Dictionary<string, Action<Notification>>();
            _listeners[key] = eventDict;
        }
        eventDict[eventName] = listener;
    }

    public void Unregister(string key, string eventName)
    {
        if (_listeners.TryGetValue(key, out var eventDict))
        {
            if (eventDict.ContainsKey(eventName)) eventDict.Remove(eventName);
            if (eventDict.Count == 0) _listeners.Remove(key);
            //Debug.Log("명령성공" + eventName);
        }
        else
        {
            //Debug.Log("명령실패" + eventName);
        }
    }
    public void ClearAllSubscribers()
    {
        _listeners.Clear();
    }
   
    public void Notify(string key, string eventName, Notification notification)
    {
        bool logCheck = false;

        if (_listeners.TryGetValue(key, out var eventDict) && eventDict.TryGetValue(eventName, out var handler))
        {
            handler?.Invoke(notification);
            logCheck = true;
        }

        _debugLog?.Invoke(key, notification, logCheck);

    }
        
}
