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
    
    
    public void showScreen(string uiName)
    {
        var manager = UImanager.manager;
        //이 방식을 개선해야함 중복성이 심각함 분리가 필요함 컨트롤러에서 애니메이션을 실행하고 해당 애니메이션에 플래그를 넣어서 해당 플래그를 작동시키면  실행? 연구가필요함..
        GameObject FadepanelObj = Instantiate(manager.exUIDictionary["BlackoutPanel"]);
        FadepanelObj.transform.SetParent(manager.canvas.transform, false);
        
        CanvasGroup Fadepanel = FadepanelObj.GetComponent<CanvasGroup>();
        
        Fadepanel.alpha = 0f;
        Fadepanel.DOFade(1f, 0.5f).OnComplete(() =>
        {
            // 암전 후, 디버그 메시지 출력
            GameObject panelInstance = Instantiate(manager.fullScreenUIDictionary[uiName]);
            panelInstance.transform.SetParent(manager.canvas.transform, false);
            panelInstance.transform.SetSiblingIndex(manager.canvas.transform.childCount - 2);//페이드 패널보다 한칸위로 보내야함
            
            manager.currentfullScreenUI = panelInstance;
            
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.AppendInterval(0.3f).Append(Fadepanel.DOFade(0, 0.7f).OnComplete(() =>  //딜레이 추가 시퀸스 이용
            {
                Destroy(FadepanelObj);

            }).SetUpdate(true));

            fadeOutSequence.SetUpdate(true).Play();

        });
       
    }
    public void showScreen_Delta(string uiName)
    {
        var manager = UImanager.manager;
        GameObject panelInstance = Instantiate(manager.fullScreenUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        //처음 켜지는 매뉴 순서
        
        for(int i = 0; i < panelInstance.transform.childCount; i++) 
        {
            var targeted = panelInstance.transform.GetChild(i);
            manager.SwapcreenUIs.Add(i + 1, targeted.gameObject);
            if(i != manager.pageNum -1) targeted.gameObject.SetActive(false);
        }
        //Debug.Log(String.Join(" ", manager.SwapcreenUIs.Keys));
        manager.currentfullScreenUI = panelInstance;
    }

    public void SwapScreen(int i) 
    {
        var manager = UImanager.manager;
        foreach(var targeted in manager.SwapcreenUIs)
        {
            targeted.Value.SetActive(false);
        }
        /**
        int cnt = manager.pageNum += i;
        if(cnt > manager.SwapcreenUIs.Count) cnt = 1;
        else if(cnt < 1) cnt = manager.SwapcreenUIs.Count;
        manager.SwapcreenUIs[cnt].SetActive(true);
        manager.pageNum = cnt;
        */
        int count = manager.SwapcreenUIs.Count;
        manager.pageNum = ((manager.pageNum - 1 + i) % count + count) % count + 1;
        manager.SwapcreenUIs[manager.pageNum].SetActive(true);
        
        //if(manager.pageNum != null)
    }
    public void hideScreen()
    {
        var manager = UImanager.manager;
        
        manager.currentfullScreenUI.GetComponent<CanvasGroup>().DOFade(0f,0.3f).SetUpdate(true).OnComplete(() =>
        {
            Destroy(manager.currentfullScreenUI);
            manager.currentfullScreenUI = null;// 뭐 갑자기 missing 이러면서 값이 남을수도있으니깐?
            if(manager.SwapcreenUIs.Count > 0)
            manager.SwapcreenUIs.Clear();      //null처리하면 예외처리뜸
        });
         
    }

    
}
