using System.Collections;
using System.Collections.Generic;
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
    public void RegisterPopupUI() 
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
    public void RegisterfullScreenUI()
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
        //Debug.Log(manager.fullScreenUIDictionary.Count);
    }
    public void NewSceenUpdate()
    {
        UImanager.manager.canvas = FindObjectOfType<Canvas>();
        
    }
}
