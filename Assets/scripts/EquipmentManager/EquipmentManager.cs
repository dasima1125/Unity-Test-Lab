using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static EquipmentManager Equipment
    {
        get => FindObjectOfType<EquipmentManager>(); // 항상 현재 활성화된 EquipmentManager를 찾아 반환
    }
    [SerializeField] private GameObject slots;
    [SerializeField] private Transform UpdateRight;
    [SerializeField] private Transform UpdateLeft;
    

    [SerializeField] private bool Activated = false;
    public Dictionary<EquipmentType,EquipmentSlot> EquipmentSlots = new();
    List<Image> SlotRayCastList = new();
    void Start()
    {
        if(InventoryManager.Inventory == null) return;

        Transform parentTransform = transform.GetChild(1).GetChild(0);
       
        EquipmentUpdate();
        /////// 장비칸 
        ///
        
        foreach (Transform child in parentTransform)
        {
            var target = child.GetComponent<EquipmentSlot>();
            if (target != null && !EquipmentSlots.ContainsKey(target.EquipedSlotType))
            {
                EquipmentSlots.Add(target.EquipedSlotType,target); 
            }
            
            target.EquipedSlot = true;
            Image imageComponent = child.GetComponent<Image>();
            if (imageComponent != null) SlotRayCastList.Add(imageComponent);
            
        }
    }
    public void EquipmentUpdate()
    {
        string [] testSlotsName = {"헬맷","상의","하의","신발"};
        
        
        //int testint = testSlots.Length;
        if(UpdateLeft == null || slots == null || UpdateLeft.childCount == 0) return; 
        GameObject LeftInventoryPanel = UpdateLeft.GetChild(0).gameObject;
        Dictionary<EquipmentType, List<ItemDTO>> equipmentDictionary = InventoryDataGrap();


        //외부 슬롯
        foreach (var equipmentType in equipmentDictionary)
        {
            GameObject panel = Instantiate(LeftInventoryPanel,UpdateLeft);
            panel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = equipmentType.Key.ToString();
            int minSlot = 15;
            int nowSlotCount = 0;
            foreach (var item in equipmentType.Value)
            {
                GameObject slot = Instantiate(slots);
                slot.transform.SetParent(panel.transform.GetChild(1), false);
                SlotRayCastList.Add(slot.GetComponent<Image>());// 슬롯 선택 상태 기입
                //정보 기입
                var send = slot.GetComponent<EquipmentSlot>();
                send.Setinfo(equipmentType.Key,item);
                send.slotIndex = nowSlotCount;
     
                nowSlotCount++;
            }
            if(nowSlotCount < minSlot)
            for(int i = 0; i < minSlot - nowSlotCount; i++) 
            {
                GameObject slot = Instantiate(slots);
                slot.transform.SetParent(panel.transform.GetChild(1), false);
                SlotRayCastList.Add(slot.GetComponent<Image>());

                slot.GetComponent<EquipmentSlot>().Setinfo(equipmentType.Key,null);
            }    
        }
        Destroy(UpdateLeft.GetChild(0).gameObject);
    }
    
    public void DeRaycastOther(bool type)
    {
        foreach (var image in SlotRayCastList)
        {
            image.raycastTarget = type;
        }
    }
    public Dictionary<EquipmentType, List<ItemDTO>> InventoryDataGrap()
    {
        var alpha = InventoryManager.Inventory.itemDatas
            .Where(item => item.ItemCategory == ItemType.Equipment);
        
        Dictionary<EquipmentType, List<ItemDTO>> equipmentDictionary = new();
        foreach (var item in alpha)
        {
            if(item.ItemQuantity <= 0) continue;

            if (!equipmentDictionary.ContainsKey(item.EquipmentCategory))//없는거 넣기 
                equipmentDictionary[item.EquipmentCategory] = new List<ItemDTO>();
            
            equipmentDictionary[item.EquipmentCategory].Add(item);
            
        }
        equipmentDictionary = equipmentDictionary
                                .OrderBy(kvp => (int)kvp.Key)
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return equipmentDictionary;
    }
}
