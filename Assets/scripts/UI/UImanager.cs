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
            UIUpdater.Updater.Panels();
            UIUpdater.Updater.RegisterPopupUI();
            UIUpdater.Updater.RegisterfullScreenUI();
        }
    }
    #region 자원 저장 관련
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public GameObject blackoutPanel;
    public Dictionary<string, GameObject> exUIDictionary = new();

    public void SceenUpdate()
    {
        UIUpdater.Updater.NewSceenUpdate();
    }
    #endregion

    #region 팝업 UI 관련
    public Dictionary<string, GameObject> popupUIDictionary = new();
    public Stack<GameObject> currentPopupUI = new Stack<GameObject>(); 
    public void ShowPanel_popup_info(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            //Debug.LogError("해당팝업이 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.PopupUI.showPopUp(uiName);
    }
    public void HidePanel_popup_info()
    {
        if (currentPopupUI.Count < 1) 
        {
            //Debug.LogError("팝업UI가 씬에 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.PopupUI.hidePopUp();
    }
    #endregion


    #region 풀스크린 UI 관련
    public Dictionary<string, GameObject> fullScreenUIDictionary = new();
    public Dictionary<int, GameObject> SwapcreenUIs = new();
    public GameObject currentfullScreenUI = null;
    public int pageNum = 1;
    public void showPanel_fullScreen(string uiName)
    {
        if(!fullScreenUIDictionary.ContainsKey(uiName) || currentfullScreenUI != null ) return; 
        UImanager_fullScreen.fullScreen.showScreen(uiName);
    }
    public void hidePanel_fullScreen()
    {
        UImanager_fullScreen.fullScreen.hideScreen();
    }
    public void fullScreenPanel(string uiName) 
    {
        if(!fullScreenUIDictionary.ContainsKey(uiName) || currentfullScreenUI != null ) return; 
        UImanager_fullScreen.fullScreen.showScreen_Delta(uiName);
    }
    public void SwapPanel(int i)
    {
        if(currentfullScreenUI == null || SwapcreenUIs.Count < 2) return;
        UImanager_fullScreen.fullScreen.SwapScreen(i);

    }
    #endregion
    
}
