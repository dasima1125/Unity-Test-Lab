using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_View : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler ,IPointerClickHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    // Slot Data //
    public int slotIndex;
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    private Sprite EmptySlotImage;
    // Slot State // 
    public GameObject selectedshader;
    public bool isItemSelect;
    
    // Description Connect //
    public TMP_Text showDescriptionNameZone;
    public TMP_Text showDescriptionTextZone;
    //초기화 구획

    void Start()
    {
        EmptySlotImage = showSlotImage.sprite;// 이미지로 보내버리면 참조를 걸어서 문제가 생김
        showDescriptionNameZone = Inventory_ViewModel.Inventory.DescriptionName_TMP;
        showDescriptionTextZone = Inventory_ViewModel.Inventory.DescriptionText_TMP;
        SlotUpdate(slotIndex);
        
        
    }
    public void SlotUpdate(int index)
    {
        var ItemData = DataManager.data.InventoryList[index];
        if (ItemData.Quantity <= 0)// 수량이 0일때
        {
            EmptySlot();
            return;
        } 
        ItemData_SO GetItemData = Inventory_ViewModel.Inventory.UpadateData(ItemData.ID);
        if(GetItemData == null)// ID를 찾을수 없을때 
        {
            Debug.LogWarning("잘못된 인수 선언 : 인벤토리 데이터를 초기화합니다.");
            EmptySlot();
            return;
        }
        showSlotImage.sprite     = GetItemData.Sprite;
        showQuantityText.text    = ItemData.Quantity.ToString();
 
        showQuantityText.enabled = true;

        
    }
    private void EmptySlot()
    {
        showQuantityText.text    = "0";
        showQuantityText.enabled = false;
        showSlotImage.sprite     = EmptySlotImage;
    }
    public void DeSelect()
    {
        isItemSelect = false;
        selectedshader.SetActive(false);
    }

    // 마우스 커서 위치 서비스
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isItemSelect) 
        selectedshader?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isItemSelect)
        selectedshader?.SetActive(false);
    }

    
    // 마우스 클릭 서비스
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            OnLeftclicked();
            
        }
        if(eventData.button == PointerEventData.InputButton.Right) {
            //OnRightclicked();
        }
        if(eventData.button == PointerEventData.InputButton.Middle) {
            //TestClicked();
        }
    }
    public void OnLeftclicked()
    {
        if(isItemSelect) 
        {
        }
        else
        {
            Inventory_ViewModel.Inventory.DeSelectAll();

            selectedshader.SetActive(true);
            isItemSelect = true;

            var ItemData = DataManager.data.InventoryList[slotIndex];
            if (ItemData.ID == 0) return;
        
            var data = Inventory_ViewModel.Inventory.UpadateData(ItemData.ID);
            showDescriptionNameZone.text = data.ItemName;
            showDescriptionTextZone.text = data.ItemDescription;
        }

    }

    // 드래그 앤 드랍 서비스
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
        var swapTarget = eventData.pointerCurrentRaycast.gameObject.GetComponent<Inventory_View>();
        if(swapTarget != null && swapTarget.slotIndex != slotIndex)
        {
            Inventory_ViewModel.Inventory.SwapItemData(slotIndex, swapTarget.slotIndex);
        }
       
    }


}
