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

    //Item Slot//
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    //Item Description Slot//
    public TMP_Text showDescriptionName;
    public TMP_Text showDescriptionText;

    public GameObject selectedshader;
    public bool isItemSelect;


    public void AddItem(string ItemName, int ItemQuantity, Sprite sprite,string itemDescription)
    {
        //Debug.Log(ItemName + " " + ItemQuantity + " " + sprite.name);
        
        this.SlotName = ItemName;
        this.slotQuantity = ItemQuantity;
        this.slotSprite = sprite;
        this.slotItemDescription = itemDescription;
        isFull = true;
       
        //showQuantityText.text = "04";
        //showQuantityText.enabled = true;
        
        //showSlotImage.sprite = slotSprite;
        
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
        InventoryManager.Inventory.deSelectAll();
        selectedshader.SetActive(true);
        isItemSelect = true;
        showDescriptionName.text = SlotName;
        showDescriptionText.text = slotItemDescription;
        
    }
    public void OnRightclicked()
    {
       
        
    }


}
