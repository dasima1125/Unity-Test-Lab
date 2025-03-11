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
    public ItemDTO[] itemDatas;
    public List<ItemSlot> ItemSlots = new();
    public List<ItemSlotHandler> ItemSlot = new();
    public ItemSO[] itemSOs;

    [SerializeField]
    private GameObject itemslot;
    
    public Transform slotPostion;
    public int slotIndex;
    //--Item Descript panel--//
    public TMP_Text DescriptionName_TMP;
    public TMP_Text DescriptionText_TMP;

    public Sprite NullItemSprite;
    public GameObject testItemPrefab;
    void Start()
    {
        itemDatas = new ItemDTO[slotIndex];
        for(int i = 0; i < itemDatas.Length; i++) 
        {
            itemDatas[i] = new ItemDTO(null, 0, null, null,NullItemSprite);
        }
    }

    public void Updating()
    {
        if(slotPostion == null) return;
        foreach (Transform child in slotPostion.transform)
        {
            Destroy(child.gameObject);
        }
        ItemSlots.Clear();
        for(int i = 0; i < itemDatas.Length; i++) 
        {
            GameObject setSlot = Instantiate(itemslot);
            setSlot.transform.SetParent(slotPostion.transform, false);

            var target = setSlot.GetComponent<ItemSlot>();

            target.showDescriptionName = DescriptionName_TMP;
            target.showDescriptionText = DescriptionText_TMP;
            target.slotIndex = i;

            
            if(itemDatas[i].ItemQuantity > 0)
            {
                target.SlotCreating(itemDatas[i].ItemQuantity,itemDatas[i].ItemSprite);
            }

            ItemSlots.Add(setSlot.GetComponent<ItemSlot>());
        }
        
    }
    public void Updating2()
    {
        if(slotPostion == null) return;
        foreach (Transform child in slotPostion.transform)
        {
            Destroy(child.gameObject);
        }
        ItemSlot.Clear();
        for(int i = 0; i < itemDatas.Length; i++) 
        {
            GameObject setSlot = Instantiate(itemslot);
            setSlot.transform.SetParent(slotPostion.transform, false);

            setSlot.GetComponent<ItemSlotHandler>().slotIndex = i;
  
            ItemSlot.Add(setSlot.GetComponent<ItemSlotHandler>());
        }
        Debug.Log("" + ItemSlot.Count);

    }
   
    public bool UseItem(int index)
    {
        if (itemDatas[index].ItemName == "") return false;
        for(int i = 0; i < itemSOs.Length; i++) 
        {
            if(itemSOs[i].itemName == itemDatas[index].ItemName)
            {
                bool usAble = itemSOs[i].UseItem();
                return usAble;
            }
        }
        return false;
    }
    
    public int Add_ver(string ItemName, int Quantity, Sprite sprite, string itemDescription)
    {
   
        for(int i = 0; i < itemDatas.Length; i++) 
        {
            if(itemDatas[i].IsFull == false && itemDatas[i].ItemName == ItemName || itemDatas[i].ItemQuantity == 0)
            {
                //Debug.Log(i + " 번째 배열 기입");
                int leftoverItme = itemDatas[i].AddItem(ItemName, Quantity, sprite,itemDescription);
                if(leftoverItme > 0)
                    leftoverItme = Add_ver(ItemName,leftoverItme,sprite,itemDescription);
                if(leftoverItme <= 0 && ItemSlots != null)
                    Updating();
                
                return leftoverItme;
            }
            
        }
        return Quantity;

    }
    public void DeSelectAll()
    {
        //작동
        Debug.Log("jhgkj");
        for (int i = 0; i < ItemSlot.Count;i++)
        {
            ItemSlot[i].selectedshader.SetActive(false);
            ItemSlot[i].isItemSelect = false; 
        }
    }
    // new Zoon
    
    
}
