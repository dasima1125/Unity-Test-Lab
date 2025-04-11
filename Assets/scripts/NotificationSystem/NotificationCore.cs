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
   
    public void Unregister(string key, string eventName, Action<Notification> listener)
    {
        if (_listeners.TryGetValue(key, out var eventDict))
        {
            if(eventDict.ContainsKey(eventName)) eventDict.Remove(eventName);
            if(eventDict.Count == 0) _listeners.Remove(key);
        }
    }
    public void ClearAllSubscribers()
    {
        _listeners.Clear();
    }
   
    public void Notify(string key, string eventName, Notification notification)
    {
        if(_listeners.TryGetValue(key, out var eventDict) && eventDict.TryGetValue(eventName, out var handler))
        {
            _debugLog?.Invoke(key, notification, true);
            handler?.Invoke(notification);
            return;
        }

        _debugLog?.Invoke(key, notification, false);
        return;
    }
        
}
