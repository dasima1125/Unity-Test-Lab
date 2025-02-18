using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        GameObject panelInstance = Instantiate(manager.fullScreenUIDictionary[uiName]);
        panelInstance.transform.SetParent(manager.canvas.transform, false);

        manager.currentfullScreenUI = panelInstance;
       

    }
    public void hideScreen()
    {
        var manager = UImanager.manager;
        Destroy(manager.currentfullScreenUI);
        manager.currentfullScreenUI = null; // 뭐 갑자기 missing 이러면서 값이 남을수도있으니깐?
    }

    
}
