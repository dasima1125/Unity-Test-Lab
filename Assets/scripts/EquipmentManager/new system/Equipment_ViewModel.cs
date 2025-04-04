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
    public Dictionary<int, ItemData_SO> ItemData = new();
    public List<InventoryItem> InventoryList = new();
    public Dictionary<EquipmentTypeEnums,int> EquipedData = new();
    
    void Start()
    {
        ItemData = DataManager.data.ItemData;
        InventoryList = DataManager.data.InventoryList;
        EquipedData = DataManager.data.EquipedDatas;
        if(ItemData == null || EquipedData ==null) 
        {
            Debug.LogWarning("장비 모듈이 준비되어있지않습니다 : 원인, 장비데이터 색인불가");
          
            Destroy(gameObject);
        }
        SetUp();
    }
    public void SetUp()
    {
        if(InventoryManager.Inventory == null) return;
        
 
        if(panels == null || slotprefeb == null) return; 
        CreateEquippedItems();
        CreateInventoryItems();
        Debug.Log(string.Join(",", DataManager.data.EquipedDatas.Keys));
        
    }
    public void CreateEquippedItems()
    {
        //EquipedSlots.Clear();
        foreach (Transform child in EquipedContent)
        {
            var target = child.GetComponent<Equipment_View>();   
            //아이디호출은  모델로 전이시켜야함
            var grapTest = DataManager.data.EquipedDatas;

            if (target != null && !EquipedSlots.ContainsKey(target.EquipedSlotType)) EquipedSlots.Add(target.EquipedSlotType,child.gameObject); 
            if(grapTest.ContainsKey(target.EquipedSlotType))// 저장된아이템이있을경우
            {
                Debug.Log("장비창 업데이트 :" + target.EquipedSlotType);
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true,grapTest[target.EquipedSlotType]);
            }
            else// 저장된아이템이없을경우
                target.GetComponent<Equipment_View>().SetUpEquipedSlot(true);
        }

    }
    public void CreateInventoryItems()
    {
        //초기화작업
        //foreach (Transform child in ScrollContent)
        //{
        //    Destroy(child.gameObject);
        //}
        //InventorySlots.Clear();
        
        Dictionary<EquipmentTypeEnums, List<(int,int)>> data = InventoryDataGrap_delta();
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

        if (DataManager.data.EquipedDatas.TryGetValue(type, out var ID))
            EquipedSlots[type].GetComponent<Equipment_View>().SetUpEquipedSlot(true, ID);
        
        else
            EquipedSlots[type].GetComponent<Equipment_View>().SetUpEquipedSlot(true);
    }
    public void UpdateInventoryItems(EquipmentTypeEnums type)
    {
        Dictionary<EquipmentTypeEnums, List<(int, int)>> dataDict = InventoryDataGrap_delta();
        if (!dataDict.TryGetValue(type, out var data)) data = new List<(int, int)>();
        
        for(int i = 0; i < data.Count - InventorySlots[type].Count;i++)//빈칸확장
        {
            GameObject slot = Instantiate(slotprefeb);
            slot.transform.SetParent(PanelAdress[type].GetChild(1), false);
            slot.GetComponent<Equipment_View>().SetupSlot(false,type);
            InventorySlots[type].Add(slot);
        }
        //새로운 데이터 순회
    
        Debug.Log("재배치할 슬롯 상태 : " +data.Count);
        Debug.Log("재배치할 슬롯 크기 : " +InventorySlots[type].Count);
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
        var target = DataManager.data.InventoryList[index];
    
        var data = DataManager.data.ItemData;
        if(index < 0 || index >= DataManager.data.InventoryList.Count)//이부분 좀 sus함
        {
            Debug.LogWarning("잘못된 인벤토리 인덱스를 가져온 상태입니다.");
            return null;
        }
       
        return data[target.ID];
    }
    public ItemData_SO GetDataInfoByID(int ID)
    {
        if(ID == 0) return null;
        return DataManager.data.ItemData[ID];
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
    //너도가야지
    /// <summary>
    /// 인덱스 , 아이디
    /// </summary>
    public Dictionary<EquipmentTypeEnums, List<(int,int)>> InventoryDataGrap_delta()
    {
        Dictionary<EquipmentTypeEnums, List<(int,int)>> output = new();

        var pickItem = InventoryList
            .Select((item, index) => new { item, index })
            .Where(a => ItemData.TryGetValue(a.item.ID, out var data) && data.ItemType == ItemTypeEnums.Equipment);
        
        foreach (var target in pickItem)
        {
            if(!output.ContainsKey(ItemData[target.item.ID].EquipmentType)) output[ItemData[target.item.ID].EquipmentType] = new();
            
            output[ItemData[target.item.ID].EquipmentType].Add((target.index ,target.item.ID));
        }
        // 장비 enum순으로 재배치
        return output
                .OrderBy(kvp => (int)kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                           
    }
    /// 서비스 구획
    public void HighlightOnHover(EquipmentTypeEnums Type , bool check)
    {
        if (Type == EquipmentTypeEnums.Null) return;
        EquipedSlots[Type].GetComponent<Equipment_View>().PointHighligt(check);
    }
    public void EquipItem(EquipmentTypeEnums Type ,int targetIndex)
    {
        Debug.Log(" 슬롯크기 "+ InventorySlots[Type].Count);
        HighlightOnHover(Type,false);
        var output = DataManager.data.InventoryList[targetIndex];//빼야할곳
        
        if (DataManager.data.EquipedDatas.TryGetValue(Type, out var previousItem))
        {
            Debug.Log(" 교체 실시");
            var copyID = previousItem;
            DataManager.data.EquipedDatas[Type] = DataManager.data.InventoryList[targetIndex].ID;
            DataManager.data.InventoryList[targetIndex] = new InventoryItem(copyID,1);
        }
        else
        {
            Debug.Log(" 장착 실시");
            DataManager.data.EquipedDatas[Type] = DataManager.data.InventoryList[targetIndex].ID;
            output.ID = 0;
            output.Quantity = 0;
        }
        UpdateEquippedItems(Type);
        UpdateInventoryItems(Type);
 
    }

    
    public void UnequipItem(EquipmentTypeEnums Type)
    {
        var target = DataManager.data.EquipedDatas;
        var data = DataManager.data.InventoryList;
        if(!target.ContainsKey(Type) || target[Type] == 0)
        {
            Debug.Log("빈슬롯");
            return;
        }

        int emptyIndex = data.FindIndex(item => item.ID == 0);
        if (emptyIndex != -1)
        {
            data[emptyIndex] = new InventoryItem(target[Type], 1);
            target.Remove(Type);
            Debug.Log("삭제완료" + target.Count);
            UpdateEquippedItems(Type);
            UpdateInventoryItems(Type);
        }
        
        Debug.Log("해제 실패 : 빈공간이 없습니다");
    }
}
