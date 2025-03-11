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
    public int Quantity; //이건 임시용
    public Sprite slotSprite;
    
    
    //Item Slot//
    public int slotIndex;
    public bool isFull =false;
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    public GameObject selectedshader;
    public bool isItemSelect;
    //Item Description Slot//
    public TMP_Text showDescriptionName;
    public TMP_Text showDescriptionText;

    
   
    
    public void SlotCreating(int Quantity,Sprite Sprite)
    {
        showSlotImage.sprite = Sprite;
        showQuantityText.text = Quantity.ToString();
        showQuantityText.enabled = true;

        this.Quantity = Quantity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            OnLeftclicked();
            
        }
        if(eventData.button == PointerEventData.InputButton.Right) {
            OnRightclicked();
        }
    }
    public void OnLeftclicked()
    {
        if(isItemSelect) 
        {
            bool usAble = InventoryManager.Inventory.UseItem(slotIndex);
            if(usAble)
            {
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

            var data = InventoryManager.Inventory.itemDatas[slotIndex];
        
            showDescriptionName.text = data.ItemName;
            showDescriptionText.text = data.ItemDescription;
        }
    }

    private void EmptySlot()
    {
        showQuantityText.enabled = false;
        isFull = false;
        
        slotSprite = null;
        showSlotImage.sprite = InventoryManager.Inventory.itemDatas[slotIndex].Empty;

    }

    public void OnRightclicked()
    {
        if(isItemSelect)
        {
            if(InventoryManager.Inventory.testItemPrefab == null) return;
            DropItem(slotIndex);
        }
        
    }
    private Vector2 originalPosition;
    private Vector2 originalSizeDelta;
    
    private Transform originalLayer;
    private RectTransform  testtarget;

    
     

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Quantity <= 0) return;
        
        testtarget = transform.Find("ItemImage").GetComponent<RectTransform>();

        originalPosition = testtarget.position;
        originalSizeDelta = testtarget.sizeDelta;
        
        originalLayer = testtarget.parent;
        testtarget.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (testtarget != null)
        testtarget.gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(testtarget == null) return;

        testtarget.position = originalPosition;
        testtarget.sizeDelta = originalSizeDelta;
        testtarget.SetParent(originalLayer);

        
            var swapTarget = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();
            if (swapTarget != null && swapTarget.slotIndex != slotIndex)
            {  
                swapItem(swapTarget.slotIndex);
            }
        
    }
    public void swapItem(int targetIndex)
    {
        var datas =InventoryManager.Inventory;

        var temp = datas.itemDatas[targetIndex];
        
        datas.itemDatas[targetIndex] = datas.itemDatas[slotIndex];
        datas.itemDatas[slotIndex] = temp;
        datas.Updating();


    }
    public void DropItem(int index)
    {
        var target = InventoryManager.Inventory;

        GameObject testItem = Instantiate(target.testItemPrefab);
        testItem.transform.position = GameObject.Find("player").transform.position;
        
        
        var setting = testItem.GetComponent<Items>();
        setting.CanPick = false;
        
        setting.AddInfo(target.itemDatas[index]);
        Debug.Log("아이템 드랍 체크");

    }
}
