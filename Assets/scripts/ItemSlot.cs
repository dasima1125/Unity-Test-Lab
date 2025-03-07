using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour , IPointerClickHandler
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
    public void AddType(string ItemName)
    {

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
            //일단 사용할수있을지 체크를 먼저 해야함
            bool usAble = InventoryManager.Inventory.UseItem(SlotName);
            if(usAble)
            {
                slotQuantity -= 1;
                showQuantityText.text = slotQuantity.ToString();
                if(slotQuantity <= 0) EmptySlot();
            }
        }
        else
        {
            InventoryManager.Inventory.deSelectAll();
            selectedshader.SetActive(true);
            isItemSelect = true;
            showDescriptionName.text = SlotName;
            showDescriptionText.text = slotItemDescription;
        }
    }

    private void EmptySlot()
    {
        showQuantityText.enabled = false;
        isFull = false;
        
        slotSprite = null;
        showSlotImage.sprite = null;

        showDescriptionName.text = "";
        showDescriptionText.text = "";


    }

    public void OnRightclicked()
    {
       
        
    }


}
