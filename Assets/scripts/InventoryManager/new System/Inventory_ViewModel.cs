using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory_ViewModel : MonoBehaviour
{
    // Start is called before the first frame update
    public static Inventory_ViewModel Inventory
    {
        get => FindObjectOfType<Inventory_ViewModel>();
    }
    //인벤토리 요소 
    
    [SerializeField] 
    private Transform slotPostion;
    [SerializeField] 
    private TMP_Text DescriptionName_TMP;
    [SerializeField] 
    private TMP_Text DescriptionText_TMP;
    [SerializeField] 
    private GameObject Itemslot;

    
    //인스턴스 요소 
    private DataCommandHandler _data;
    private Inventory_Model _Model;
    
    //슬롯 주소
    private List<Inventory_View> ItemSlots = new();
    private Action<Notification> _notificationRoute;

    //최초생성 서비스
    void Awake()
    {
        _data = GameManager.DataSystem.commandHandler;
        _Model = new Inventory_Model(_data);
    }
    void OnEnable()
    {
        GameManager.NotificationSystem.Port.SubscribePayload("InventorySystem", UpdateAllSlot);
    }
    void OnDisable()
    {
        GameManager.NotificationSystem.Port.UnsubscribePayload("InventorySystem", UpdateAllSlot);
        
    }
    public void InventoryOpen()
    {
        if(ContextUIDictionary.Count == 0)ContextUpdate();

        if(slotPostion == null) return;
        
        foreach (Transform child in slotPostion.transform)
        {
            Destroy(child.gameObject);
        }
        ItemSlots.Clear();

        
        for (int i = 0; i < _data.InventoryCount(); i++)
        {
            GameObject Slot = Instantiate(Itemslot);
            Slot.transform.SetParent(slotPostion.transform, false);
            Slot.GetComponent<Inventory_View>().Init(this, i);

            ItemSlots.Add(Slot.GetComponent<Inventory_View>());
        } 
    }
    //인벤토리 슬롯 통제요소
    public void UpdateSlot(int target)
    { 
        var a = ItemSlots[target];
        ItemSlots[target].SlotUpdate();
    }
    public void UpdateAllSlot()
    {
        foreach (var slot in ItemSlots) slot.SlotUpdate();
    }
    public ItemData_SO GetItemDatabyID(int ID)
    {
        return _Model.GetItemSOByID_beta(ID);
    }
    public InventoryItem GetSlotDatabyIndex(int index)
    {
        return _Model.SlotInfoData(index);
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
        return _Model.IncreaseItem_beta(ID,Quantity);
    }
    public int ItemDecrease(int index, int Quantity)
    {
        return _Model.DecreaseItem_beta(index,Quantity);
    }

    public void TakeOutItem(int index ,int quantity)
    {
        _Model.TakeOutItem_beta(index,quantity);
    }
    public int SplitItem(int index,int itemAmount)
    {
        return _Model.SplitItem_beta(index,itemAmount);
    }
    public void EquipedItem(int index)
    {
        _Model.EquipItem_beta(index);
    }

    public void SwapItemSlot(int target1 ,int target2)
    {
        _Model.SwapItem_beta(target1,target2);
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
    
}
