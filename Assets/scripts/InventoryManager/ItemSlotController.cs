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
    public int Add_ver(string ItemName, int Quantity, Sprite sprite, string itemDescription)
    {
        var data = Inventory.itemDatas;

        for(int i = 0; i < data.Length; i++) 
        {
            if((data[i].IsFull == false && data[i].ItemName == ItemName) || data[i].ItemQuantity == 0)
            {
                int leftoverItme = data[i].AddItem(ItemName, Quantity, sprite,itemDescription);
                if(leftoverItme > 0) leftoverItme = Add_ver(ItemName,leftoverItme,sprite,itemDescription);
                
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
    public bool DropItem(int index)
    {
        if(Inventory.itemDatas[index].ItemQuantity < 0 || Inventory.testItemPrefab == null) return false;
        GameObject dropItem = Instantiate(Inventory.testItemPrefab);
        dropItem.transform.position = GameObject.Find("player").transform.position;

        int dropCount = 1;

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
            int leftoverItem = targetData.AddItem(slotData.ItemName,slotData.ItemQuantity,slotData.ItemSprite,slotData.ItemDescription);
            
            slotData.IsFull = false;
            slotData.ItemQuantity = 0;
            slotData.AddItem(slotData.ItemName,leftoverItem,slotData.ItemSprite,slotData.ItemDescription);
            
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
    public void SortItemSlot() //일단 빈칸정리
    {   
        /**
        int type = 1;
        // 이름순
        // 수량순
        ItemDTO[] SortDatas = new ItemDTO[Inventory.itemDatas.Length];
        List<ItemDTO>SortDatasName = new();
    
        int index = 0;
        foreach(ItemDTO item in Inventory.itemDatas)
        {
            if (item.ItemQuantity > 0)
            {
                //SortDatas[index] = item;
                SortDatasName.Add(item);
                index++;
            }
        }
        Debug.Log("존재하는 인벤토리 아이템 칸 수: " +SortDatasName.Count);

        
        switch (type)
        {
            case 1:
            
            SortDatasName.Sort((a, b) =>
            {
                int nameCompare = a.ItemName.CompareTo(b.ItemName); // 이름 기준 정렬 (오름차순)
                if (nameCompare == 0) 
                    return b.ItemQuantity.CompareTo(a.ItemQuantity); // 같은 이름이면 수량 많은 순 (내림차순)
                return nameCompare;
            });
            break;
            case 2:

            break;
            case 3:

            break;
            
        }
        for (int i = 0; i < SortDatasName.Count; i++)
        {
            SortDatas[i] = SortDatasName[i];
        }

        for (int i = index; i < SortDatas.Length; i++)
        {
            SortDatas[i] = new ItemDTO(null, 0, null, null,Inventory.NullItemSprite);
        }

        Debug.Log("빈칸 제거");
        Inventory.itemDatas = SortDatas;
        */

        ItemDTO[] targetDatas = Inventory.itemDatas.ToArray();
        int type = 2;

        switch (type)
        {
            case 1: //이름순
            
                List<ItemDTO> sortItems_1 = targetDatas.Where(item => item.ItemQuantity > 0)  
                                                        .OrderBy(item => item.ItemName)       
                                                        .ThenByDescending(item => item.ItemQuantity) 
                                                        .ToList();
        
                List<ItemDTO> emptySlots_1 = targetDatas.Where(item => item.ItemQuantity == 0) 
                                                        .ToList(); 
            
                sortItems_1.AddRange(emptySlots_1);
                Inventory.itemDatas = sortItems_1.ToArray();
            break;
            
            case 2: //수량순
                List<IGrouping<string, ItemDTO>> sortGroupItems_2 = targetDatas.Where(item => item.ItemQuantity > 0)
                                                    .GroupBy(item => item.ItemName)// 같은 아이템끼리 그룹화
                                                    .OrderByDescending(group => group.Sum(item => item.ItemQuantity)) // 그룹 내 수량 합으로 내림차순 정렬
                                                    .ThenBy(group => group.Key) // 만약 같은수량이면 뭐 이름순으로 하자
                                                    .ToList();
                
                List<ItemDTO> emptySlots_2 = targetDatas.Where(item => item.ItemQuantity == 0)
                                                                    .ToList();
                
                List<ItemDTO> sortItem_2 = new List<ItemDTO>();// 각 그룹 분해후 추가
                foreach (var group in sortGroupItems_2) sortItem_2.AddRange(group.OrderByDescending(item => item.ItemQuantity));// 람다식으로 수량 많은 순으로
                
                sortItem_2.AddRange(emptySlots_2);
                Inventory.itemDatas = sortItem_2.ToArray();
            break;
            case 3:

            break;
            
        }
    
        
    }

}
