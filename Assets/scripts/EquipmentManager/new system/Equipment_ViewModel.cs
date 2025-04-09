using System;
using System.Collections.Generic;
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
    private DataCommandHandler _data;
    private Equipment_ViewModel _viewmodel;
    private Equipment_Model _model;
    
    
    void Start()
    {
        _data = GameManager.DataSystem.commandHandler;
        
        _model = new Equipment_Model(_data);
        _viewmodel = this;
        

        SetUp();
    }
    void OnEnable()
    {
        GameManager.Game.EquipmentNotify.Subscribe(HandleEquipmentAcquisition);
    }
    void OnDisable()
    {
        GameManager.Game.EquipmentNotify.Unsubscribe(HandleEquipmentAcquisition);
    }
    
    public void SetUp()
    {
        if(panels == null || slotprefeb == null) return; 
     
        CreateEquippedItems();
        CreateInventoryItems();  
    }
    public void CreateEquippedItems()
    {

        foreach (Transform child in EquipedContent)
        {
            var target = child.GetComponent<Equipment_View>();
            var slotType = target.EquipedSlotType;
            var EquipID = _data.Excute_GetEquipedItemID(slotType);

            if (target != null && !EquipedSlots.ContainsKey(slotType)) EquipedSlots.Add(slotType,child.gameObject); 
            if(EquipID != 0)// 저장된아이템이있을경우
            {
            
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true,EquipID);
            }
            else// 저장된아이템이없을경우
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true);
        }

    }
    public void CreateInventoryItems()
    {
        Dictionary<EquipmentTypeEnums, List<(int,int)>> data = _data.Excute_GetEquipmentGropbyInventory();
        
        foreach (EquipmentTypeEnums type in Enum.GetValues(typeof(EquipmentTypeEnums)))
        {
            //뭐 장비타입인데 나중에 손보자 
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

        for(int i = 0; i < InventorySlots[type].Count ; i++) 
        {
            if (i < data.Count)
                InventorySlots[type][i].GetComponent<Equipment_View>().SetupSlot(false,type,data[i].Item1,i);
            
            else
                InventorySlots[type][i].GetComponent<Equipment_View>().SetupSlot(false, type);
        }

    }
    public void HighlightOnHover(EquipmentTypeEnums Type , bool check)
    {
        if (Type == EquipmentTypeEnums.Null) return;
        EquipedSlots[Type].GetComponent<Equipment_View>().PointHighligt(check);
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
    

    public void HandleEquipmentAcquisition(int ID)
    {
        if (_model.TryGetEquipmentType(ID, out var type) && EquipedSlots.ContainsKey(type))
            UpdateInventoryItems(type);
    }
    public ItemData_SO GetDataInfoByIndex(int index)
    {
        return _model.GetDataInfoByIndex(index);
    }
    
    public ItemData_SO GetDataInfoByID(int ID)
    {
        return _model.GetDataInfoByID(ID);
    }
    

    public void EquipItem(EquipmentTypeEnums Type ,int ItemslotIndex)
    {
        _model.EquipItem(Type,ItemslotIndex);

        UpdateEquippedItems(Type);
        UpdateInventoryItems(Type);
    }
    public void UnequipItem(EquipmentTypeEnums Type)
    {
        _model.UnequipItem(Type);

        UpdateEquippedItems(Type);
        UpdateInventoryItems(Type);
    }
   
    
}
