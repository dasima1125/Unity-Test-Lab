using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSlotController : MonoBehaviour
{
    private InventoryManager Inventory;
    private static ItemSlotController instance;
    public static ItemSlotController controll
    { 
        get => instance ?? (instance = FindObjectOfType<ItemSlotController>()); 
    }
    void Start()
    {
        Inventory = InventoryManager.Inventory;
    }

    // [Add_ver] 반복문에서 아이템을 추가하는 과정에서 발생하는 문제점들에 대한 설명 << 해결완료

    // 1. 주요 문제: IsFull 상태와 ItemQuantity가 0인 상태에서 조건문이 잘못 설정되어 무한 재귀가 발생
    //    - `IsFull`이 true이고 `ItemQuantity`가 0인 경우에도 `if` 조건을 통과하게 되어, 
    //      계속해서 `AddItem`이 호출되고, 결국 무한 반복에 빠지게 됨.

    // 2. 원인 분석:
    //    - `IsFull == false`와 `ItemQuantity == 0`이 동시에 만족되는 경우에도 `AddItem`이 호출됨.
    //    - `AddItem`에서 `IsFull` 상태를 true로 변경하고 리턴하지만, `ItemQuantity == 0`인 상태에서 재귀가 다시 호출되면서 
    //      무한 루프가 발생함.
    public int Add_ver(string ItemName, int Quantity, Sprite sprite, string itemDescription, ItemType itemType ,EquipmentType EquipmentType)
    {
        Debug.Log("컨트롤러");
        var data = Inventory.itemDatas;

        for(int i = 0; i < data.Length; i++) 
        {
            if((data[i].IsFull == false && data[i].ItemName == ItemName) || data[i].ItemQuantity == 0)
            {
                int leftoverItme = data[i].AddItem(ItemName, Quantity, sprite,itemDescription,itemType,EquipmentType);
                if(leftoverItme > 0) leftoverItme = Add_ver(ItemName,leftoverItme,sprite,itemDescription,itemType,EquipmentType);
                
                if(leftoverItme <= 0 && Inventory.ItemSlot != null)
                {
                    if(Inventory.ItemSlot.Count > i && Inventory.ItemSlot[i] != null) Inventory.ItemSlot[i].SlotUpdate(i);
                }  
                           
                return leftoverItme;
            }    
        }
        return Quantity;
    }
    public bool UseItem(int index)
    {
        if (Inventory.itemDatas[index].ItemName == "") return false;
        for(int i = 0; i < Inventory.itemSOs.Length; i++) 
        {
            if(Inventory.itemSOs[i].itemName == Inventory.itemDatas[index].ItemName)
            {
                bool usAble = Inventory.itemSOs[i].UseItem();
                if(usAble && Inventory.itemDatas[index].DecreaseItem(1)) return usAble;
            }
        }
        return false;
    }
    public bool DropItem(int index , int itemAmount = 0 )
    {

        if(Inventory.itemDatas[index].ItemQuantity < 0 || Inventory.testItemPrefab == null) return false;
        GameObject dropItem = Instantiate(Inventory.testItemPrefab);
        dropItem.transform.position = GameObject.Find("player").transform.position;

        int dropCount = (itemAmount != 0) ? itemAmount : 1;

        if(!Inventory.itemDatas[index].DecreaseItem(dropCount))
        {
            Destroy(dropItem);
            return false;
        }
        
        var setting = dropItem.GetComponent<Items>();
        setting.DropItem(Inventory.itemDatas[index], dropCount);
        setting.CanPick = false;
        return true;

    }
    public void SwapItem(int slotIndex, int targetIndex)
    {
        ItemDTO slotData   = Inventory.itemDatas[slotIndex];
        ItemDTO targetData = Inventory.itemDatas[targetIndex];
        
        if(slotData.ItemName == targetData.ItemName)
        {
            int leftoverItem = targetData.AddItem(slotData.ItemName,slotData.ItemQuantity,slotData.ItemSprite,slotData.ItemDescription,slotData.ItemCategory,slotData.EquipmentCategory);
            
            slotData.IsFull = false;
            slotData.ItemQuantity = 0;
            slotData.AddItem(slotData.ItemName,leftoverItem,slotData.ItemSprite,slotData.ItemDescription,slotData.ItemCategory,slotData.EquipmentCategory);
            
            if (slotData.ItemQuantity == 0)
            {
                Inventory.itemDatas[slotIndex].ResetSlot();  
            } 
            else
            {
                Inventory.itemDatas[slotIndex] = slotData; 
            }  
            Inventory.itemDatas[targetIndex] = targetData;
        }
        else
        {
            Inventory.itemDatas[slotIndex]   = targetData;
            Inventory.itemDatas[targetIndex] = slotData;
        }

    }
    public int? SplitItem(int index , int itemAmount = 0)
    {
        int ? TargetIndex = null;
        for (int i = 0; i < Inventory.itemDatas.Length; i++)
        {
            var item = Inventory.itemDatas[i];  
            if (item.ItemQuantity == 0)
            {
                TargetIndex = i;
                break;  
            }
        }
        if(Inventory.itemDatas[index].ItemQuantity < 0 || TargetIndex == null) return null;
       
        Inventory.itemDatas[index].DecreaseItem(itemAmount);
        Inventory.itemDatas[TargetIndex.Value] = Inventory.itemDatas[index].CopyItemDTO();
        Inventory.itemDatas[TargetIndex.Value].ItemQuantity = itemAmount;
        
        return TargetIndex;
    }
    public void SortItemSlot() //일단 빈칸정리
    {   
        int type = 3;
        ItemDTO[] targetDatas = Inventory.itemDatas.ToArray();
        List<ItemDTO> emptySlots = targetDatas.Where(item => item.ItemQuantity == 0).ToList();

        List<ItemDTO> sortedItems = new List<ItemDTO>(); 
        switch (type)
        {
            case 1: //이름순
                List<ItemDTO> sortItems_1 = targetDatas
                                                .Where(item => item.ItemQuantity > 0)  
                                                .OrderBy(item => item.ItemName)       
                                                .ThenByDescending(item => item.ItemQuantity) 
                                                .ToList();
            
                sortedItems.AddRange(sortItems_1.ToArray()); 
            break;
            
            case 2: 
                //수량순
                //
                // 같은 아이템끼리 그룹화
                // 그룹 내 수량 합으로 내림차순 정렬
                // 이름순 정렬
                //
                //  List<IGrouping<string, ItemDTO>> 타입 //
                var sortGroupItems_2 = targetDatas
                                            .Where(item => item.ItemQuantity > 0)
                                            .GroupBy(item => item.ItemName)                                   
                                            .OrderByDescending(group => group.Sum(item => item.ItemQuantity)) 
                                            .ThenBy(group => group.Key)                                      
                                            .ToList();
                
                List<ItemDTO> sortItem_2 = new();
                
                foreach (var group in sortGroupItems_2) 
                    sortItem_2.AddRange(group.OrderByDescending(item => item.ItemQuantity));
                
                sortedItems.AddRange(sortItem_2.ToArray()); 
            break;

            case 3:
                //타입순
                //
                // 같은 아이템끼리 그룹화
                // num 순서대로 정렬
                // 그룹 내 수량 합으로 내림차순 정렬
                // 이름순 정렬
                //
                //  List<IGrouping<string, ItemDTO>> 타입 //
                int arrayType = 2;
                var sortGroupItems_3 = targetDatas
                                            .Where(item => item.ItemQuantity > 0)
                                            .GroupBy(item => item.ItemCategory)                 
                                            .OrderBy(g => (int)g.Key)
                                            .ThenByDescending(g => g.Sum(item => item.ItemQuantity))
                                            .ThenBy(g => g.Key)                                
                                            .ToList();
                //arrayType을 0 으로하고 나머지를 1로설정해서 해당인자만 맨앞으로
                Debug.Log(string.Join(",", sortGroupItems_3.Select(group => group.Key).ToArray()));
                var phase_2 = sortGroupItems_3.OrderBy(g => g.Key == (ItemType)arrayType ? 0 : 1) 
                                    .ToList();
                
                List<ItemDTO> sortItem_3 = new();// 각 그룹 분해후 추가
    
                foreach (var group in phase_2) 
                    sortItem_3.AddRange(group);
                
                sortedItems.AddRange(sortItem_3.ToArray()); 
            break;
        }
        
        sortedItems.AddRange(emptySlots);
        Inventory.itemDatas = sortedItems.ToArray();
    }

}
