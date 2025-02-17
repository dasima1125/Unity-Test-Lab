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
    public void OnShowButtonClicked()
    {
        //Debug.Log("팝업생성 명령");
        UImanager.UI_Instance.ShowPanel_popup_info(call_popupID);
    }
    public void OnHideButtonClicked()
    {
        //Debug.Log("뒤로가기 명령");
        UImanager.UI_Instance.HidePanel_popup_info();
    }
}
