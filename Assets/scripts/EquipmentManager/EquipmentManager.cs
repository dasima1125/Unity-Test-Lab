using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static EquipmentManager instance;
    public static EquipmentManager Equipmen
    { 
        get => instance ?? (instance = FindObjectOfType<EquipmentManager>());
    }
    [SerializeField] private GameObject slots;
    [SerializeField] private Transform UpdateRight;
    [SerializeField] private Transform UpdateLeft;
    

    Dictionary<EquipmentType,EquipmentSlot> EquipmentSlots = new();
    List<Image> SlotRayCastList = new();
    void Start()
    {
        EquipmentUpdate();
        Transform parentTransform = transform.GetChild(1).GetChild(0);

        foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
        {
            EquipmentSlots[equipmentType] = new EquipmentSlot();
        }
        foreach (Transform child in parentTransform)
        {
            var target = child.GetComponent<EquipmentSlot>();
            
            if (target != null && !EquipmentSlots.ContainsKey(target.EquipedSlotType))
            EquipmentSlots.Add(target.EquipedSlotType,target);
            
            Image imageComponent = child.GetComponent<Image>();
            if (imageComponent != null)
            {
                SlotRayCastList.Add(imageComponent);
            }
        } 
    }
    public void EquipmentUpdate()
    {
        int [] testSlots = {4,5,3,2,1};
        string [] testSlotsName = {"헬맷","상의","하의","신발","도구"};
        
        int testint = testSlots.Length;
        if(UpdateLeft == null || slots == null || UpdateLeft.childCount == 0) return; 
        GameObject LeftInventoryPanel = UpdateLeft.GetChild(0).gameObject;
       
        for(int i = 0; i < testint; i++) 
        {
            GameObject panel = Instantiate(LeftInventoryPanel,UpdateLeft);
            panel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = testSlotsName[i];
            for(int j = 0; j < testSlots[i] + 5; j++) 
            {
                GameObject slot = Instantiate(slots);
                slot.transform.SetParent(panel.transform.GetChild(1), false);
                
                SlotRayCastList.Add(slot.GetComponent<Image>());// 슬롯 선택 상태 기입
                //정보 기입
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
    public void EquipedSlot()
    {
        
    }


}
