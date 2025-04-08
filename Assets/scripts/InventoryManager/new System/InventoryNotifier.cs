using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InventoryNotifierType
{
    InventoryUpdated,
    ItemEquipped,
    ItemUsed,
    ItemSplit,
    SlotSwapped
}

public class InventoryNotifier 
{
    private readonly Dictionary<InventoryNotifierType, Action> notifyDict = new();
    public void Subscribe(InventoryNotifierType type, Action callback)
    {
        if (notifyDict.ContainsKey(type))
            notifyDict[type] += callback;
        else
            notifyDict[type] = callback;
    }

    public void Unsubscribe(InventoryNotifierType type, Action callback)
    {
        if (notifyDict.ContainsKey(type))
        {
            notifyDict[type] -= callback;
            if (notifyDict[type] == null)
                notifyDict.Remove(type);
        }
        
    }

    public void Notify(InventoryNotifierType type)
    {
        if (notifyDict.TryGetValue(type, out var action))
            action.Invoke();
    }

}
