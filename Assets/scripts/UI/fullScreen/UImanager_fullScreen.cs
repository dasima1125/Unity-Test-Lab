using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public void hideScreen()
    {
        var manager = UImanager.manager;
        
        manager.currentfullScreenUI.GetComponent<CanvasGroup>().DOFade(0f,0.3f).SetUpdate(true).OnComplete(() =>
        {
            Destroy(manager.currentfullScreenUI);
            manager.currentfullScreenUI = null;
        });
        

        //Destroy(manager.currentfullScreenUI);
         // 뭐 갑자기 missing 이러면서 값이 남을수도있으니깐?
    }

    
}
