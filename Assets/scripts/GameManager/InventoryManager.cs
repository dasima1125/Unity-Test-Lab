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
    public ItemSlot[] itemSlots;
    public List<ItemSlot> ItemSlots = new();
    public ItemSO[] itemSOs;

    [SerializeField]
    private GameObject itemslot;
    
    public Transform slotPostion;
    public int slotIndex = 3;
    //--Item Descript panel--//
    public TMP_Text DescriptionName_TMP;
    public TMP_Text DescriptionText_TMP;

    public Sprite NullItemSprite;
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
    public void Updating2()
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
                Debug.Log("수량 : " + itemDatas[i].ItemQuantity);
                target.SlotCreating(itemDatas[i].ItemQuantity,itemDatas[i].ItemSprite);
            }

            ItemSlots.Add(setSlot.GetComponent<ItemSlot>());
        }
    }
    public bool UseItem(string itemName)
    {
        if (itemName == "") return false;
        for(int i = 0; i < itemSOs.Length; i++) 
        {
            if(itemSOs[i].itemName == itemName)
            {
                Debug.Log("부울문 확인");
                bool usAble = itemSOs[i].UseItem();
                return usAble;
            }
        }
        return false;
    }
    public bool UseItem2(int index)
    {
        if (itemDatas[index].ItemName == "") return false;
        for(int i = 0; i < itemSOs.Length; i++) 
        {
            if(itemSOs[i].itemName == itemDatas[index].ItemName)
            {
                Debug.Log("부울문 확인");
                bool usAble = itemSOs[i].UseItem();
                return usAble;
            }
        }
        return false;
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
    public int Add_ver2(string ItemName, int Quantity, Sprite sprite, string itemDescription)
    {
        //Debug.Log("습득");
        for(int i = 0; i < itemDatas.Length; i++) 
        {
            if(itemDatas[i].IsFull == false && itemDatas[i].ItemName == ItemName || itemDatas[i].ItemQuantity == 0)
            {
                int leftoverItme = itemDatas[i].AddItem(ItemName, Quantity, sprite,itemDescription);
                if(leftoverItme > 0)
                    leftoverItme = Add_ver2(ItemName,leftoverItme,sprite,itemDescription);

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
    public void DeSelectAll()
    {
        for (int i = 0; i < ItemSlots.Count;i++)
        {
            ItemSlots[i].selectedshader.SetActive(false);
            ItemSlots[i].isItemSelect = false; 
        }
    }
    
    
}
