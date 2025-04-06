using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Model : MonoBehaviour
{
    // Start is called before the first frame update
    private static Inventory_Model instance;
    public static Inventory_Model Inventory
    { 
        get => instance ?? (instance = FindObjectOfType<Inventory_Model>()); 
    }

     
    #region 아이템 습득 시스템
    public int AddItem(int ID,int Quantity)
    {
        var data = DataManager.data.InventoryList;
        var itemdata = DataManager.data.ItemData;
        
        for(int i = 0; i < data.Count; i++) 
        {   
            if((data[i].ID == ID && data[i].Quantity < itemdata[ID].MaxNumberItems)|| (data[i].ID == 0 && data[i].Quantity == 0))
            {
                int leftoverItem = InsertItem(i, ID, Quantity);
                if(leftoverItem > 0) 
                {
                    leftoverItem = AddItem(ID , leftoverItem);
                }
                return leftoverItem;
            }
        }
        Debug.Log("남은수량 : " + Quantity);
        return Quantity;
    }
    public bool DecreaseItem(int index,int Quantity)
    {
        var data = DataManager.data.InventoryList;
        if(data[index].Quantity <= 0 || data[index].Quantity < Quantity) return false;
        data[index].Quantity -= Quantity;

        if(data[index].Quantity == 0)
            data[index].ID = 0;

        return true;
    }
    
    public int InsertItem(int index, int ID, int Quantity)
    {
        var data = DataManager.data.InventoryList[index];
        var itemdata = DataManager.data.ItemData;
        if(data.Quantity >= itemdata[ID].MaxNumberItems)
        {
            return Quantity; 
        } 
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
    #endregion
    #region  아이템 정보 추출 서비스
    public ItemData_SO GetItemSOByIndex(int index)
    {
        int data =DataManager.data.InventoryList[index].ID;
       
        if(data == 0) return null; 
        return DataManager.data.ItemData[data];
    } 
    public ItemData_SO ItemDataReader(int ID)
    {
        var data = DataManager.data.ItemData;
        if (!data.ContainsKey(ID))
        {
            Debug.LogWarning("잘못된 ID가 저장된 상태입니다.");
            return null;
        }
        ItemData_SO itemdata = data[ID];
        return itemdata;
    }
    #endregion

    #region 아이템 데이터배치 서비스
    
    public int SplitItemData(int slotIndex,int DecreaseQuantity) 
    {
        int targetIndex = -1;
        for(int i = 0;i < DataManager.data.InventoryList.Count;i++)
        {
            var item = DataManager.data.InventoryList[i];
            if(item.ID == 0 && item.Quantity == 0)
            {
                targetIndex = i;
                break;
            }
        }
        if(targetIndex == -1) return targetIndex -1;

        var data = DataManager.data.InventoryList;
        data[slotIndex].Quantity -= DecreaseQuantity;
        
        data[targetIndex].ID = data[slotIndex].ID;
        data[targetIndex].Quantity = DecreaseQuantity;
        
        return targetIndex;
    }
    public void EquipInventoryItem(int slotIndex)
    {
        Debug.Log("작업 시작");
        var data = DataManager.data.InventoryList;
        var targetdata = DataManager.data.EquipedDatas;

        var item = GetItemSOByIndex(slotIndex);
        if (targetdata.TryGetValue(item.EquipmentType, out var equippedItemID))
        {
            targetdata[item.EquipmentType] = item.ItemID;
            data[slotIndex].ID       = ItemDataReader(equippedItemID).ItemID;
            data[slotIndex].Quantity = 1;
        }
        else
        {
            targetdata[item.EquipmentType] = item.ItemID;
            data[slotIndex].ID = 0;
            data[slotIndex].Quantity = 0;
        }
    }
    #endregion


    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///
    /// 버전.2 의존성 주입 구현
    /// 
    ///
    private DataSystem _data;
    private DataCommandHandler data;
    public void Init (DataSystem data)
    {
        Debug.Log("아마 적용됨?");
        _data = data;
        this.data = _data.commandHandler;
    }   
    
    /**
    private readonly DataSystem _data;
    public Inventory_Model(DataSystem data)
    {
        _data = data;
    }
    */
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
        var MainData = DataManager.data.InventoryList;
        var slotData   = MainData[target1];
        var targetData = MainData[target2];
        if(slotData.ID == targetData.ID)
        {
            int leftitem = InsertItem(target2,targetData.ID,slotData.Quantity);
            slotData.Quantity = leftitem;
            if (slotData.Quantity <= 0) slotData.ID = 0;
        }
        else
        {
            MainData[target1] = targetData;
            MainData[target2] =   slotData;
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
 
    public ItemData_SO GetItemSOByID_bet(int ID)
    {
        var data = DataManager.data.ItemData;
        if (!data.ContainsKey(ID))
        {
            Debug.LogWarning("잘못된 ID가 저장된 상태입니다.");
            return null;
        }
        ItemData_SO itemdata = data[ID];
        return itemdata;
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
