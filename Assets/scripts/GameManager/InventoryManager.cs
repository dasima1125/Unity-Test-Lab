using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Inventory
    
    { 
        get => instance ?? (instance = FindObjectOfType<InventoryManager>());
    }
    public ItemSlot[] itemData;
    public ItemSlot[] itemSlots;
    [SerializeField]
    private GameObject itemslot;
    
    public Transform slotPostion;
    public int slotIndex = 10;
    //--Item Descript panel--//
    public TMP_Text DescriptionName_TMP;
    public TMP_Text DescriptionText_TMP;

    public void Updating()
    {
        if(slotPostion == null) 
        {
            Debug.Log("위치 전송 실패");
            return;
        }
        foreach (Transform child in slotPostion.transform)
        {
            Destroy(child.gameObject);
        }
        itemSlots = new ItemSlot[slotIndex];
        for(int i = 0; i < slotIndex; i++) 
        {
            GameObject setSlot = Instantiate(itemslot);
            setSlot.transform.SetParent(slotPostion.transform, false);
            //인수 재정립
            setSlot.GetComponent<ItemSlot>().showDescriptionName = DescriptionName_TMP;
            setSlot.GetComponent<ItemSlot>().showDescriptionText = DescriptionText_TMP;
            
            itemSlots[i] = setSlot.GetComponent<ItemSlot>();
        }

    }

    public int Add(string ItemName, int Quantity, Sprite sprite, string itemDescription)
    {
        for (int i = 0; i <itemSlots.Length;i++)
        {
            if (itemSlots[i].isFull == false && itemSlots[i].SlotName == ItemName || itemSlots[i].slotQuantity == 0)
            {
                int leftoverItme = itemSlots[i].AddItem(ItemName,Quantity,sprite,itemDescription);
                // 0보다 크면 재귀
                if(leftoverItme > 0)
                    leftoverItme = Add(ItemName,leftoverItme,sprite,itemDescription);
                
                return leftoverItme;
            }
        }
        return Quantity;
        
    }
    public void deSelectAll()
    {
        for (int i = 0; i < itemSlots.Length;i++)
        {
            //Debug.Log("초기화중");
            itemSlots[i].selectedshader.SetActive(false);
            itemSlots[i].isItemSelect = false; 
        }
        

    }
    
    
}
