using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UImanager : MonoBehaviour
{
    private static UImanager instance;
    public static UImanager manager
    { 
        get => instance ?? (instance = FindObjectOfType<UImanager>());
    }
    public Dictionary<string, GameObject> popupUIDictionary = new();
    public Dictionary<string, GameObject> fullScreenUIDictionary = new();
    public Stack<GameObject> currentPopupUI = new Stack<GameObject>(); 
    public GameObject currentfullScreenUI = null;

    [HideInInspector] public Canvas canvas;
    void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UIUpdater.Updater.RegisterPopupUI();
            UIUpdater.Updater.RegisterfullScreenUI();
        }
    }
    public void ShowPanel_popup_info(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            Debug.LogError("해당팝업이 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.PopupUI.showPopUp(uiName);
    }
    public void HidePanel_popup_info()
    {
        if (currentPopupUI.Count < 1) 
        {
            Debug.LogError("팝업UI가 씬에 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.PopupUI.hidePopUp();
    }

    public void showPanel_fullScreen(string uiName)
    {
        if(!fullScreenUIDictionary.ContainsKey(uiName) || currentfullScreenUI != null )
        {
            Debug.LogError("조건에 가로막힘");
            return; 
        }
        Debug.Log(currentfullScreenUI);

        UImanager_fullScreen.fullScreen.showScreen(uiName);
    }
    public void hidePanel_fullScreen()
    {
        UImanager_fullScreen.fullScreen.hideScreen();
    }
    public void SceenUpdate()
    {
        UIUpdater.Updater.NewSceenUpdate();
    }
}
