using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UImanager_PopupUI : MonoBehaviour
{
    private static UImanager_PopupUI instance;
    public static UImanager_PopupUI PopupUI
    {
        get => instance ?? (instance = FindObjectOfType<UImanager_PopupUI>());
    }
    public void solo_showPopUp(string uiName)
    {
        var manager = UImanager.manager;

        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);

        panelInstance.transform.localScale = Vector3.zero;
        panelInstance.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack); 

        manager.currentPopupUI.Push(panelInstance);
    }
    //흐려진상태에서 1.5배큰상태였다 점점 또렷해지면서 1크기로 돌아옴

    //시퀸스 Sequence sequence = DOTween.Sequence(); 선언하고
    //sequence.Join()으로 합치셈 예시는 아래에
    //sequence.Join(
    //panelInstance.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack),  // 첫 번째 애니메이션 (스케일 변화)
    //canvasGroup.DOFade(0f, 0.3f),  // 두 번째 애니메이션 (알파 변화)
    //panelInstance.transform.DORotate(new Vector3(0, 0, 45), 0.3f)  // 세 번째 애니메이션 (회전)
    //);
    //
    //
    public void hidePopUp() 
    {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        panelInstance.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(panelInstance);
        }).SetUpdate(true);
    }
    public void show(string uiName) 
    {
        var manager = UImanager.manager;
       
        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        if(manager.currentPopupUI.Count > 0)
        {
            Vector2 newPos = manager.currentPopupUI.Peek().transform.position;
            panelInstance.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);
        }
        manager.currentPopupUI.Push(panelInstance);
    
        UIComposer.Call.Next();
        
    }
    public void hide() 
    {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        Destroy(panelInstance);
        
        
    }
    /// <summary>
    /// 후순위 적용을 해야합니다 존재하는 팝업이없는경우 문제가 생깁니다.
    /// </summary>
}
/**
public void hide() 
    {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        Destroy(panelInstance);
        
    }
    public void fadeIN() {
        
    }
    public void fadeOut() {
        var manager = UImanager.manager;

        GameObject panelInstance = manager.currentPopupUI.Pop();
        panelInstance.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            manager.currentPopupUI.Push(panelInstance);
            UIComposer.Call.Next();

        }).SetUpdate(true);
        
    }
*/
