using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject panels;
    [SerializeField] private Transform UpdateRight;
    [SerializeField] private Transform UpdateLeft;
    
    public Dictionary<EquipmentType,EquipmentSlot> EquipmentSlots = new();
    List<Image> SlotRayCastList = new();
    
    void Start()
    {
        EquipmentUpdate();
    }
    public void EquipmentUpdate()
    {
        if(InventoryManager.Inventory == null) return;
        SlotRayCastList.Clear();

        Transform parentTransform = transform.GetChild(1).GetChild(0);
        
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
 
        if(panels == null || slots == null) return; 
        
        Dictionary<EquipmentType, List<(int, ItemDTO)>> equipmentDictionary2 =InventoryDataGrap2();
        //장착칸 슬롯
        foreach (var type in EquipmentSlots)
        {
            var test = InventoryManager.Inventory.EquipedItemDatas[type.Key];
            var alpha = type.Value;

            if (test.ItemSprite == null)
            {
                alpha.SlotImage.sprite = test.Empty;
                alpha.isFull = false;
                continue;
            } 
            alpha.isFull = true;
            alpha.SlotImage.sprite = test.ItemSprite;
        }

        //인벤칸 슬롯
        foreach (Transform child in UpdateLeft)
        {
            Destroy(child.gameObject);
        }
        foreach (var equipmentType in equipmentDictionary2)
        {
            GameObject panel = Instantiate(panels,UpdateLeft);
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
                send.itemIndex = item.Item1;
                send.Setinfo(equipmentType.Key,item.Item2);
                send.slotIndex = nowSlotCount;
                send.isFull = true;
     
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
    }
    
    
    public void DeRaycastOther(bool type)
    {
        foreach (var image in SlotRayCastList)
        {
            if(image == null) 
            {
                Debug.Log("이미지 컴포넌트 저장 오류:");
                continue;
            }
            image.raycastTarget = type;
        }
    }
    //장비 데이터 그룹화
    public Dictionary<EquipmentType, List<ItemDTO>> InventoryDataGrap()
    {
        Dictionary<EquipmentType, List<ItemDTO>> equipmentDictionary = new();
        var alpha = InventoryManager.Inventory.itemDatas
            .Where(item => item.ItemCategory == ItemType.Equipment);
        
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

    public  Dictionary<EquipmentType, List<(int, ItemDTO)>> InventoryDataGrap2()
    {
        
        Dictionary<EquipmentType, List<(int, ItemDTO)>> equipmentDictionary = new();

        var beta = InventoryManager.Inventory.itemDatas
            .Select((item, index) => new { item, index }) // 파라미터 순서상 인덱스는 값 뒤에옴
            .Where(x => x.item.ItemCategory == ItemType.Equipment);

        foreach (var x in beta)
        {
            if (!equipmentDictionary.ContainsKey(x.item.EquipmentCategory))
            {
                equipmentDictionary[x.item.EquipmentCategory] = new List<(int, ItemDTO)>();
            }
            equipmentDictionary[x.item.EquipmentCategory].Add((x.index, x.item));
        }
        equipmentDictionary = equipmentDictionary
                                .OrderBy(kvp => (int)kvp.Key)
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return equipmentDictionary;
        
    }
}
