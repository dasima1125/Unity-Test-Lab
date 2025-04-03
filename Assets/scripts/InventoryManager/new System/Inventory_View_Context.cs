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
                    
        MaxQuantity = DataManager.data.InventoryList[TargetItemIndex].Quantity;
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
                //if()
                return;
            
            break;
            
            case "use":
                return;
            
            break;
            
            case "ContextSplitlUI":
                var targetIndex = Manager.SplitItem(slotIndex,SelectQuantity);
                if(targetIndex < 0)  return;

                Manager.UpdateSlot(slotIndex);
                Manager.UpdateSlot(targetIndex);
                
            break;
            case "ContextDroplUI":
                GameObject dropItem = Instantiate(itemBulk);
                dropItem.transform.position = GameObject.Find("player").transform.position;
                
                var OutputData = Manager.GetItemDatabyIndex(slotIndex);
                var OutputQuantity = Manager.ItemDecrease(slotIndex,SelectQuantity == 0 ? 1 : SelectQuantity);
                
                if(OutputData == null ||OutputQuantity == 0)
                {
                    Debug.Log("데이터 추출실패. 작업을 취소합니다..");
                    Destroy(dropItem);
                    return;
                }
                var setting = dropItem.GetComponent<Items>();
                setting.Setup(OutputData.ItemID,OutputQuantity);
                setting.CanPick = false;

                Manager.UpdateSlot(slotIndex);
            break;
            
            default:
            break;
        }

        Context_back();
    }
    
}

