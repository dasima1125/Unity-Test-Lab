using System;
using UnityEngine;

public class Notification
{
    public string Key { get; } //시스템 목적지 주소
    public string  Sender { get; } //발신자 주소
    public object Payload { get; } //매개변수 타입
    
    
    #nullable enable
    public Notification(string key, object sender, object? payload = null)
    {
        Key = key;
        Sender = sender.GetType().Name;  
        Payload = payload; //매개없으면 인수할당 X
        
    }

    public bool TryGetPayload<T>(out T payload)
    {
        if (Payload is T typedPayload)
        {
            payload = typedPayload;
            return true; 
        }
        payload = default!; 
        return false; 
    }
}

// 확장구조
public static class NotificationExtensions
{
   
    public static void SubscribePayload<T>(this NotificationPort port, string key, Action<T> onPayload)
    {
        port.Subscribe(key, onPayload.Method.Name,notification =>
        {
            if (notification.TryGetPayload(out T typed))
                onPayload(typed);
        });
    }
    public static void SubscribePayload(this NotificationPort port, string key, Action onNotification)
    {
        port.Subscribe(key, onNotification.Method.Name, notification => // 람다식 특성상 구조를 저장하는거지 실행을 하는게 아님
        {                                
            if(notification.Payload == null) //내부 호출 로직 
                onNotification();
        });
    }
    // 구독 해제

    public static void UnsubscribePayload<T>(this NotificationPort port, string key, Action<T> onPayload)
    {
        port.Unsubscribe(key, onPayload.Method.Name);
    }
    public static void UnsubscribePayload(this NotificationPort port, string key, Action onNotification)
    {
        port.Unsubscribe(key, onNotification.Method.Name);
    }
}