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
            UIUpdater.Updater.ChatBoxUI();

            action =  UImanager_fullScreen.fullScreen;
            anim =    UIAnimator.anim;
            
        }
    }
    #region 자원 저장 관련
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public GameObject blackoutPanel;
    public Dictionary<string, GameObject> exUIDictionary = new();
    private UImanager_fullScreen action;
    private UIAnimator anim;

    public void SceenUpdate()
    {
        UIUpdater.Updater.NewSceenUpdate();
    }
    #endregion

    #region ChatBox UI 관련

    public Dictionary<string, GameObject> chatBoxUIs = new();
    public GameObject chatBoxUI = null;

    public void testalpha()
    {
        if(!chatBoxUIs.ContainsKey("ChatBox") || chatBoxUI != null )
        {
            Debug.Log("찾을수없는 ui");
            return; 
        } 
        UIComposer.Call.Execute(() => StartCoroutine(UImanager_ChatBox.Chat.PrintDialog()));
   
    }

    #endregion

    #region 팝업 UI 관련
    public Dictionary<string, GameObject> popupUIDictionary = new();
    public Stack<GameObject> currentPopupUI = new Stack<GameObject>(); 
    public void ShowPanel_popup_info(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            return;
        }
        UIComposer.Call.Execute(() => UImanager_PopupUI.PopupUI.show(uiName));
    }
    public void HidePanel_popup_info()
    {
        if (currentPopupUI.Count < 1) 
        {
            return;
        }
        UIComposer.Call.Execute(() => anim.popup_fadeOut(),()=> UImanager_PopupUI.PopupUI.hide());
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
        UIComposer.Call.Execute(() => anim.fullscreen_animation_blank(),() => action.showScreen_Alpha(uiName));
    }
    public void hidePanel_fullScreen()
    {
        UIComposer.Call.Execute(() => anim.fullscreen_fadeOut(),() => action.hideScreen());
    }
    public void fullScreenPanel(string uiName) 
    {
        if(!fullScreenUIDictionary.ContainsKey(uiName) || currentfullScreenUI != null ) return; 
        UIComposer.Call.Execute(() => anim.fullscreen_animation_blank(),() => action.showScreen_Delta(uiName));
       
    }
    public void SwapPanel(int i)
    {
        if(currentfullScreenUI == null || SwapcreenUIs.Count < 2) return;
        UIComposer.Call.Execute(() => anim.fullscreen_animation_blank(),() => action.SwapScreen(i));

    }
    
    #endregion

    #region 강제적 통제 구획
    private void flush_AllPanel()
    {
        Debug.LogWarning("⚠️ 경고 : 모든 패널값이 초기화되었습니다");
        if(currentfullScreenUI != null)Destroy(currentfullScreenUI);
        currentfullScreenUI = null;
        foreach(var ui in currentPopupUI)Destroy(ui);
        currentPopupUI.Clear();
        Debug.LogError("삭제 완료 : 현재 저장상태" +  (currentfullScreenUI == null ? "null" : currentfullScreenUI.name)  + " | " +currentPopupUI.Count);
    }

    #endregion

    
}
