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
    public void showPopUp(string uiName)
    {
        var manager = UImanager.manager;

        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);

        panelInstance.transform.localScale = Vector3.zero;
        panelInstance.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack); 

        if(manager.currentPopupUI.Count > 0)
        {
            Vector2 newPos = manager.currentPopupUI.Peek().transform.position;
            panelInstance.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);
        }
        manager.currentPopupUI.Push(panelInstance);
    }

    public void hidePopUp() 
    {
        var manager = UImanager.manager;
        GameObject panelInstance = manager.currentPopupUI.Pop();
        panelInstance.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(panelInstance);
        }).SetUpdate(true);
    }
}
