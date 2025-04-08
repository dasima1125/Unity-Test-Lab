using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCommandHandler 
{
    // Start is called before the first frame update
    private readonly DataController controller;

    public DataCommandHandler(DataController controller)  // 생성자로 주입
    {
        this.controller = controller;
    }
    //아이템 정보 검색 
    public ItemData_SO Execute_GetItemSOIndex(int index) => controller.GetItemSOIndex(index);
    public ItemData_SO Execute_GetItemSOID(int ID) => controller.GetItemSOByID(ID);
    
    //인벤토리 - 전체 배열 조작구조
    public int InventoryCount() => controller.InventoryCount();
    public int Execute_IncreaseItem(int ID,int Quantity) => controller.IncreaseItem(ID,Quantity);
    public int Execute_DecreaseItem(int ID,int Quantity) => controller.DecreaseItem(ID,Quantity);
    public int Execute_InventoryEmptyslot() => controller.InventoryEmptyslot();
    //인벤토리 - 인덱스 집중 조작구조
    public void Execute_ClearItem(int index) => controller.ClearItem(index);
    public int Execute_InsertItem(int index, int ID, int Quantity) => controller.InsertItem(index, ID, Quantity);
    public int Execute_TakeOutItem(int index, int Quantity) => controller.TakeOutItem(index,Quantity);
    public InventoryItem Execute_InventoryIndexInfo_Solo(int index) => controller.InventoryIndexInfo_Solo(index);
    //장비 - 타입 집중 조작구조
    public int Excute_GetEquipedItemID(EquipmentTypeEnums type) => controller.GetEquipedItemID(type);
    public int Execute_EquipedItem(EquipmentTypeEnums type , int ID) => controller.EquipedItem(type,ID);
    public int Execute_UnequipedItem(EquipmentTypeEnums type) => controller.UnequipedItem(type);
    public Dictionary<EquipmentTypeEnums, List<(int,int)>> Excute_GetEquipmentGropbyInventory() => controller.GetEquipmentGropbyInventory();

    
}
