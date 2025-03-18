using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryContextHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private InventoryManager inventory;
    private ItemSlotController controller;
    public int slotIndex;
    
    private void Start() 
    {
        inventory  = InventoryManager.Inventory;
        controller = ItemSlotController.controll;
        
    }
    public void Clicked_Back()
    {  
        if(inventory.InventoryUI.Count < 1) return;

        GameObject contextPanle = inventory.InventoryUI.Pop();
        Destroy(contextPanle);
        
        if(inventory.InventoryUI.Count > 0)
        inventory.InventoryUI.Peek().SetActive(true);
    }
    #region 컨텍스트 매뉴 구획
    public void Clicked_Use()
    {
        Debug.Log("사용버튼");
        
    }
    public void Clicked_Drop()
    {
        string a ="ContextDroplUI";
        if(!inventory.InventoryUIDictionary.ContainsKey(a)) return;
        if(inventory.InventoryUI.Count > 0) inventory.InventoryUI.Peek().SetActive(false);

        GameObject contextPanel = Instantiate(inventory.InventoryUIDictionary[a]);
        contextPanel.transform.SetParent(UImanager.manager.canvas.transform, false);
        contextPanel.GetComponent<InventoryContextHandler>().SetupDropPanel(slotIndex);

        inventory.InventoryUI.Push(contextPanel);
    }
    public void Clicked_Split()
    {
        string a ="ContextSplitlUI";
        if(!inventory.InventoryUIDictionary.ContainsKey(a)) return;
        if(inventory.InventoryUI.Count > 0) inventory.InventoryUI.Peek().SetActive(false);

        GameObject contextPanel = Instantiate(inventory.InventoryUIDictionary[a]);
        contextPanel.transform.SetParent(UImanager.manager.canvas.transform, false);
        contextPanel.GetComponent<InventoryContextHandler>().SetupDropPanel(slotIndex);

        inventory.InventoryUI.Push(contextPanel);
    }

    #endregion

    #region 버리기 팝업 구획

    private TMP_Text ScoreZoon;
    private int SelectQuantity;
    private int ItemQuantity;

    public void SetupDropPanel(int? slotIndex = null)
    {
        ScoreZoon = transform.Find(
                            "ContextPopup_Drop/" +
                            "CountPanel/" +
                            "ItemQuantityText"
                            ).GetComponent<TMP_Text>();
        if(slotIndex != null) this.slotIndex = slotIndex.Value;
        if(slotIndex != null) 
        ItemQuantity   = InventoryManager.Inventory.itemDatas[slotIndex.Value].ItemQuantity;

        SelectQuantity = 0;
        ScoreZoon.SetText("{0} / {1}", SelectQuantity, ItemQuantity);

    }
    
    
    public void DropPanel_IncreaseQuantity()
    {
        if(SelectQuantity >= ItemQuantity) return;
        ScoreZoon.SetText("{0} / {1}", ++SelectQuantity, ItemQuantity);

    }
    public void DropPanel_DecreaseQuantity()
    {
        if(SelectQuantity <= 0) return;
        ScoreZoon.SetText("{0} / {1}", --SelectQuantity, ItemQuantity);
    }
    public void DropPanel_Accept()
    {
        
        bool usAble = controller.DropItem(slotIndex , SelectQuantity);
        if(!usAble) 
        {
            Debug.LogError("오류 발생 : DropPanel_Accept()"); 
            return;
        }
        inventory.ItemSlot[slotIndex].SlotUpdate(slotIndex);
        Clicked_Back();
    }


    #endregion
    #region 나누기 팝업 구획
    public void SplitPanel_Accept()
    {
        int ? target = controller.SplitItem(slotIndex,SelectQuantity);
        if(target == null) return;
        inventory.ItemSlot[target.Value].SlotUpdate(target.Value);

        Clicked_Back();
    }
    #endregion
}
