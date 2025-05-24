using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
            UIUpdater.Updater.DynamicUIs();
            UIUpdater.Updater.PanelUIs();
            UIUpdater.Updater.PopupUIs();
            UIUpdater.Updater.fullScreenUIs();
            UIUpdater.Updater.ChatBoxUIs();

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
    
    #region Dynamic UI 관련
    public Dictionary<string, GameObject> DynamicUIs = new();
    public Queue<GameObject> ItemPanelQueue = new();
    public GameObject InfoUI = null;

    public void ShowItemInfo(string [] insert)
    {
        UIComposer.Call.Execute(() => UImanager_Dynamic.Dynamic.MakeGetItem(insert));
    }
    

    #endregion

    #region ChatBox UI 관련
    //public PlayerInput playerInput;
    public Dictionary<string, GameObject> chatBoxUIs = new();
    public GameObject chatBoxUI = null;
    public bool talking = false;//대화중 상태 통제
    public bool skip = false;//스킵명령 하달

    public void testalpha()
    {
        /**
        if(!chatBoxUIs.ContainsKey("ChatBox") || chatBoxUI != null )
        {
            Debug.Log("찾을수없는 ui");
            return; 
        } 
        UIComposer.Call.Execute(() => StartCoroutine(UImanager_ChatBox.Chat.PrintDialog()));
        */
        UIComposer.Call.Execute(() => action.showScreen_Alpha("Overlay"));
   
    }    
    void OnTest()
    {
        var control = GetComponent<PlayerInput>().actions["test"].activeControl.displayName;
        if(control == "T")
        {
            string [] testinsert = {"테스트 아이템","정보"};
            UIComposer.Call.Execute(() => UImanager_Dynamic.Dynamic.MakeGetItem(testinsert));
        }
        if(control == "E")
        {
            if(chatBoxUI != null && talking && !skip) skip = true;
            else
            {
                string [] testinsert = {"--테스트 인포--","테스트 정보 출력부분"};
                if(InfoUI == null)
                UIComposer.Call.Execute(() => UImanager_Dynamic.Dynamic.MakeInfo(testinsert));
            }
            
        }
        if (control == "Tab")
        {
            if (currentfullScreenUI == null)
                UIComposer.Call.Execute(() => action.showScreen_Alpha("Overlay"));
        }
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
    public void ShowPanel_popup_Inventory(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            return;
        }
        UIComposer.Call.Execute(() => UImanager_PopupUI.PopupUI.ShowInventory(uiName));
    }
    public void ShowPanel_popup_Equipment(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            return;
        }
        UIComposer.Call.Execute(() => UImanager_PopupUI.PopupUI.ShowEquipment(uiName));
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
