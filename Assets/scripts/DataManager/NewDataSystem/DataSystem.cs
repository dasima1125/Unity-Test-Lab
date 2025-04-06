using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSystem 
{
    private readonly DataStorage storage;  // 데이터 저장소 
    private readonly DataController controller;  // 데이터 조작 시스템 
    public readonly DataCommandHandler commandHandler; //데이터 접근 시스템
    
    public DataSystem()
    {
        // 각 모듈을 한 번만 생성 (변경 불가능)
        storage        = new DataStorage();
        controller     = new DataController(storage);
        commandHandler = new DataCommandHandler(controller);

        InitializeData();
    }
    public DataCommandHandler GetCommandHandler() => commandHandler;
    public void InitializeData() 
    { 
        /* 아이템 데이터 로드 */
        var data = Resources.LoadAll<ItemData_SO>("itemData");
        foreach (var item in data)
        {
            if (!storage.ItemData.ContainsKey(item.ItemID)) 
                storage.ItemData.Add(item.ItemID, item);
            else
                Debug.LogWarning("중복된 아이디 발견, 대상 :  " 
                + item.ItemName  
                + " / " 
                +storage.ItemData[item.ItemID].ItemName);
        }
        // 인벤토리 데이터 설정(일단 빈칸임)
        int maximumSize = 5;
        if (storage.InventoryList.Count < maximumSize)
        {
            while (storage.InventoryList.Count < maximumSize)
            {
                storage.InventoryList.Add(new InventoryItem(0, 0)); 
            }
        }
        //장비칸 설정
        foreach (EquipmentTypeEnums type in Enum.GetValues(typeof(EquipmentTypeEnums)))
        {
            if (type == EquipmentTypeEnums.Null) continue;
            storage.EquipedDatas[type] = 0;
            
        }
    
    }
    public void SaveData() { /*TODO 데이터 저장 */ }
    public void LoadData() { /*TODO 저장된 데이터 불러오기 */ }
}
