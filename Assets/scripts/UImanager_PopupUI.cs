using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager_PopupUI : MonoBehaviour
{
    // Start is called before the first frame update
    private static UImanager_PopupUI instance;
    public static UImanager_PopupUI UI_Instance//접근자 당분간 건들지 마쇼.
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UImanager_PopupUI>(); 
            }
            return instance;
        }
    }
    public void showPopUp(string uiName)
    {
        var manager = UImanager.UI_Instance;
        
        GameObject panelInstance = Instantiate(manager.popupUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        //                                  ㄴ  나중에 캔버스 주소를 매니저에서 가져오는방식을 써봐야지
        if(manager.currentPopupUI.Count > 0)
        {
            Vector2 newPos = manager.currentPopupUI.Peek().transform.position;
            panelInstance.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);
        }
        manager.currentPopupUI.Push(panelInstance);
    }

    public void hidePopUp() 
    {
        var manager = UImanager.UI_Instance;
        GameObject panelInstance = manager.currentPopupUI.Pop();
       
        Destroy(panelInstance);
    }
}
