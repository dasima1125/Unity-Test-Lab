using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory_View_Context : MonoBehaviour
{
    private Inventory_ViewModel Manager;
    private GameObject itemBulk;
    public int slotIndex;
    public void Awake()
    {
        Manager = Inventory_ViewModel.Inventory;
        itemBulk =Resources.Load<GameObject>("InventorySystem/prefep/ItemBulk");
    }
    public void Context_back()
    {
        if(Manager.ContextUI.Count < 1) return; //있어도 되고 없어도됨
        GameObject removeTarget = Manager.ContextUI.Pop();
        Destroy(removeTarget);

        if(Manager.ContextUI.Count > 0) Manager.ContextUI.Peek().SetActive(true); 
    
    }
    public void Context_In(string PanelName)
    {
        if(!Manager.ContextUIDictionary.ContainsKey(PanelName)) return;
        if(Manager.ContextUI.Count > 0) Manager.ContextUI.Peek().SetActive(false);

        GameObject newTarget = Instantiate(Manager.ContextUIDictionary[PanelName],UImanager.manager.canvas.transform, false);
        Manager.ContextUI.Push(newTarget);

        newTarget.GetComponent<Inventory_View_Context>().orderType = PanelName;
        newTarget.GetComponent<Inventory_View_Context>().slotIndex = slotIndex;
        newTarget.GetComponent<Inventory_View_Context>().SetupConutElmental(slotIndex);   
    }
    public void ContextUse(string order) 
    {
        Debug.Log("명령 실행");
        orderType = order;
        AcceptContext();
    }

    private TMP_Text ScoreZoon;
    private int SelectQuantity;
    private int MaxQuantity;

    [HideInInspector]
    public string orderType;
    public void SetupConutElmental(int TargetItemIndex)
    {
        ScoreZoon = transform.GetChild(0)
                    .GetChild(1)  
                    .GetChild(0)  
                    .GetComponent<TMP_Text>();
                    
        MaxQuantity = Manager.GetSlotDatabyIndex(slotIndex).Quantity;
        if(MaxQuantity < 2)
        {   
            
            AcceptContext();
            return;
        }
        SelectQuantity = 1;
        ScoreZoon.SetText("{0} / {1}", SelectQuantity, MaxQuantity);

    }
    //기능별 컨텍스트 인풋 서비스 
    public void IncreaseQuantity()
    {
        if(SelectQuantity >= MaxQuantity) return;
        ScoreZoon.SetText("{0} / {1}", ++SelectQuantity, MaxQuantity);

    }
    public void DecreaseQuantity()
    {
        if(SelectQuantity <= 1) return;
        ScoreZoon.SetText("{0} / {1}", --SelectQuantity, MaxQuantity);
    }
    //기능별 실행 서비스 

    public void AcceptContext()
    {
        switch (orderType)
        {
            case "Equip":
                if(DataManager.data.EquipedDatas == null)
                return;
                Debug.Log("명령 진행");
                //데이터 전송
                Manager.EquipedItem(slotIndex);

                //해당슬롯 업데이트
                Manager.UpdateSlot(slotIndex);
                Context_back();
            
            break;
            
            case "use":
               
            
            break;
            
            case "ContextSplitlUI":
                
                var slotData = Manager.GetSlotDatabyIndex(slotIndex);
                if(
                    slotData.ID == 0 ||
                    SelectQuantity <= 0 ||
                    slotData.Quantity < SelectQuantity 
                    ) break;

                var targetIndex = Manager.SplitItem(slotIndex,SelectQuantity);
                if(targetIndex == -1)  break; //실패 선언 

                Manager.UpdateSlot(slotIndex);
                Manager.UpdateSlot(targetIndex);
                
            break;
            case "ContextDroplUI":
                GameObject dropItem = Instantiate(itemBulk);
                dropItem.transform.position = GameObject.Find("player").transform.position;
                
                var OutputData = Manager.GetItemDatabyID(Manager.GetSlotDatabyIndex(slotIndex).ID);
                if(OutputData == null)
                {
                    Debug.Log("데이터 추출실패. 작업을 취소합니다..");
                    break;
                }

                int quantityToDrop = SelectQuantity == 0 ? 1 : SelectQuantity;
                Manager.TakeOutItem(slotIndex,quantityToDrop);

                var setting = dropItem.GetComponent<Items>();

                setting.Setup(OutputData.ItemID,quantityToDrop);
                setting.CanPick = false;

                Manager.UpdateSlot(slotIndex);
                
            break;
            
            default:
            break;
        }

        Context_back();
    }
    
}

