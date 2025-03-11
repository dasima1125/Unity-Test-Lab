using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotHandler : MonoBehaviour, IPointerClickHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    //입력과 출력 담당

    private InventoryManager inventory;
    private ItemSlotController controller;
    // Slot Data //
    public int slotIndex;
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    // Slot State // 
    public GameObject selectedshader;
    public bool isItemSelect;

    // Description Connect //
    public TMP_Text showDescriptionNameZone;
    public TMP_Text showDescriptionTextZone;

    void Start()
    {
        inventory  = InventoryManager.Inventory;
        controller = ItemSlotController.controll;
        showDescriptionNameZone = inventory.DescriptionName_TMP;
        showDescriptionTextZone = inventory.DescriptionText_TMP;
        SlotUpdate(slotIndex);
    }

    public void SlotUpdate(int index)
    {
        var ItemData = inventory.itemDatas[index];
        if (ItemData.ItemQuantity <= 0)
        {
            EmptySlot();
            return;
        } 
        showSlotImage.sprite = ItemData.ItemSprite;
        showQuantityText.text = ItemData.ItemQuantity.ToString();
        showQuantityText.enabled = true;
    }

    #region 드래그 구현구획

    //== 입력 부분 ==//
    // Drag  Connect //
    private Vector2 originalPosition , originalSize;
    private Transform originalLayer;
    private RectTransform MoveTarget;
    public void OnBeginDrag(PointerEventData eventData)
    {
        MoveTarget        =  transform.Find("ItemImage").GetComponent<RectTransform>();
        originalPosition  =  MoveTarget.position;
        originalSize      =  MoveTarget.sizeDelta;

        originalLayer = MoveTarget.parent;

        MoveTarget.SetParent(MoveTarget.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MoveTarget == null) return;
        
        MoveTarget.gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(MoveTarget == null) return;

        MoveTarget.position  = originalPosition;
        MoveTarget.sizeDelta =     originalSize;
        MoveTarget.SetParent(originalLayer);

        //작동구조 위치
        
    }
    #endregion

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
            bool usAble = inventory.UseItem(slotIndex);
            if(usAble)
            {
                Debug.Log("사용");
                SlotUpdate(slotIndex);
            }
        }
        else
        {
            //Description show
            InventoryManager.Inventory.DeSelectAll();

            selectedshader.SetActive(true);
            isItemSelect = true;

            var data = inventory.itemDatas[slotIndex];
            showDescriptionNameZone.text = data.ItemName;
            showDescriptionTextZone.text = data.ItemDescription;
        }

    }
    public void OnRightclicked()
    {
        if(isItemSelect)
        {
            bool usAble = controller.DropItem(slotIndex);
            if(usAble)
            {
                Debug.Log("드랍");
                SlotUpdate(slotIndex);
            }
        }

    }
    //== 출력 부분 ==//
    public void EmptySlot()
    {
        showQuantityText.enabled = false;
        showSlotImage.sprite = InventoryManager.Inventory.itemDatas[slotIndex].Empty;
    }
    
}
