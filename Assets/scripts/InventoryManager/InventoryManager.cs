using System;
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
    //인벤토리 저장기능 만드는중
    public ItemDTO[] itemDatas;
    public Dictionary<EquipmentType,ItemDTO> EquipedItemDatas = new();
    

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
        foreach (EquipmentType type  in  Enum.GetValues(typeof(EquipmentType)))
        {
            if(type != EquipmentType.Null)
                EquipedItemDatas.Add(type,new ItemDTO(null, 0, null, null,NullItemSprite));
        }
        
        //이건 나중에 옴길꺼임
        TestUpdate();
    }
   
    public void Updating()
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
    
    public void DeSelectAll()
    {
        //작동
        for (int i = 0; i < ItemSlot.Count;i++)
        {
            ItemSlot[i].selectedshader.SetActive(false);
            ItemSlot[i].isItemSelect = false; 
        }
    }
    // new Zoon
    // Context Zoon

    public Dictionary<string, GameObject> InventoryUIDictionary = new();
    public Stack<GameObject> InventoryUI = new(); 
    
    public void TestUpdate() 
    {
        GameObject[] InventoryUIs = Resources.LoadAll<GameObject>("InventorySystem/ContextMenuUIs");
        //Debug.Log(InventoryUIs.Length);
        foreach(GameObject InventoryUI in InventoryUIs)
        {
            if(!InventoryUIDictionary.ContainsKey(InventoryUI.name))
            {
                InventoryUIDictionary.Add(InventoryUI.name, InventoryUI);
            }
        }
        
    }
    
    
}
