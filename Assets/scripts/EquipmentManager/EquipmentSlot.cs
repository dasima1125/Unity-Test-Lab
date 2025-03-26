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
    private EquipmentType InvetoryslotType;
    public int slotIndex;
    //Slot Show//
    public Image SlotImage;

    //Slot Data//
    
    
    void Start() 
    {
        selectedshader = transform.transform.GetChild(0).gameObject;
        if(SlotImage == null) SlotImage = transform.Find("ItemImage").GetComponent<Image>();
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
       
        var swapTarget = eventData.pointerCurrentRaycast;
        if (swapTarget.gameObject == null) return;

        if(!EquipedSlot && swapTarget.gameObject.name == "LoadOutZoon")
        { 
            EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].selectedshader.SetActive(false);

            if(EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].isFull)
            Debug.Log("아이템 교체");
            else
            {
                Debug.Log("아이템 장착");
                if(EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].SlotImage == null)
                {
                    Debug.Log("이미지 설정 실패");
                    return;
                }
                EquipmentManager.Equipment.EquipmentSlots[EquipedSlotType].SlotImage.sprite = SlotImage.sprite;

            }
            

        }
        else if(EquipedSlot && swapTarget.gameObject.name != "LoadOutZoon")
        Debug.Log("아이템 해제");
        
    }
    
}
