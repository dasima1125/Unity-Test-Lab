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
    public int Add_ver(string ItemName, int Quantity, Sprite sprite, string itemDescription)
    {
        Debug.Log("진입");
        var data = Inventory.itemDatas;
   
        for(int i = 0; i < data.Length; i++) 
        {
            if(data[i].IsFull == false && data[i].ItemName == ItemName || data[i].ItemQuantity == 0)
            {
                
                int leftoverItme = data[i].AddItem(ItemName, Quantity, sprite,itemDescription);
                if(leftoverItme > 0)
                {
                    leftoverItme = Add_ver(ItemName,leftoverItme,sprite,itemDescription);
                }
                if(leftoverItme <= 0 && Inventory.ItemSlot != null)
                {
                    if(Inventory.ItemSlot[i] != null)
                    Inventory.ItemSlot[i].SlotUpdate(i);
                    //Debug.Log("끝" + i);
                    //if(Inventory.ItemSlot.Count > 0)
                    //Debug.Log("슬롯 사이즈");
                    
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
                if(usAble && Inventory.itemDatas[index].ItemQuantity > 0)
                {
                    Inventory.itemDatas[index].ItemQuantity --;
                    return usAble;
                }
                
            }
        }
        Debug.Log("사용 불가");
        return false;
    }
    public void swapItem(int slotIndex,int targetIndex)
    {
        var temp = Inventory.itemDatas[targetIndex];
        
        Inventory.itemDatas[targetIndex] = Inventory.itemDatas[slotIndex];
        Inventory.itemDatas[slotIndex] = temp;
        Inventory.Updating();
    }
    public bool DropItem(int index)
    {
        if(Inventory.itemDatas[index].ItemQuantity < 0 || Inventory.testItemPrefab == null) return false;
        GameObject dropItem = Instantiate(Inventory.testItemPrefab);
        dropItem.transform.position = GameObject.Find("player").transform.position;
        
        var setting = dropItem.GetComponent<Items>();
        setting.CanPick = false;
        
        setting.AddInfo(Inventory.itemDatas[index]);
        Debug.Log("아이템 드랍 체크");
        return true;

    }
}
