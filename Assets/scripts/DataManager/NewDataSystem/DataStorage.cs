using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage 
{
    public Dictionary<int,ItemData_SO> ItemData { get; private set; }
    // 인벤토리 데이터
    public List<InventoryItem> InventoryList { get; private set; }
    // 장비칸 데이터
    public Dictionary<EquipmentTypeEnums,int> EquipedDatas { get; private set; }
    public DataStorage()
    {
        ItemData      = new();
        InventoryList = new();
        EquipedDatas  = new();
    }
}
