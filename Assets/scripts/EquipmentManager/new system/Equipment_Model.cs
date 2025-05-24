using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Model 
{
    private readonly DataCommandHandler _data;
    
    public Equipment_Model(DataCommandHandler data)
    {
        _data = data;
    }
    //정보 추출
    public ItemData_SO GetDataInfoByIndex(int index)
    {
        var ID = _data.Execute_InventoryIndexInfo_Solo(index).ID;
        var data = _data.Execute_GetItemSOID(ID);
        
        if(index < 0 || index >= _data.InventoryCount())//이부분 좀 sus함
        {
            Debug.LogWarning("잘못된 인벤토리 인덱스를 가져온 상태입니다.");
            return null;
        }
       
        return data;
    }
    public ItemData_SO GetDataInfoByID(int ID)
    {
        if(ID == 0) return null;
        return _data.Execute_GetItemSOID(ID);
    }

    public bool TryGetEquipmentType(int ID, out EquipmentTypeEnums type)
    {
        type = EquipmentTypeEnums.Null;
        var data = GetDataInfoByID(ID);
        
        if (data == null || data.ItemType != ItemTypeEnums.Equipment)
            return false;

        type = data.EquipmentType;
        return true;
    }
    

    public void EquipItem(EquipmentTypeEnums Type ,int ItemslotIndex)
    {
        var item = _data.Execute_InventoryIndexInfo_Solo(ItemslotIndex);
        var EquipItemID = _data.Excute_GetEquipedItemID(Type);
       
        int leftEquip = _data.Execute_EquipedItem(Type,item.ID);
        _data.Execute_ClearItem(ItemslotIndex);
        if(leftEquip > 0)
        {
            _data.Execute_InsertItem(ItemslotIndex,EquipItemID,1);
        }
            
    }

    public void UnequipItem(EquipmentTypeEnums Type)
    {
        var Equipedid = _data.Execute_UnequipedItem(Type);
        
        int leftover  = _data.Execute_IncreaseItem(Equipedid,1);
        if (leftover > 0)
        {
            _data.Execute_EquipedItem(Type, Equipedid);
        }
    }

    
}
