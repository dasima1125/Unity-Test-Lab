using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Equipment_ViewModel : MonoBehaviour
{
    public static Equipment_ViewModel Equipment
    {
        get => FindObjectOfType<Equipment_ViewModel>();
    }
    // Slots Adress
    private Dictionary<EquipmentTypeEnums,GameObject> EquipedSlots  = new();
    private Dictionary<EquipmentTypeEnums,List<GameObject>> InventorySlots = new();
    private Dictionary<EquipmentTypeEnums,Transform>PanelAdress = new();
    
    // UI modul
    [SerializeField] private GameObject slotprefeb;
    [SerializeField] private GameObject panels;
    [SerializeField] private Transform EquipedContent;
    [SerializeField] private Transform ScrollContent;
    // Data

    //모델로 보내야함

    public List<InventoryItem> InventoryList = new();
    public Dictionary<EquipmentTypeEnums,int> EquipedData = new();
    private DataCommandHandler _data;
    
    void Start()
    {
        _data =GameManager.DataSystem.commandHandler;

        if(EquipedData ==null) 
        {
            Debug.LogWarning("장비 모듈이 준비되어있지않습니다 : 원인, 장비데이터 색인불가");
          
            Destroy(gameObject);
        }
        SetUp();
    }
    public void SetUp()
    {
     
        if(panels == null || slotprefeb == null) return; 
        Debug.Log("업데이트 시작");
        CreateEquippedItems();
        CreateInventoryItems();
        
        
    }
    public void CreateEquippedItems()
    {

        foreach (Transform child in EquipedContent)
        {
            Debug.Log("장비칸 업데이트 : " +  child.name);
            var target = child.GetComponent<Equipment_View>();
            var slotType = target.EquipedSlotType;
            var EquipID = _data.Excute_GetEquipedItemID(slotType);

            if (target != null && !EquipedSlots.ContainsKey(slotType)) EquipedSlots.Add(slotType,child.gameObject); 
            if(EquipID != 0)// 저장된아이템이있을경우
            {
                Debug.Log("장비창 업데이트 :" + slotType);//아이디 넘겨주면됨
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true,EquipID);
            }
            else// 저장된아이템이없을경우
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true);
        }

    }
    public void CreateInventoryItems()
    {
        
        Dictionary<EquipmentTypeEnums, List<(int,int)>> data = _data.Excute_GetEquipmentGropbyInventory();
        Debug.Log("타입들 : " + string.Join(", ", data.Keys));
        
        foreach (EquipmentTypeEnums type in Enum.GetValues(typeof(EquipmentTypeEnums)))
        {
             if (type == EquipmentTypeEnums.Null || 
                 type == EquipmentTypeEnums.Tools||
                 type == EquipmentTypeEnums.MainWeapon || 
                 type == EquipmentTypeEnums.SubWeapon || 
                 type == EquipmentTypeEnums.MeleeWeapon)
                 continue;

            GameObject panel = Instantiate(panels,ScrollContent);
            panel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = type.ToString();
            PanelAdress.Add(type,panel.transform);
            
            if (!InventorySlots.ContainsKey(type)) InventorySlots[type] = new List<GameObject>(); 
            
            int minslotcount = 10;
            int nowSlotCount = 0;
            
            if(data.ContainsKey(type)) Debug.Log("타입 : " + type + " 수량 : " + data[type].Count);
            if (data.ContainsKey(type) && data[type] != null && data[type].Count > 0)    
            foreach(var item in data[type])
            {
                GameObject slot = Instantiate(slotprefeb);
                slot.transform.SetParent(panel.transform.GetChild(1), false);
                slot.GetComponent<Equipment_View>().SetupSlot(false, type, item.Item1 ,nowSlotCount);
        
                InventorySlots[type].Add(slot);        
                nowSlotCount++;
            }

            for (int i = nowSlotCount; i < minslotcount; i++)
            {
                GameObject slot = Instantiate(slotprefeb);
                slot.transform.SetParent(panel.transform.GetChild(1), false);
                slot.GetComponent<Equipment_View>().SetupSlot(false,type);
                InventorySlots[type].Add(slot);
            }
        }
        
    }
    public void UpdateEquippedItems(EquipmentTypeEnums type)
    {
        if (!EquipedSlots.ContainsKey(type))
        {
            Debug.LogWarning("슬롯접근 실패: " + type);
            return;
        }
        var ID = _data.Excute_GetEquipedItemID(type);
        if (ID != 0)
            EquipedSlots[type].GetComponent<Equipment_View>().SetUpEquipedSlot(true, ID);
        
        else
            EquipedSlots[type].GetComponent<Equipment_View>().SetUpEquipedSlot(true);
    }
    public void UpdateInventoryItems(EquipmentTypeEnums type)
    {
        Dictionary<EquipmentTypeEnums, List<(int, int)>> dataDict = _data.Excute_GetEquipmentGropbyInventory();
        if (!dataDict.TryGetValue(type, out var data)) data = new List<(int, int)>();
        
        for(int i = 0; i < data.Count - InventorySlots[type].Count;i++)//빈칸확장
        {
            GameObject slot = Instantiate(slotprefeb);
            slot.transform.SetParent(PanelAdress[type].GetChild(1), false);
            slot.GetComponent<Equipment_View>().SetupSlot(false,type);
            InventorySlots[type].Add(slot);
        }
        //새로운 데이터 순회
    
        //Debug.Log("재배치할 슬롯 상태 : " +data.Count);
        //Debug.Log("재배치할 슬롯 크기 : " +InventorySlots[type].Count);
        for(int i = 0; i < InventorySlots[type].Count ; i++) 
        {
            if (i < data.Count)
                InventorySlots[type][i].GetComponent<Equipment_View>().SetupSlot(false,type,data[i].Item1,i);
            
            else
                InventorySlots[type][i].GetComponent<Equipment_View>().SetupSlot(false, type);
        }

    }
    
    //모델로 가야지
    
    public ItemData_SO GetDataInfoByIndex(int index)
    {
        var ID = _data.Execute_InventoryIndexInfo_Solo(index).ID;
        var data2 = _data.Execute_GetItemSOID(ID);
        
        if(index < 0 || index >= _data.InventoryCount())//이부분 좀 sus함
        {
            Debug.LogWarning("잘못된 인벤토리 인덱스를 가져온 상태입니다.");
            return null;
        }
       
        return data2;
    }
    
    public ItemData_SO GetDataInfoByID(int ID)
    {
        if(ID == 0) return null;
        return _data.Execute_GetItemSOID(ID);
    }
    public void DeRaycastOther(bool type)
    {
        foreach (var slot in EquipedSlots.Values)
        {
            if(!slot.TryGetComponent<Image>(out var target))  
            {
                Debug.Log("이미지 컴포넌트 저장 오류:");
                continue;
            }
            target.raycastTarget = type;
        }
        foreach (var slotList in InventorySlots.Values) 
        {
            foreach (var slot in slotList) 
            {
                if (!slot.TryGetComponent<Image>(out var target))
                {
                    Debug.Log("이미지 컴포넌트 저장 오류:");
                    continue;
                }
                target.raycastTarget = type; 
            }
        }
    }
    
    
    /// 서비스 구획
    public void HighlightOnHover(EquipmentTypeEnums Type , bool check)
    {
        if (Type == EquipmentTypeEnums.Null) return;
        EquipedSlots[Type].GetComponent<Equipment_View>().PointHighligt(check);
    }
   

    public void EquipItem(EquipmentTypeEnums Type ,int ItemslotIndex)
    {
        Debug.Log("명령실행");
        HighlightOnHover(Type,false);
        var item = _data.Execute_InventoryIndexInfo_Solo(ItemslotIndex);
        var EquipItemID = _data.Excute_GetEquipedItemID(Type);
       
        int leftEquip = _data.Execute_EquipedItem(Type,item.ID);
        Debug.Log("있는 값 :" + leftEquip);
        _data.Execute_ClearItem(ItemslotIndex);
        if(leftEquip > 0)
        {
            Debug.Log("교체대상 :" + leftEquip);
            _data.Execute_InsertItem(ItemslotIndex,EquipItemID,1);
        }
            
            
        UpdateEquippedItems(Type);
        UpdateInventoryItems(Type);
    }
    public void UnequipItem(EquipmentTypeEnums Type)
    {
        var Equipedid = _data.Execute_UnequipedItem(Type);
        
        int leftover  = _data.Execute_IncreaseItem(Equipedid,1);
        if (leftover > 0)
        {
            _data.Execute_EquipedItem(Type, Equipedid);
        }
        UpdateEquippedItems(Type);
        UpdateInventoryItems(Type);
    }
   
    
}
