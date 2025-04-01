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
    int maximumSize = 10;
 
    public int AddItem(int ID,int Quantity)
    {
        var data = DataManager.data.InventoryList;
        var itemdata = DataManager.data.ItemData;
        //이부분은 나중에알아서 처리할예정
        if (data.Count < maximumSize)
        {
            while (data.Count < maximumSize)
            {
                data.Add(new InventoryItem(0, 0)); 
            }
        }
      
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
        if(data[ID] == null)
        {
            Debug.LogWarning("잘못된 ID가 저장된 상태입니다.");
            return null;
        }
        ItemData_SO itemdata = data[ID];
        return itemdata;
    }
    #endregion

    #region 아이템 데이터배치 서비스
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
    #endregion
    
}
