using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager data
    { 
        get => instance ?? (instance = FindObjectOfType<DataManager>()); 
    }
    private void Start() {
        /**
        var data = Resources.LoadAll<ItemData_SO>("itemData");

        foreach (var item in data)
        {
            //Debug.Log("Item ID: " + item.ItemID + ", Item Name: " + item.ItemName);
            if (!ItemData.ContainsKey(item.ItemID)) ItemData.Add(item.ItemID, item);
            else
            {
                Debug.LogWarning("중복된 아이디 발견, 대상 :  " + item.ItemName  + " / " +ItemData[item.ItemID].ItemName);
            }
        }
        int maximumSize = 20;

        if (InventoryList.Count < maximumSize)
        {
            while (InventoryList.Count < maximumSize)
            {
                InventoryList.Add(new InventoryItem(0, 0)); 
            }
        }
        */
        
        
    }
    /// <summary>
    /// ID , 수량 순
    /// </summary>
    /// 아이템 데이터
    /// 나중에 이부분들음 .. 음 아마 모듈이있는지 확인하고 생성한다음 데이터를 끌어오는식? 이런식으로 개선해봐야할듯
    //public Dictionary<int,ItemData_SO> ItemData = new();
    
    // 인벤토리 데이터
    //public List<InventoryItem> InventoryList = new();
    
    // 장비칸 데이터
    public Dictionary<EquipmentTypeEnums,int> EquipedDatas = new();
    
    

}
