using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Model 
{
    // Start is called before the first frame update
    public Inventory_Model(DataCommandHandler _data)
    {
        data = _data;
    }
    private readonly DataCommandHandler data;  
    ///
    /// 버전.2 의존성 주입 구현
   
    #region 인벤토리 서비스
    public InventoryItem SlotInfoData(int SlotIndex)
    {
        return data.Execute_InventoryIndexInfo_Solo(SlotIndex);
    }
    public int IncreaseItem_beta(int ID,int Quantity)
    {
        return data.Execute_IncreaseItem(ID, Quantity);
    }
    public int DecreaseItem_beta(int ID,int Quantity)
    {
        return data.Execute_DecreaseItem(ID, Quantity);
    }
    public void TakeOutItem_beta(int Index ,int Quantity) 
    {
        data.Execute_TakeOutItem(Index,Quantity);
    }
    public void SwapItemData(int target1 ,int target2)
    {
        //var MainData = DataManager.data.InventoryList;
        var slotData   = data.Execute_InventoryIndexInfo_Solo(target1);
        var targetData = data.Execute_InventoryIndexInfo_Solo(target1);
        if(slotData.ID == targetData.ID)
        {
            int leftitem = data.Execute_InsertItem(target2,targetData.ID,slotData.Quantity);
            slotData.Quantity = leftitem;
            if (slotData.Quantity <= 0) data.Execute_ClearItem(target1);
        }
        else
        {
            data.Execute_ClearItem(target1);
            data.Execute_ClearItem(target2);
            data.Execute_InsertItem(target1,targetData.ID,targetData.Quantity); 
            data.Execute_InsertItem(target2,slotData.ID,slotData.Quantity); 
            
        }
    }
    public void SwapItem_beta(int startIndex, int targetIndex)
    {
     
        var startData = data.Execute_InventoryIndexInfo_Solo(startIndex);
        var targetData = data.Execute_InventoryIndexInfo_Solo(targetIndex);

        data.Execute_ClearItem(startIndex);

        if (startData.ID == targetData.ID)
        {
            int leftitem = data.Execute_InsertItem(targetIndex, startData.ID,startData.Quantity);
            if(leftitem > 0)
            {
                data.Execute_InsertItem(startIndex,startData.ID,leftitem);
            } 
                
        }
        else
        {
            Debug.Log("교체");
            data.Execute_ClearItem(targetIndex);
            data.Execute_InsertItem(targetIndex,startData.ID,startData.Quantity);
            data.Execute_InsertItem(startIndex,targetData.ID,targetData.Quantity);
        }
    }
    #endregion

    #region  아이템 정보 추출 서비스
    public ItemData_SO GetItemSOByIndex_beta(int index)
    {
        ItemData_SO info = data.Execute_GetItemSOIndex(index);
       
        if(info == null) return null; 
        return info;
    }
    public bool GetItemSOByIndex_beta_new(int index, out ItemData_SO result)
    {
        result = data.Execute_GetItemSOIndex(index);
        return result != null;
    }
 
    public ItemData_SO GetItemSOByID_beta(int ID)
    {
        var info = data.Execute_GetItemSOID(ID);
        if (info == null)
        {
            Debug.LogWarning("잘못된 ID가 저장된 상태입니다.");
            return null;
        }
        return info;
    }
    #endregion 
   
    #region 컨텍스트 섹터
    public bool UseItem()
    {
        return true;
    }
    //반환값은 타겟 인덱스 ,-1이면 명령취소
    public int SplitItem_beta(int slotIndex,int DecreaseQuantity) 
    {
        int targetIndex = data.Execute_InventoryEmptyslot();
        if (targetIndex == -1)  return -1;
        var ItemData = data.Execute_InventoryIndexInfo_Solo(slotIndex);
        if(ItemData == null)  return -1;

        data.Execute_TakeOutItem(slotIndex, DecreaseQuantity);
        data.Execute_InsertItem(targetIndex,ItemData.ID,DecreaseQuantity); 
        
        return targetIndex;
    }
    public void EquipItem_beta(int slotIndex)
    {
        var ItemData = data.Execute_GetItemSOIndex(slotIndex);

        int insideItem = data.Execute_EquipedItem(ItemData.EquipmentType,ItemData.ItemID);
        data.Execute_ClearItem(slotIndex);
        if(insideItem != 0) data.Execute_InsertItem(slotIndex ,insideItem ,1); 

    }
    #endregion
 
    
}
