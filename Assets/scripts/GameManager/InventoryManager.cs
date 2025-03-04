using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Inventory
    
    { 
        get => instance ?? (instance = FindObjectOfType<InventoryManager>());
    }
    public ItemSlot[] itemSlots;
    [SerializeField]
    private GameObject itemslot;
    public Transform posSlot;
    public int slotIndex = 10;
    public void Updating()
    {
        Debug.Log("진행중");
        if(posSlot == null) 
        {
            Debug.Log("위치 전송 실패");
            return;
        }
        for(int i = 0; i < slotIndex; i++) 
        {
            GameObject setSlot = Instantiate(itemslot);
            setSlot.transform.SetParent(posSlot.transform, false);

            //인수 재정립
        }

    }

    public void Add(string ItemName, int ItemQuantity, Sprite sprite, string itemDescription)
    {
        for (int i = 0; i <itemSlots.Length;i++)
        {
            if (itemSlots[i].isFull == false)
            {
                Debug.Log("삽입");
                itemSlots[i].AddItem(ItemName,ItemQuantity,sprite,itemDescription);
                return;
            }
        }
        
    }
    public void deSelectAll()
    {
        for (int i = 0; i <itemSlots.Length;i++)
        {
            //Debug.Log("초기화중");
            itemSlots[i].selectedshader.SetActive(false);
            itemSlots[i].isItemSelect = false; 
        }
        

    }
    
    
}
