using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EquipmentNotifierType
{
    InventorySlotUpdated,
    EquippedSlotUpdate,
   
}

public class EquipmentNotifier
{
    private Action _noParamEvent;
    private Action<int> _withParamEvent;

    public void Subscribe(Action listener) => _noParamEvent += listener;
    public void Subscribe(Action<int> listener) => _withParamEvent += listener;

    public void Unsubscribe(Action listener) => _noParamEvent -= listener;
    public void Unsubscribe(Action<int> listener) => _withParamEvent -= listener;

    public void Notify() => _noParamEvent?.Invoke();
    public void Notify(int ID) => _withParamEvent?.Invoke(ID);
    
}
