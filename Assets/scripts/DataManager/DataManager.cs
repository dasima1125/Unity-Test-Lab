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
        //InventoryList.Add(new InventoryItem(0, 0));
        //InventoryList.Add(new InventoryItem(0, 0));
        //InventoryList.Add(new InventoryItem(0, 0));
    }
    public Dictionary<int,ItemData_SO> ItemData = new();
    /// <summary>
    /// ID , 수량 순
    /// </summary>
    public List<InventoryItem> InventoryList = new();
    

}
