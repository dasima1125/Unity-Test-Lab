using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour , IPointerClickHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    //Item Data//
    public string SlotName;
    public int slotQuantity;
    public Sprite slotSprite;
    public bool isFull =false;
    public string slotItemDescription;
    [SerializeField] private int maxNumberItems = 9;
    //Item Slot//
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    //Item Description Slot//
    public TMP_Text showDescriptionName;
    public TMP_Text showDescriptionText;

    public GameObject selectedshader;
    public bool isItemSelect;
    //type 2

    [HideInInspector]
    public int slotIndex;
    public int Quantity;
    public Sprite Sprite;


    public int AddItem(string ItemName, int ItemQuantity, Sprite sprite,string itemDescription)
    {
        //Debug.Log(ItemName + " " + ItemQuantity + " " + sprite.name);
        if(isFull)
            return ItemQuantity;

        SlotName = ItemName;
        
        slotSprite = sprite;
        showSlotImage.sprite = slotSprite;

        slotItemDescription = itemDescription;
        
        slotQuantity += ItemQuantity;
        if(slotQuantity >= maxNumberItems)
        {
            Debug.Log("초과");
            //최대 수량으로 지정하고 가득 찬상태 갱신
            showQuantityText.text = maxNumberItems.ToString();
            showQuantityText.enabled = true;
            isFull = true;

            //남은수량 반환
            int extraItems = slotQuantity - maxNumberItems;
            slotQuantity = maxNumberItems;
            Debug.Log("초과된 수량 : " + extraItems);
            return extraItems;
        }
        
        showQuantityText.text = slotQuantity.ToString();
        showQuantityText.enabled = true;

        return 0;
        
    }
    public void SlotCreating(int Quantity,Sprite Sprite)
    {
        showSlotImage.sprite = Sprite;
        showQuantityText.text = Quantity.ToString();
        showQuantityText.enabled = true;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            //OnLeftclicked();
            OnLeftclicked2();
            
        }
        if(eventData.button == PointerEventData.InputButton.Right) {
            OnRightclicked();
        }
    }
    public void OnLeftclicked()
    {
        if(isItemSelect) 
        {
            //일단 사용할수있을지 체크를 먼저 해야함
            Debug.Log("체크중");
            bool usAble = InventoryManager.Inventory.UseItem2(slotIndex);
            if(usAble)
            {
                Debug.Log("체크");
                slotQuantity -= 1;
                showQuantityText.text = slotQuantity.ToString();
                if(slotQuantity <= 0) EmptySlot();
            }
        }
        else
        {   
            Debug.Log(slotIndex); 
            InventoryManager.Inventory.deSelectAll();
            InventoryManager.Inventory.DeSelectAll();
            selectedshader.SetActive(true);
            isItemSelect = true;
            showDescriptionName.text = SlotName;
            showDescriptionText.text = slotItemDescription;
        }
    }
    public void OnLeftclicked2()
    {
        if(isItemSelect) 
        {
            //일단 사용할수있을지 체크를 먼저 해야함
            Debug.Log("체크중");
       
            bool usAble = InventoryManager.Inventory.UseItem2(slotIndex);
            if(usAble)
            {
                Debug.Log("체크");
                var data = InventoryManager.Inventory.itemDatas[slotIndex];
                data.ItemQuantity -= 1;
                showQuantityText.text = data.ItemQuantity.ToString();
                if(data.ItemQuantity <= 0) EmptySlot();
            }
        }
        else
        {   
           
            InventoryManager.Inventory.DeSelectAll();
            selectedshader.SetActive(true);
            isItemSelect = true;
            showDescriptionName.text = SlotName;
            showDescriptionText.text = slotItemDescription;
        }
    }

    private void EmptySlot()
    {
        Debug.Log("비었음");
        showQuantityText.enabled = false;
        isFull = false;
        
        slotSprite = null;
        showSlotImage.sprite = InventoryManager.Inventory.itemDatas[slotIndex].Empty;

        showDescriptionName.text = "";
        showDescriptionText.text = "";


    }

    public void OnRightclicked()
    {
       
        
    }
    public bool Swaped = false;
    private Vector2 originalPostion;
    private Transform originalLayer;
     

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPostion = transform.Find("ItemImage").gameObject.transform.position;
        originalLayer = transform.Find("ItemImage");
        transform.Find("ItemImage").SetParent(transform.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.Find("ItemImage").gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!Swaped)
        transform.Find("ItemImage").gameObject.transform.position = originalPostion;
    }
}
