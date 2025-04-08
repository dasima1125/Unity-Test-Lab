using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment_View : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    public EquipmentTypeEnums EquipedSlotType;
    public Image SlotImage;
    [SerializeField]private Sprite nullImage;
    [SerializeField]private bool EquipedSlot;
    [SerializeField]private bool IsFull;


    public GameObject selectedshader;
    private int ? SlotIndex;
    private int ? LinkeditemIndex;
    
    /// <summary>
    /// 슬롯을 설정하는 함수입니다.
    /// </summary>
    /// <param name="EquipedSlot">장비 슬롯 여부 (true: 장비칸, false: 인벤토리칸)</param>
    /// <param name="LinkeditemIndex">연결된 아이템의 인덱스 </param>
    /// <param name="SlotIndex">슬롯의 인덱스 ( 장비칸일 경우 필요 없음)</param>
    public void SetupSlot(bool EquipedSlot ,EquipmentTypeEnums EquipedSlotType ,int ? LinkeditemIndex = null ,int ? SlotIndex = null)
    {
        this.LinkeditemIndex = LinkeditemIndex;
        this.EquipedSlotType = EquipedSlotType;
        this.EquipedSlot = EquipedSlot;
        this.SlotIndex = SlotIndex; 
    
        if(LinkeditemIndex != null )
        {
            var data = Equipment_ViewModel.Equipment.GetDataInfoByIndex(LinkeditemIndex.Value);
            SlotImage.sprite = data.Sprite;
            IsFull = true;
        }
        else
        {
            SlotImage.sprite = nullImage;
            IsFull = false;
        } 
            
    
    }
    public void SetUpEquipedSlot(bool EquipedSlot ,int itemID = 0)
    {
        SlotImage.sprite = nullImage;
        this.EquipedSlot = EquipedSlot;
        if(itemID != 0) //아이디가있으면 이미지업데이트
        {
            var data = Equipment_ViewModel.Equipment.GetDataInfoByID(itemID);
            if(data == null) 
            {
                Debug.Log("데이터 호출 실패");
                return;   
            }
            SlotImage.sprite = data.Sprite;
            IsFull = true;
            return;
        }
        IsFull = false;
    }
    public void PointHighligt(bool onoff)
    {
        selectedshader.SetActive(onoff);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(IsFull)
        selectedshader.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectedshader.SetActive(false);
    }
    //
    // Drag  Connect //
    //
    private Vector2 originalPosition , originalSize;
    private Transform originalLayer;
    private RectTransform MoveTarget;
    private EquipmentTypeEnums MoveTargetType;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!IsFull) return;
        MoveTarget = transform.Find("ItemImage").GetComponent<RectTransform>();
        Equipment_ViewModel.Equipment.DeRaycastOther(false);
      

        originalPosition = MoveTarget.position;
        originalSize     = MoveTarget.sizeDelta;
        originalLayer    = MoveTarget.parent;

        MoveTargetType = EquipedSlotType;

        MoveTarget.SetParent(MoveTarget.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(MoveTarget == null) return;
        MoveTarget.position = Input.mousePosition;

        var swapTarget = eventData.pointerCurrentRaycast;
        if(swapTarget.gameObject == null) return;

        if(!EquipedSlot && swapTarget.gameObject.name == "LoadOutZoon")
        {
            Equipment_ViewModel.Equipment.HighlightOnHover(EquipedSlotType,true);
        }
        else
        {
            Equipment_ViewModel.Equipment.HighlightOnHover(EquipedSlotType,false);
        }


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(MoveTarget == null) return;
        Equipment_ViewModel.Equipment.DeRaycastOther(true);

        MoveTarget.position = originalPosition;
        MoveTarget.sizeDelta = originalSize;
        MoveTarget.SetParent(originalLayer);

        var SelectTarget = eventData.pointerCurrentRaycast;
        if (SelectTarget.gameObject == null) return;
        if(!EquipedSlot && SelectTarget.gameObject.name == "LoadOutZoon")
        {
            Equipment_ViewModel.Equipment.EquipItem(EquipedSlotType,LinkeditemIndex.Value);
        }
        else if(EquipedSlot && SelectTarget.gameObject.name != "LoadOutZoon")
        {
            Equipment_ViewModel.Equipment.UnequipItem(MoveTargetType);
        }
    
    
    }
    
    
    
}
