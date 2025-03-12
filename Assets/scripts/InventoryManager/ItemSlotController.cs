using System.Collections;
using System.Collections.Generic;
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
        // 임시 변수 사용하여 두 아이템을 교환
        var temp = Inventory.itemDatas[slotIndex];
        Inventory.itemDatas[slotIndex] = Inventory.itemDatas[targetIndex];
        Inventory.itemDatas[targetIndex] = temp;
    }

}
