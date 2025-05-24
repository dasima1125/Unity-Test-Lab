using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationCenter 
{
    private readonly NotificationCore _core;
    private readonly NotificationBuffer  _buffer;
    private readonly NotificationBuffer_C  _bufferC;
    
    private bool _locked = false;
    private bool _flushCStopRequested = false;
    
    public bool IsLocked => _locked;
    public void Lock() => _locked = true;
    public void Unlock() => _locked = false;
    

    #nullable enable
    public NotificationCenter(NotificationCore core ,NotificationBuffer ? buffer = null)
    {
        _core  = core;
        _buffer = buffer;
        _bufferC = new NotificationBuffer_C();//테스트용
    }
    public void AddListener(string key, string eventName, Action<Notification> listener)
    {
        _core.Register(key, eventName, listener);
    }

    public void RemoveListener(string key, string eventName)
    {
        _core.Unregister(key, eventName);
    }

    public void Post(string key, string eventName, object sender, object? data = null, BufferSpeed? priority = null)
    {
        var notification = new Notification(key, sender, data);
     
        if(priority != null)
        {
            _buffer.Enqueue(notification, eventName);
        }
        else
        {
            _core.Notify(key, eventName, notification);
        }   
    }
    public void FlushBuffered()
    {
        if(_buffer != null)
        foreach(var (notification, eventName) in _buffer.DequeueAll())
        {
            _core.Notify(notification.Key, eventName, notification);
        }
            
    }
    //개선 버퍼타입_C , TODO : 교체완료시 기존 시스템 대체
    public void Post_C(string key, string eventName, object sender, object? data = null, BufferSpeed? priority = null)
    {
        if(_locked) return;

        var notification = new Notification(key, sender, data);
        if(priority != null)
        {
            _bufferC.EnqueueUpgrade(notification, eventName ,priority.Value);
        }
          
    }
    public void FlushBufferedC()// TODO 이건 처리타이밍을 다른방식으로 구현해야겠는데?
    {
        if(_locked || _flushCStopRequested) return;

        foreach(var(notification, eventName) in _bufferC.DequeueUpgrade(100))
        {
            if(_flushCStopRequested)
            {
                _flushCStopRequested = false;
                break;
            } 
            _core.Notify(notification.Key, eventName, notification);
        }
    }
    public void BufferKill()
    {
        Lock();
        _flushCStopRequested = true;
        _bufferC.AllKill();
    }
    
}


public class NotificationBuffer // 단순 버퍼처리
{
    private readonly Queue<(Notification, string)> _buffer = new();
    public void Enqueue(Notification notification, string eventName)
    {
        _buffer.Enqueue((notification, eventName));
    }

    public IEnumerable<(Notification, string)> DequeueAll()
    {
        while(_buffer.Count > 0)
        {
            var(notif, eventName) = _buffer.Dequeue();
            yield return (notif, eventName);
        }
    }
}

//TODO: 사실 비동기를쓸려했는데.. 시스템자체가  애당초 .. 메인루프에서작동하는데 굳이 쓰레드를 더파야하나?
public class NotificationBuffer_C 
{
    private readonly SortedDictionary<BufferSpeed, Queue<(Notification notification, string eventName)>> _priorityQueues = new();
    private readonly SortedDictionary<BufferSpeed, Queue<(Notification notification, string eventName)>> _damperQueues = new(); 
    private readonly int _maxCapacity;
    private readonly int _maxDamperCapacity;

    public NotificationBuffer_C(int maxCapacity = 1000)
    {
        _maxCapacity = maxCapacity;
        _maxDamperCapacity = maxCapacity * 100;
    }

    public void EnqueueUpgrade(Notification notification, string eventName, BufferSpeed priority)
    {
    
        int currentCount = _priorityQueues.Values.Sum(q => q.Count);
        int currentDamperCount = _damperQueues.Values.Sum(q => q.Count);
        if(currentCount >= _maxCapacity) //뎀퍼 구독
        {
            if(currentDamperCount >= _maxDamperCapacity) return; 

            if(!_damperQueues.TryGetValue(priority, out var damperQueue))
            {
                damperQueue = new Queue<(Notification, string)>();
                _damperQueues[priority] = damperQueue;
            }
            damperQueue.Enqueue((notification, eventName));
            return;
        }
    
        if(!_priorityQueues.TryGetValue(priority, out var queue)) //메인 구독
        {
            queue = new Queue<(Notification, string)>();
            _priorityQueues[priority] = queue;
        }
        queue.Enqueue((notification, eventName));
    }


    public IEnumerable<(Notification notification, string eventName)> DequeueUpgrade(int maxCount)
    {
        int damperQuota = (int)(maxCount * 0.3f); //처리비율 선언 TODO 나중에 게임 메모리상태 읽어서 동적 처리량 조절등 구현예정..아마도?
        int mainQuota = maxCount - damperQuota;

        int damperCount = 0, mainCount = 0;

        if(_priorityQueues.All(kv => kv.Value.Count == 0)) 
        {
            damperQuota = maxCount;
            mainQuota = 0;
        }

        foreach(var damperQueue in _damperQueues.OrderBy(kv => kv.Key))//댐퍼 처리
        {
            while(damperQueue.Value.Count > 0 && damperCount < damperQuota)
            {
                yield return damperQueue.Value.Dequeue();
                damperCount++;
            }
            if(damperCount >= damperQuota) break;
        }

        foreach(var priorityQueue in _priorityQueues.OrderBy(kv => kv.Key))//메인 처리
        {
            while(priorityQueue.Value.Count > 0 && mainCount < mainQuota)
            {
                yield return priorityQueue.Value.Dequeue();
                mainCount++;
            }

            if(mainCount >= mainQuota) yield break; 
        }
    }

    public void AllKill()
    {
        _priorityQueues.Clear();
        _damperQueues.Clear();
    }

    public bool HasPending => _priorityQueues.Values.Sum(q => q.Count) + _damperQueues.Count > 0;
}


public enum BufferSpeed
{
    Fast = 0,
    Medium = 1,
    SLow = 2
}
