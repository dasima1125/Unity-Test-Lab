using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Inventory_ViewModel : MonoBehaviour
{
    // Start is called before the first frame update
    private static Inventory_ViewModel instance;
    public static Inventory_ViewModel Inventory
    { 
        get => instance ?? (instance = FindObjectOfType<Inventory_ViewModel>()); 
    }
    //인벤토리 요소 
    public GameObject InventoryPanel;
    public TMP_Text DescriptionName_TMP;
    public TMP_Text DescriptionText_TMP;

    
    [SerializeField] 
    private GameObject Itemslot;
    public List<Inventory_View> ItemSlots = new();
    public Transform slotPostion;
    //인벤토리 슬롯 통제요소
    public void InventoryOpen(GameObject gameObject)
    {
        if(InventoryPanel != null) return;
        if(ContextUIDictionary.Count == 0)ContextUpdate();
        InventoryPanel = gameObject;
        
        DescriptionName_TMP  = InventoryPanel.transform.Find("inventoryDescirption/DescriptionName/Name").GetComponent<TMP_Text>();
        DescriptionText_TMP  = InventoryPanel.transform.Find("inventoryDescirption/DescriptionText/Text").GetComponent<TMP_Text>();
        slotPostion          = InventoryPanel.transform.Find("inventorySlot");

        if(slotPostion == null) return;
        foreach (Transform child in slotPostion.transform)
        {
            Destroy(child.gameObject);
        }
        ItemSlots.Clear();

        for(int i = 0; i < DataManager.data.InventoryList.Count; i++) 
        {
            GameObject Slot = Instantiate(Itemslot);
            Slot.transform.SetParent(slotPostion.transform, false);
            Slot.GetComponent<Inventory_View>().slotIndex = i;
  
            ItemSlots.Add(Slot.GetComponent<Inventory_View>());
        } 
    }
    public void UpdateSlot(int target)
    {
        ItemSlots[target].SlotUpdate();
    }
    public ItemData_SO UpadateData(int ID)
    {
        ItemData_SO data = Inventory_Model.Inventory.ItemDataReader(ID);
        return data;
    }
    public ItemData_SO GetItemDatabyIndex(int index)
    {
        ItemData_SO data = Inventory_Model.Inventory.GetItemSOByIndex(index);
        if(data == null) {Debug.Log("생성실패, 인덱스의 id가 존재하지않는 id입니다");};
        return data;
    }
    public void DeSelectAll()
    {
        foreach (var item in ItemSlots)
        {
            item.DeSelect();
        }
    }
    //인벤토리 데이터 서비스
    public int ItemAdd(int ID ,int Quantity)
    {
        int itemCount = Inventory_Model.Inventory.AddItem(ID,Quantity);
        return itemCount;
    }
    public int ItemDecrease(int index, int Quantity)
    {
        if(!Inventory_Model.Inventory.DecreaseItem(index,Quantity)) return 0;
        return Quantity;
    }
    public int SplitItem(int index,int itemAmount)
    {
        int targetIndex = Inventory_Model.Inventory.SplitItemData(index,itemAmount);
        return targetIndex;
    }
    public void EquipedItem(int index)
    {
        Inventory_Model.Inventory.EquipInventoryItem(index);

    }

    public void SwapItemData(int target1 ,int target2)
    {
        Inventory_Model.Inventory.SwapItemData(target1,target2);
        ItemSlots[target1].SlotUpdate();
        ItemSlots[target2].SlotUpdate();
    }
    //컨텍스트 패널 서비스
    public Dictionary<string, GameObject> ContextUIDictionary = new();
    public Stack<GameObject> ContextUI = new(); 

    public void ContextUpdate() 
    {
        GameObject[] InventoryUIs = Resources.LoadAll<GameObject>("InventorySystem/ContextMenuUIs_beta");
        foreach(GameObject InventoryUI in InventoryUIs)
        {
            if(!ContextUIDictionary.ContainsKey(InventoryUI.name))
            {
                //Debug.Log("" + InventoryUI.name);
                ContextUIDictionary.Add(InventoryUI.name, InventoryUI);
            }
        }
        
    }
}
