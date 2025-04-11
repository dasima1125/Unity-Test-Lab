using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    private static UIUpdater instance;
    public static UIUpdater Updater
    {
        get 
        {
            if(instance == null) instance = FindObjectOfType<UIUpdater>();
            return instance; 
        }
    }
    public void DynamicUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] dynamicUIs = Resources.LoadAll<GameObject>("DynamicUIs");
        
      
        foreach(GameObject dynamicUI in dynamicUIs)
        {
            if(!manager.DynamicUIs.ContainsKey(dynamicUI.name)) 
            {
                manager.DynamicUIs.Add(dynamicUI.name,dynamicUI);   
            }
        }
        
    }
    public void ChatBoxUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] ChatBoxUIs = Resources.LoadAll<GameObject>("ChatBoxUIs");
      
        foreach(GameObject ChatBox in ChatBoxUIs)
        {
            if(!manager.chatBoxUIs.ContainsKey(ChatBox.name)) 
            {
                manager.chatBoxUIs.Add(ChatBox.name,ChatBox);   
            }
        }
        
    }
    public void PanelUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] exUIs = Resources.LoadAll<GameObject>("exUIs");
        foreach(GameObject panels in exUIs)
        {
            if(!manager.exUIDictionary.ContainsKey(panels.name)) 
            {
                manager.exUIDictionary.Add(panels.name,panels);   
            }
        }
    }
    public void PopupUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] popupUIs = Resources.LoadAll<GameObject>("PopupUIs");
        foreach (GameObject popupUI in popupUIs)
            {
                if (!manager.popupUIDictionary.ContainsKey(popupUI.name)) 
                {
                    manager.popupUIDictionary.Add(popupUI.name, popupUI);
                }
            }
    }
    public void fullScreenUIs()
    {
        var manager = UImanager.manager;
        GameObject[] fullScreenUIs = Resources.LoadAll<GameObject>("FullScreenUIs");
        foreach(GameObject fullScreenUI in fullScreenUIs)
        {
            if(!manager.fullScreenUIDictionary.ContainsKey(fullScreenUI.name))
            {
                manager.fullScreenUIDictionary.Add(fullScreenUI.name, fullScreenUI);
            }
        }
    }
    public void NewSceenUpdate()
    {
        Debug.Log("씬 전환");
        UImanager.manager.canvas = FindObjectOfType<Canvas>();
        
        while (UImanager.manager.currentPopupUI.Count > 0)
        {
            GameObject popup = UImanager.manager.currentPopupUI.Pop(); // 스택에서 하나씩 꺼냄
            if (popup != null) 
            {
                Destroy(popup); // 오브젝트 삭제
            }
        }
        UImanager.manager.currentPopupUI.Clear();
        UImanager.manager.ItemPanelQueue.Clear();
        //Debug.Log(UImanager.manager.currentPopupUI.Count);
        
    }
}
