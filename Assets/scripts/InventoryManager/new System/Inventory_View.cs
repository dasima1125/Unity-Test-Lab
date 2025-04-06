using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_View : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler ,IPointerClickHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    // Slot Data //
    public int SlotIndex;
    [SerializeField] private TMP_Text showQuantityText;
    [SerializeField] private Image showSlotImage;
    [SerializeField] private Sprite EmptySlotImage;
    // Slot State // 
    public GameObject selectedshader;
    public bool isItemSelect;
    
    // Description Connect //
    //public TMP_Text showDescriptionNameZone;
    //public TMP_Text showDescriptionTextZone;2
    //초기화 구획

    private Inventory_ViewModel Order;
    public void Init(Inventory_ViewModel Order , int SlotIndex)
    {
        this.Order = Order;
        this.SlotIndex = SlotIndex;
        SlotUpdate();
        
    }
    // 슬롯 상태 서비스
    public void SlotUpdate()
    {
        var IndexInfo = Order.GetSlotDatabyIndex(SlotIndex);
        if(IndexInfo == null) 
        {
            Debug.Log("데이터 스토리지 접근 실패 : 인덱스에 해당되는 공간이없습니다");
            return;
        }
        if (IndexInfo.ID == 0 || IndexInfo.Quantity == 0)// 수량이 0일때
        {
         
            EmptySlot();
            return;
        }
        var ItemInfo = Order.GetItemDatabyID(IndexInfo.ID);
        showSlotImage.sprite     = ItemInfo.Sprite;
        showQuantityText.text    = IndexInfo.Quantity.ToString();

        showQuantityText.enabled = true;

    }
    private void EmptySlot()
    {
        showQuantityText.text    = "0";
        showQuantityText.enabled = false;
        showSlotImage.sprite     = EmptySlotImage;
    }
    public void Select(bool type)
    {
        isItemSelect = type;
        selectedshader.SetActive(type);
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
        if(eventData.button == PointerEventData.InputButton.Left) OnLeftclicked();
        if(eventData.button == PointerEventData.InputButton.Right) OnRightclicked();
        
    }
    public void OnLeftclicked()
    {
        if(isItemSelect) 
        {
        }
        else
        {
            Order.DeSelectAll();
            var data = Order.GetSlotDatabyIndex(SlotIndex);
            if (data.ID == 0) return;

            //이거도 뷰모델에서 뿌려줘야함
            
            selectedshader.SetActive(true);
            isItemSelect = true;
            //TODO 뷰모델 예하 아마 뷰모델에계속둘껀데 예외적으로 그냥 뷰모델에서 처리하게 할예정
            //즉 이기능을 안쓸꺼임여기서
            //showDescriptionNameZone.text = data.ItemName;
            //showDescriptionTextZone.text = data.ItemDescription;
        }

    }
    public void OnRightclicked()
    {
        if(isItemSelect && Order.GetSlotDatabyIndex(SlotIndex).ID != 0)
        CreateContext();
    }
    public void CreateContext() 
    {
        
        string target ="ContextPanelUI";

        if(!Order.ContextUIDictionary.ContainsKey(target)) return;
        GameObject contextPanel = Instantiate(Order.ContextUIDictionary[target]);
        
        contextPanel.transform.SetParent(UImanager.manager.canvas.transform, false);
        Order.ContextUI.Push(contextPanel);
        contextPanel.SetActive(false);

        //인덱스 부여
        //나중에 init로 할예정 생각해보니 이거도 뷰가 해야함
        //아닌가?
        contextPanel.GetComponent<Inventory_View_Context>().slotIndex = SlotIndex;  
        
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
        Debug.Log("컨텍스트 생성 슬롯인덱스 : " + SlotIndex);
        ItemData_SO SlotItemData = Order.GetItemDatabyID(Order.GetSlotDatabyIndex(SlotIndex).ID);
        Debug.Log("슬롯인덱스 정보: " + SlotIndex);
        Debug.Log("슬롯아이템 정보: " + SlotItemData.ItemName);
        
        IfDisableButton(SlotItemData.ItemType != ItemTypeEnums.Equipment       ,"Equip_Panel"  ,childrenPanel);
        IfDisableButton(SlotItemData.ItemType != ItemTypeEnums.Consumable      ,"Use_Panel"    ,childrenPanel);
        IfDisableButton(Order.GetSlotDatabyIndex(SlotIndex).Quantity < 2 ,"Split_Panel"  ,childrenPanel);
        
                
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
        if(Order.GetSlotDatabyIndex(SlotIndex).ID == 0 || Order.GetSlotDatabyIndex(SlotIndex).Quantity == 0) return;
        
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
        
        if(swapTarget != null && swapTarget.SlotIndex != SlotIndex)
        {
            Inventory_ViewModel.Inventory.SwapItemSlot(SlotIndex, swapTarget.SlotIndex);
        }
       
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////
    ///
    /// 버전.2 의존성 주입 구현
    /// 
    ///


}
