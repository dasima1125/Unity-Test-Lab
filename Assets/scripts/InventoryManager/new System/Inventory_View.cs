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
        SlotUpdate();
        
        
    }
    // 슬롯 상태 서비스
    public void SlotUpdate()
    {
        var ItemData = DataManager.data.InventoryList[slotIndex];
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
    public void OnRightclicked()
    {
        if(isItemSelect && DataManager.data.InventoryList[slotIndex].ID != 0)
        CreateContext();
    }
    public void CreateContext() 
    {
        var Manager = Inventory_ViewModel.Inventory;
        string target ="ContextPanelUI";

        if(!Manager.ContextUIDictionary.ContainsKey(target)) return;
        GameObject contextPanel = Instantiate(Manager.ContextUIDictionary[target]);
        
        contextPanel.transform.SetParent(UImanager.manager.canvas.transform, false);
        Manager.ContextUI.Push(contextPanel);
        contextPanel.SetActive(false);

        //인덱스 부여
        contextPanel.GetComponent<Inventory_View_Context>().slotIndex = slotIndex;  
        
        RectTransform slotPos    = gameObject.GetComponent<RectTransform>();                            // 슬롯 위치
        RectTransform buttonRect = contextPanel.transform.GetChild(0).GetComponent<RectTransform>();    //버튼위치 조정
        
        buttonRect.position = slotPos.position - new Vector3((buttonRect.rect.width / 2) -5,buttonRect.rect.height + 35); //초기 설계를 잘못했나..  세부조정이 좀
        //각버튼 활성화 조건
        Dictionary<string,GameObject> childrenPanel = new();
        for (int i = 0; i < contextPanel.transform.GetChild(0).childCount; i++)
        {
            var ContextPanle = contextPanel.transform.GetChild(0).GetChild(i);
            childrenPanel.Add(ContextPanle.name,ContextPanle.gameObject);
        }

        ItemData_SO SlotItemData = Inventory_ViewModel.Inventory.GetItemDatabyIndex(slotIndex);
        
        IfDisableButton(SlotItemData.ItemType != ItemTypeEnums.Equipment       ,"Equip_Panel"  ,childrenPanel);
        IfDisableButton(SlotItemData.ItemType != ItemTypeEnums.Consumable      ,"Use_Panel"    ,childrenPanel);
        IfDisableButton(DataManager.data.InventoryList[slotIndex].Quantity < 2 ,"Split_Panel"  ,childrenPanel);
        
                
        contextPanel.SetActive(true);

    }
    private void IfDisableButton(bool condition, string panelName, Dictionary<string, GameObject> childrenPanel)
    {
        if (!condition) return; // 조건이 false면 그대로 둠
        if (childrenPanel.TryGetValue(panelName, out var lockdownTarget))
        {
            lockdownTarget.GetComponent<Image>().color =Color.black;
            lockdownTarget.GetComponent<Button>().interactable = false;
            lockdownTarget.transform.GetChild(0).GetComponent<TMP_Text>().alpha = 0.2f;
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
