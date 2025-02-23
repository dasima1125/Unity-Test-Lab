using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] string call_popupID; 
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnShowButtonClicked(string a)
    {
        //Debug.Log("팝업생성 명령");
        UImanager.manager.ShowPanel_popup_info(call_popupID);
    }
    public void OnHideButtonClicked()
    {
        //Debug.Log("뒤로가기 명령");
        UImanager.manager.HidePanel_popup_info();
    }
    public void fullScreen_Clicked(string a)
    {
        UImanager.manager.showPanel_fullScreen("resume panel");
    }
    public void fullScreen_Clicked_delta(string a)
    {
        UImanager.manager.fullScreenPanel("totalstate panel");
    }
    public void fullScreen_swap (int i) 
    {
        Debug.Log("스왑 입력");
        UImanager.manager.SwapPanel(i);
        
    }
    public void fullScreenHide_Clicked()
    {
        //Debug.Log("팝업생성 명령");
        UImanager.manager.hidePanel_fullScreen();
    }
    public void test_Clicked()
    {
        //Debug.Log("팝업생성 명령");
        UImanager.manager.testalpha();
    }
}
