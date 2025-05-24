using System;
using UnityEngine;

public class NotificationSystem 
{
    private readonly NotificationCore _core;
    private readonly NotificationCenter _center;
    public NotificationPort Port { get; private set; }

    public NotificationSystem()
    {
        //인식못할땐 강제 캐싱하면됨Debug.Log("das");
        _core   = new NotificationCore(DebugLog);
        _center = new NotificationCenter(_core , new NotificationBuffer());
        
        Port = new NotificationPort(_center,LogSubscription);
    }

    // 관제 시스템
    public bool DebugLogEnabled = true; 
    private void DebugLog(string key, Notification notification , bool success)
    {
        if (!DebugLogEnabled) return;

        string payloadStr = notification.Payload != null ? notification.Payload.ToString() : "null";
        string senderStr = notification.Sender != null ? notification.Sender.ToString() : "null";
        string result = success ? "✔" : "✘";;
        
        Debug.Log($"📢 [NotifySys] 주소: {key}\n호출자: {senderStr}\n매개인수: {payloadStr}\n호출 여부: {result}");
    }
    private void LogSubscription(string key,  bool isSubscribed)
    {
        if (!DebugLogEnabled) return;
  
        string status = isSubscribed ? "구독됨" : "구독 해제됨";
        Debug.Log($"📢 [NotifySys] {status}: 키: {key}");
    }
    
}
//모킹용
//협업을 해야 단위테스트를 하든 할텐데..
/**
public interface INotificationCore
{
    bool Notify(string key, Notification notification);
}
public interface INotificationCenter
{
    void Post(string key, object sender, object? payload);
}
*/