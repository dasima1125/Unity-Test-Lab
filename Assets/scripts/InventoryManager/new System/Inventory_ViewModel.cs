using System.Collections;
using System.Collections.Generic;
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

    //인벤토리 내부요소
    [SerializeField] 
    private GameObject Itemslot;
    public List<Inventory_View> ItemSlots = new();
    public Transform slotPostion;

    public void InventoryOpen(GameObject gameObject)
    {
        if(InventoryPanel != null) return;
        
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

        for(int i = 0; i < DataManager.data.InventoryList.Count; i++) 
        {
            GameObject Slot = Instantiate(Itemslot);
            Slot.transform.SetParent(slotPostion.transform, false);

            Slot.GetComponent<Inventory_View>().slotIndex = i;
  
            ItemSlots.Add(Slot.GetComponent<Inventory_View>());
        } 
    }
    public ItemData_SO UpadateData(int ID)
    {
        ItemData_SO data = Inventory_Model.Inventory.ItemDataReader(ID);
        return data;
    }
    public void DeSelectAll()
    {
        foreach (var item in ItemSlots)
        {
            item.DeSelect();
        }
    }
    public int ItemAdd(int ID ,int Quantity)
    {
        int itemCount = Inventory_Model.Inventory.AddItem(ID,Quantity);
        return itemCount;
    }
    public void SwapItemData(int target1 ,int target2)
    {
        Inventory_Model.Inventory.SwapItemData(target1,target2);
        ItemSlots[target1].SlotUpdate(target1);
        ItemSlots[target2].SlotUpdate(target2);
    }
}
