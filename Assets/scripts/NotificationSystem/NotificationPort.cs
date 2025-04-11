using System;
using UnityEngine;

public class NotificationPort 
{
    private readonly NotificationCenter _center;
    private readonly Action<string, Action<Notification> ,bool> _debugLog; 

    public NotificationPort(NotificationCenter center ,Action<string, Action<Notification> ,bool> debugLog = null)
    {
        _center = center;
        _debugLog = debugLog;
    }
    
    public void Subscribe(string key, string eventName, Action<Notification> listener)
    {   
        _center.AddListener(key, eventName, listener);
        _debugLog?.Invoke(key, listener, true);
    }

    public void Unsubscribe(string key, string eventName, Action<Notification> listener)
    {
        _center.RemoveListener(key, eventName, listener);
        _debugLog?.Invoke(key, listener, false);
    }

    #nullable enable
    public void Send(string key, string eventName, object sender, object? data = null ,BufferSpeed? priority = null)
    {
        _center.Post(key, eventName, sender, data, priority);
    }
    public void ProcessBuffer() 
    {
        _center.FlushBuffered();
    }
    
}
