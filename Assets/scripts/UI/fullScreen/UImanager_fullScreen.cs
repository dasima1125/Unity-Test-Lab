using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UImanager_fullScreen : MonoBehaviour
{
    // Start is called before the first frame update
    private static UImanager_fullScreen instance;

    public static UImanager_fullScreen fullScreen
    {
        get => instance ?? (instance = FindObjectOfType<UImanager_fullScreen>());
    }
    
    
    public void showScreen_Alpha(string uiName) //resume같은거
    {
        var manager = UImanager.manager;

        GameObject panelInstance = Instantiate(manager.fullScreenUIDictionary[uiName]);//주 패널 생성
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        if(GameObject.Find("BlackoutPanel(Clone)"))panelInstance.transform.SetSiblingIndex(manager.canvas.transform.childCount - 2);
        
        manager.currentfullScreenUI = panelInstance;
        
    }
    public void showScreen_Delta(string uiName) //패널있는 인밴토리같은거
    {
        var manager = UImanager.manager;

        GameObject panelInstance = Instantiate(manager.fullScreenUIDictionary[uiName]);//주 패널 생성
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        if(GameObject.Find("BlackoutPanel(Clone)"))panelInstance.transform.SetSiblingIndex(manager.canvas.transform.childCount - 2);
        
        manager.currentfullScreenUI = panelInstance;

        for(int i = 0; i < panelInstance.transform.childCount; i++)//매뉴 패널들 등록 
        {
            var targeted = panelInstance.transform.GetChild(i);
            manager.SwapcreenUIs.Add(i + 1, targeted.gameObject);
            if(i != manager.pageNum -1) targeted.gameObject.SetActive(false);
        } 
    }

    public void SwapScreen(int i) 
    {
        var manager = UImanager.manager;
        
        foreach(var targeted in manager.SwapcreenUIs) targeted.Value.SetActive(false);
        int count = manager.SwapcreenUIs.Count;
        manager.pageNum = ((manager.pageNum - 1 + i) % count + count) % count + 1;
        manager.SwapcreenUIs[manager.pageNum].SetActive(true);

    }
    public void hideScreen()
    {
        var manager = UImanager.manager;
        
        Destroy(manager.currentfullScreenUI);
        manager.currentfullScreenUI = null;// 뭐 갑자기 missing 이러면서 값이 남을수도있으니깐?
        if(manager.SwapcreenUIs.Count > 0)
        manager.SwapcreenUIs.Clear();      //null처리하면 예외처리뜸
        
         
    }

    
}
