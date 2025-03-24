using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler ,IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    public EquipmentType EquipedSlotType;
    public bool isFull;
    [SerializeField]private bool EquipedSlot;

    private GameObject selectedshader;
    private ItemType InvetoryslotType;
    private int slotIndex;
    
    
    private void Start() 
    {
        selectedshader = transform.transform.GetChild(0).gameObject;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(!isFull) return;
        selectedshader?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectedshader?.SetActive(false);
    }
    
    //
    // Drag  Connect //
    //

    private Vector2 originalPosition , originalSize;
    private Transform originalLayer;
    private RectTransform MoveTarget;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if(!isFull) return;
    
        
        MoveTarget        =  transform.Find("ItemImage")?.GetComponent<RectTransform>();
        if (MoveTarget == null) return;
        
        var manager = EquipmentManager.Equipmen;
        manager.DeRaycastOther(false);

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
        
        var manager = EquipmentManager.Equipmen;
        manager.DeRaycastOther(true);
        
        MoveTarget.position  = originalPosition;
        MoveTarget.sizeDelta =     originalSize;
        MoveTarget.SetParent(originalLayer);
        var swapTarget = eventData.pointerCurrentRaycast;
        
        if(!EquipedSlot && swapTarget.gameObject.name == "LoadOutZoon")
        Debug.Log("아이템 장착");

        else if(EquipedSlot && swapTarget.gameObject.name != "LoadOutZoon")
        Debug.Log("아이템 해제");
    }
    
}
