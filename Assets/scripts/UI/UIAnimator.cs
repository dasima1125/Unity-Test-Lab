using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    private static UIAnimator instance;
    public static UIAnimator anim
    {
        get => instance ?? (instance = FindObjectOfType<UIAnimator>());

    }
    #region 팝업창 UI
    public void popup_Expand(){
        if(UImanager.manager.currentPopupUI.Count <= 0)
        {
            Debug.Log("이 애니메이션은 후순위 적용입니다.");
            return;
        } 
        
        var manager = UImanager.manager.currentPopupUI.Pop();
        
        manager.transform.localScale = Vector2.zero;
        manager.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack); 

        if(UImanager.manager.currentPopupUI.Count > 0)
        {
            Vector2 newPos = UImanager.manager.currentPopupUI.Peek().transform.position;
            manager.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);
        }
        UImanager.manager.currentPopupUI.Push(manager);
        
    }
    public void popup_Shrink() {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        

        panelInstance.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            manager.currentPopupUI.Push(panelInstance);
            UIComposer.Call.Next();

        }).SetUpdate(true);
        
    }
    public void popup_fadeIn() 
    {
        var manager = UImanager.manager;
        if(manager.currentPopupUI.Count <= 0)
        {
            Debug.Log("효과를 줄 대상패널이 없습니다 : fadeIn_popup()");
            return;
        }
        
        GameObject panelInstance = manager.currentPopupUI.Pop();
        var target = panelInstance.GetComponent<CanvasGroup>();
        
        target.alpha = 0f;
        target.DOFade(1f,0.3f).SetUpdate(true);

        manager.currentPopupUI.Push(panelInstance);
    }
    public void popup_fadeOut() 
    {
        var manager = UImanager.manager;
        if(manager.currentPopupUI.Count <= 0)
        {
            Debug.Log("효과를 줄 대상패널이 없습니다 : fadeOut_popup()");
            return;
        }
        GameObject panelInstance = manager.currentPopupUI.Pop();
        var target = panelInstance.GetComponent<CanvasGroup>();
        target.interactable = false;
        //target.blocksRaycasts = false; <==이거쓰지마라 뒤에영향가서 ㅈ됨
        target.DOFade(0f,0.3f).SetUpdate(true).OnComplete(() =>
        { 
            manager.currentPopupUI.Push(panelInstance);
            UIComposer.Call.Next();
            
        });   
    }
    public void popup_ConvergeFadeIn() 
    {
        var manager = UImanager.manager;
        if(manager.currentPopupUI.Count <= 0)
        {
            Debug.Log("효과를 줄 대상패널이 없습니다 : fadeIn_popup()");
            return;
        }
        
        GameObject target = manager.currentPopupUI.Pop();
        var canvas = target.GetComponent<CanvasGroup>();
        canvas.interactable = false;

        Vector2 originalScale = target.transform.localScale;
        target.transform.localScale = originalScale * 1.5f;
        canvas.alpha = 0f;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(target.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(canvas.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        { 
            canvas.interactable = true;
        }));
        sequence.Play();
        manager.currentPopupUI.Push(target);
        
    }

    #endregion


    #region 풀스크린 UI
    public void fullscreen_animation_blank() 
    {
        var manager = UImanager.manager;
        
        
        GameObject FadepanelObj = Instantiate(manager.exUIDictionary["BlackoutPanel"]);
        FadepanelObj.transform.SetParent(manager.canvas.transform, false);
        
        CanvasGroup Fadepanel = FadepanelObj.GetComponent<CanvasGroup>();
        
        Fadepanel.alpha = 0f;
        Fadepanel.DOFade(1f, 0.5f).OnComplete(() =>
        {
            UIComposer.Call.Next();
            
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.AppendInterval(0.3f).Append(Fadepanel.DOFade(0, 0.7f).OnComplete(() => 
            {
                Destroy(FadepanelObj);

            }).SetUpdate(true));

            fadeOutSequence.SetUpdate(true).Play();
        });

    }
    public void fullscreen_fadeIn() 
    {
        var manager = UImanager.manager;
        if(manager.currentfullScreenUI == null)
        {
            Debug.Log("효과를 줄 대상패널이 없습니다 : fadeIn_fullscreen()");
            return;
        }
        var target = manager.currentfullScreenUI.GetComponent<CanvasGroup>();
        target.alpha = 0f;
        target.DOFade(1f,0.3f).SetUpdate(true);
        
    }
    
    public void fullscreen_fadeOut() 
    {
        var manager = UImanager.manager;
        if(manager.currentfullScreenUI == null)
        {
            Debug.Log("효과를 줄 대상패널이 없습니다 : fadeOut_fullscreen()");
            return;
        }
        var target = manager.currentfullScreenUI.GetComponent<CanvasGroup>();
        target.interactable = false;
        target.DOFade(0f,0.3f).SetUpdate(true).OnComplete(() =>
        { 
            UIComposer.Call.Next();
        }); 
    }
    #endregion
}
