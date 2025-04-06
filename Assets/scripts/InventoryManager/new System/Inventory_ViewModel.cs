using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
    public DataCommandHandler Data;
    //인벤토리 슬롯 통제요소
    void Awake()
    {
        Data = GameManager.Data.commandHandler;
    }
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
        
        
        for(int i = 0; i < Data.InventoryCount(); i++) 
        {
            GameObject Slot = Instantiate(Itemslot);
            Slot.transform.SetParent(slotPostion.transform, false);
            Slot.GetComponent<Inventory_View>().Init(Inventory,i);

            ItemSlots.Add(Slot.GetComponent<Inventory_View>());
        } 
    }
    public void UpdateSlot(int target)
    {
        ItemSlots[target].SlotUpdate();
    }
    public ItemData_SO GetItemDatabyID(int ID)
    {
        return Inventory_Model.Inventory.ItemDataReader(ID);
    }
    public InventoryItem GetSlotDatabyIndex(int index)
    {
        return Inventory_Model.Inventory.SlotInfoData(index);
    }
    public void Select(int targetIndex ,bool type)
    {
        ItemSlots[targetIndex].Select(type);
    }
    public void DeSelectAll()
    {
        foreach (var item in ItemSlots)
        {
            item.DeSelect();
        }
    }
    //인벤토리 데이터 서비스
    public int ItemIncrease(int ID ,int Quantity)
    {
        return Inventory_Model.Inventory.IncreaseItem_beta(ID,Quantity);
    }
    public int ItemDecrease(int index, int Quantity)
    {
        return Inventory_Model.Inventory.DecreaseItem_beta(index,Quantity);
    }
    public void TakeOutItem(int index ,int quantity)
    {
        Inventory_Model.Inventory.TakeOutItem_beta(index,quantity);
    }
    public int SplitItem(int index,int itemAmount)
    {
        return Inventory_Model.Inventory.SplitItem_beta(index,itemAmount);
    }
    public void EquipedItem(int index)
    {
        Inventory_Model.Inventory.EquipItem_beta(index);
    }

    public void SwapItemSlot(int target1 ,int target2)
    {
        Inventory_Model.Inventory.SwapItem_beta(target1,target2);
        DeSelectAll();
        Select(target2,true);
      
        UpdateSlot(target1);
        UpdateSlot(target2);
    }
    //컨텍스트 패널 서비스
    public Dictionary<string, GameObject> ContextUIDictionary = new();
    public Stack<GameObject> ContextUI = new(); 

    public void ContextUpdate() 
    {
        GameObject[] InventoryUIs = Resources.LoadAll<GameObject>("InventorySystem/ContextMenuUIs_beta");
        foreach(GameObject InventoryUI in InventoryUIs)
        {
            if(!ContextUIDictionary.ContainsKey(InventoryUI.name)) ContextUIDictionary.Add(InventoryUI.name, InventoryUI);
        }
    }

    /// 의존성 주입시스템 변환
}
