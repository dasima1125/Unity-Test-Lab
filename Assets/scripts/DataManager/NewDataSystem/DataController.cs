using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataController 
{
    private readonly DataStorage storage;

    public DataController(DataStorage storage)  // 생성자로 주입
    {
        this.storage = storage;
    }
    #region 아이템 정보 서비스 구획
    public ItemData_SO GetItemSOByID(int ID)
    {
        var data = storage.ItemData;
        if (!data.ContainsKey(ID))
        {
            Debug.LogWarning("잘못된 ID가 저장된 상태입니다.");
            return null;
        }
        return data[ID];
    }
    
    #endregion

    #region 인벤토리 제어 시스템 구획
    //
    // 지정 인덱스 제어 서비스
    //
    public int InsertItem(int index, int ID, int Quantity)
    {
        var data = storage.InventoryList[index];
        var itemdata = storage.ItemData;
        
        if(ID == 0 || Quantity == 0) 
        {   ClearItem(index);
            return 0;
        }
        
        if(data.Quantity >= itemdata[ID].MaxNumberItems) return Quantity; 
    
        data.ID = ID;     
        data.Quantity += Quantity;
        
        if(data.Quantity > itemdata[ID].MaxNumberItems)
        {
            int OverQuantity = data.Quantity;
            data.Quantity = itemdata[ID].MaxNumberItems;

            return OverQuantity - itemdata[ID].MaxNumberItems;
        }
        return 0;
    }
    public int TakeOutItem(int index,int Quantity)
    {
        var data = storage.InventoryList;
       
        if(data[index].Quantity <= 0 || data[index].Quantity < Quantity) return Quantity;
        
        data[index].Quantity -= Quantity;

        if(data[index].Quantity <= 0)
        {
            var output = -data[index].Quantity;
            ClearItem(index);
            return output;
        } 
        return 0;
    }
    
    public void ClearItem(int index)
    {
        var target =storage.InventoryList[index];
        target.ID       = 0;
        target.Quantity = 0;
    }
    public ItemData_SO GetItemSOIndex(int index)
    {
        if (index < 0 || index >= storage.InventoryList.Count)
        {
            Debug.LogWarning("인덱스 범위 에러");
            return null;
        } 
        int data = storage.InventoryList[index].ID;
        if (data == 0) 
        {
            Debug.LogWarning("DataController.GetItemSOByInventoryIndex()  : 존재하지않는아이템.");
            return null;
        }
        return storage.ItemData[data];
    }
    public InventoryItem InventoryIndexInfo_Solo(int index)
    {
        if (index < 0 || index >= storage.InventoryList.Count) return null; 
        var data = storage.InventoryList[index];
       
        return new InventoryItem(data.ID ,data.Quantity);
    }
    
    

    //
    // 순환 인덱스 제어 서비스
    //
    public int InventoryItemCount(int ID) 
    {
        int count = 0;
        foreach(var item in storage.InventoryList)
            if(item.ID == ID && item.Quantity > 0) count += item.Quantity;

        return count;
    }
    public int InventoryEmptyslot() 
    {
        int index = -1;
        for(int i = 0; i < storage.InventoryList.Count; i++) 
            if(storage.InventoryList[i].ID == 0 && storage.InventoryList[i].Quantity == 0) return i;
        
        return index;
    }
    public int InventoryCount() 
    {
        return storage.InventoryList.Count;
    }
    public int IncreaseItem(int ID,int Quantity)
    {
        var data = storage.InventoryList;
        
        for(int i = 0; i < data.Count; i++) 
        {   //아이디가 같고 최대수량 아닐경우 or 아이디가 0이고 수량도0일경우
            if((data[i].ID == ID && data[i].Quantity < storage.ItemData[ID].MaxNumberItems)|| (data[i].ID == 0 && data[i].Quantity == 0))
            {
                int leftoverItem = InsertItem(i, ID, Quantity);
                if(leftoverItem > 0) 
                {
                    leftoverItem = IncreaseItem(ID , leftoverItem);
                }
                return leftoverItem;
            }
        }
        return Quantity;
    }
    public int DecreaseItem(int ID,int Quantity)
    {
        var data = storage.InventoryList;

        for (int i = data.Count - 1; i >= 0; i--)
        {
            if(data[i].ID == ID && data[i].Quantity > 0)
            {
                int leftoverItem = TakeOutItem(i, Quantity);
                if(Quantity > 0) 
                {
                    leftoverItem = DecreaseItem(ID , leftoverItem);
                }
                return leftoverItem;
            }
        }
        return Quantity;
    }
    #endregion

    #region 장비 관리 시스템
    public int EquipedItem(EquipmentTypeEnums type , int ID)
    {
        //var data = storage.EquipedDatas[type]; 잠시 테스트용
        
        /**
        if(data != 0)
        {
            int SwapItemID = data;
            storage.EquipedDatas[type] = ID;
            
            return SwapItemID;
        }
        else storage.EquipedDatas[type] = ID;
        */
        
        var data = DataManager.data.EquipedDatas[type];
        if(data != 0)
        {
            int SwapItemID = data;
            DataManager.data.EquipedDatas[type] = ID;
            
            return SwapItemID;
        }
        else DataManager.data.EquipedDatas[type] = ID;
        return 0;
    }
    public int UnequipedItem(EquipmentTypeEnums type)
    {
        var data = storage.EquipedDatas[type];
        if (data == 0) return 0;
        
        storage.EquipedDatas[type] = 0;
        return data;
    }
    public Dictionary<EquipmentTypeEnums, List<(int,int)>> GetEquipmentGropbyInventory()
    {
        Dictionary<EquipmentTypeEnums, List<(int,int)>> output = new();

        var pickItem = storage.InventoryList
            .Select((item, index) => new { item, index })
            .Where(a => storage.ItemData.TryGetValue(a.item.ID, out var data) && data.ItemType == ItemTypeEnums.Equipment);
        
        foreach (var target in pickItem)
        {
            if(!output.ContainsKey(storage.ItemData[target.item.ID].EquipmentType)) output[storage.ItemData[target.item.ID].EquipmentType] = new();
            
            output[storage.ItemData[target.item.ID].EquipmentType].Add((target.index ,target.item.ID));
        }
        return output;                       
    }
    
    #endregion
}
