using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UImanager_PopupUI : MonoBehaviour
{
    private static UImanager_PopupUI instance;
    public static UImanager_PopupUI PopupUI
    {
        get => instance ?? (instance = FindObjectOfType<UImanager_PopupUI>());
    }
    
    public void show(string uiName) 
    {
        var manager = UImanager.manager;
       
        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        if(manager.currentPopupUI.Count > 0)
        {
            Vector2 newPos = manager.currentPopupUI.Peek().transform.position;
            panelInstance.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);
        }
        manager.currentPopupUI.Push(panelInstance);
    
        UIComposer.Call.Next();
        
    }
    public void ShowInventory(string uiName) 
    {
        var manager = UImanager.manager;

        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        
        var send = InventoryManager.Inventory;
        send.slotPostion = panelInstance.transform.Find("inventorySlot");
        send.DescriptionName_TMP = panelInstance.transform.Find("inventoryDescirption/DescriptionName/Name").GetComponent<TMP_Text>();
        send.DescriptionText_TMP = panelInstance.transform.Find("inventoryDescirption/DescriptionText/Text").GetComponent<TMP_Text>();
        //send.Updating();
        send.Updating2();
        
        
        manager.currentPopupUI.Push(panelInstance);
    
        UIComposer.Call.Next();
    }
    public void hide() 
    {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        Destroy(panelInstance);
        
    }
   
}

