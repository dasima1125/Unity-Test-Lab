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
    public void RegisterPopupUI () 
    {
        var manager = UImanager.UI_Instance;
        GameObject[] popupUIs = Resources.LoadAll<GameObject>("PopupUIs");
        foreach (GameObject popupUI in popupUIs)
            {
                if (!manager.popupUIDictionary.ContainsKey(popupUI.name)) 
                {
                    manager.popupUIDictionary.Add(popupUI.name, popupUI);
                }
            }
    }
    public void NewSceenUpdate()
    {
        UImanager.UI_Instance.canvas = FindObjectOfType<Canvas>();
        
    }
}
