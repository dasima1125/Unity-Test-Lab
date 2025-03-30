using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    public EquipmentType EquipedSlotType;
    public bool isFull;
    [SerializeField]public bool EquipedSlot;

    public GameObject selectedshader;
    public int slotIndex;
    //Slot Show//
    public Image SlotImage;

    //Slot Data//
    public int itemIndex;
    
    void Awake()
    {
        if(SlotImage == null) SlotImage = transform.Find("ItemImage").GetComponent<Image>();
    }
    void Start() 
    {
        selectedshader = transform.transform.GetChild(0).gameObject;
        SlotUpdate(slotIndex);
    }

    public void Setinfo(EquipmentType Type,ItemDTO itemDTO)
    {
        EquipedSlotType = Type;
        if(itemDTO != null) 
        {
            isFull = true;
            if(SlotImage != null) SlotImage.sprite = itemDTO.ItemSprite;
        }

       
    }
    public void SlotUpdate(int index)
    {
        /**
        if(inventory.itemDatas == null) return;
        var ItemData = inventory.itemDatas[index];
        if (ItemData.ItemQuantity <= 0)
        {
            EmptySlot();
            return;
        } 
        showSlotImage.sprite = ItemData.ItemSprite;
        showQuantityText.text = ItemData.ItemQuantity.ToString();
        showQuantityText.enabled = true;
        */
    }

    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(!isFull) return;
        selectedshader?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectedshader?.SetActive(false);
    }
    
    //
    // Drag  Connect //
    //

    private Vector2 originalPosition , originalSize;
    private Transform originalLayer;
    private RectTransform MoveTarget;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isFull) return;
        
        MoveTarget = transform.Find("ItemImage").GetComponent<RectTransform>();
        var manager = EquipmentManager.Equipment;
        manager.DeRaycastOther(false);

        originalPosition  =  MoveTarget.position;//포지션을 좀 바꿔야할듯 아니면 옴길때 리스트패널을 고정?
        originalSize      =  MoveTarget.sizeDelta;
        originalLayer = MoveTarget.parent;

        MoveTarget.SetParent(MoveTarget.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MoveTarget == null) return;
        MoveTarget.gameObject.transform.position = Input.mousePosition;
        
        var swapTarget = eventData.pointerCurrentRaycast;
        if (swapTarget.gameObject == null) return;

        if(!EquipedSlot && swapTarget.gameObject.name == "LoadOutZoon")    
            EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].selectedshader.SetActive(true);
        else
            EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].selectedshader.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(MoveTarget == null) return;
        
        var manager = EquipmentManager.Equipment;
        manager.DeRaycastOther(true);

        MoveTarget.position  = originalPosition;
        MoveTarget.sizeDelta =     originalSize;
        MoveTarget.SetParent(originalLayer);
       
        var SelectTarget = eventData.pointerCurrentRaycast;
        if (SelectTarget.gameObject == null) return;
        //장착 로직
        if(!EquipedSlot && SelectTarget.gameObject.name == "LoadOutZoon")
        { 
            //교체로직 
            var SwapTarget = EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType];
            SwapTarget.selectedshader.SetActive(false);

            if(SwapTarget.isFull)
            {
                Debug.Log("아이템 교체");
                var data1 = InventoryManager.Inventory.itemDatas[itemIndex].CopyItemDTO();
                var data2 = InventoryManager.Inventory.EquipedItemDatas[EquipedSlotType].CopyItemDTO();
                InventoryManager.Inventory.itemDatas[itemIndex] = data2;
                InventoryManager.Inventory.EquipedItemDatas[EquipedSlotType] = data1;
            }
            else
            {   
                Debug.Log("아이템 장착");
                //대상정보 복사
                var data = InventoryManager.Inventory.itemDatas[itemIndex];
                InventoryManager.Inventory.EquipedItemDatas[EquipedSlotType] = data.CopyItemDTO();
                data.ResetSlot();

                //SwapTarget.isFull = true;
            }
            EquipmentManager.Equipment.EquipmentUpdate();
        }
        //해체 로직
        else if(EquipedSlot && SelectTarget.gameObject.name != "LoadOutZoon")
        {
            var target = InventoryManager.Inventory.EquipedItemDatas[EquipedSlotType];
            var data = InventoryManager.Inventory.itemDatas;
            //아이템 추가로직 요구
            if(target.ItemName == string.Empty)
            {
                Debug.Log("빈슬롯");
                return;
            } 
            for(int i = 0; i < data.Length; i++) 
            {
                if(data[i].IsFull == false && data[i].ItemName == target.ItemName || data[i].ItemQuantity == 0)
                {
                    data[i] = target.CopyItemDTO();
                    target.ResetSlot();

                    break;
                }
            }
            Debug.Log("해체");
            EquipmentManager.Equipment.EquipmentUpdate();
        }
        
        
    }
    
}
