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

        var swapTarget = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotHandler>();
        if (swapTarget != null && swapTarget.slotIndex != slotIndex)
        {
            controller.SwapItem(slotIndex,swapTarget.slotIndex);
            //이방식은 지양됨 뷰에서 뷰는 안되고 뷰에서 뷰모델에 명령을 보내고 뷰모델에서 명령을뿌려야함
            inventory.ItemSlot[swapTarget.slotIndex].SlotUpdate(swapTarget.slotIndex);
            SlotUpdate(slotIndex);
            // 잠시끌고옴

            //하이라이트 함수로 사용하는게맞는거같음
            InventoryManager.Inventory.DeSelectAll();

            inventory.ItemSlot[swapTarget.slotIndex].selectedshader.SetActive(true);
            inventory.ItemSlot[swapTarget.slotIndex].isItemSelect = true;

            var data = inventory.itemDatas[swapTarget.slotIndex];
            showDescriptionNameZone.text = data.ItemName;
            showDescriptionTextZone.text = data.ItemDescription;
        }
    }
    #endregion

    #region 클릭 구획

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            OnLeftclicked();
            
        }
        if(eventData.button == PointerEventData.InputButton.Right) {
            OnRightclicked();
        }
        if(eventData.button == PointerEventData.InputButton.Middle) {
            //TestClicked();
        }
        
    }

    public void OnLeftclicked()
    {
        if(isItemSelect) 
        {
            bool usAble = controller.UseItem(slotIndex);
            if(usAble)
            {
                SlotUpdate(slotIndex);
            }
        }
        else
        {
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
        if(isItemSelect && inventory.itemDatas[slotIndex].ItemQuantity > 0)
        {
            CreateContext();
        }

    }
    public void TestClicked()
    {
        /**
        controller.SortItemSlot();
        for(int i = 0; i < inventory.ItemSlot.Count; i++) 
        {
            inventory.ItemSlot[i].SlotUpdate(i);
        }
        */
        CreateContext();
    }
    //== 출력 부분 ==//
    public void EmptySlot()
    {
        showQuantityText.enabled = false;
        showSlotImage.sprite = InventoryManager.Inventory.itemDatas[slotIndex].Empty;
    }

    #endregion

    #region 컨텍스트 호출
    public void CreateContext(string alpha = null)
    {
        string a ="ContextPanelUI";
        if(!inventory.InventoryUIDictionary.ContainsKey(a)) return;
         
        GameObject contextPanel = Instantiate(inventory.InventoryUIDictionary[a]);
        contextPanel.transform.SetParent(UImanager.manager.canvas.transform, false);
        contextPanel.GetComponent<InventoryContextHandler>().slotIndex = slotIndex;
        
        RectTransform slotPos    = gameObject.GetComponent<RectTransform>();// 슬롯 위치
        RectTransform buttonRect = contextPanel.transform.GetChild(0).GetComponent<RectTransform>();//버튼위치 조정
        
        buttonRect.position = slotPos.position - new Vector3((buttonRect.rect.width / 2) -5,(buttonRect.rect.height / 2) - 13); //초기 설계를 잘못했나..  세부조정이 좀
        //맘에 안들긴하는데 피벗 관련 나중에 해결 요망
        inventory.InventoryUI.Push(contextPanel);
        
    }
    #endregion
    
}
