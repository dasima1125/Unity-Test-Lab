using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager_observer : MonoBehaviour
{
    // Start is called before the first frame update
    static bool SceneLoadedSubscribed = false;

    void Awake()
    {
        // 이벤트가 구독되지 않았을 때만 구독하기
        if (!SceneLoadedSubscribed)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoadedSubscribed = true;  // 구독된 상태로 변경
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        UImanager.UI_Instance.currentPopupUI.Clear();
    }
    private void OnDisable()
    {  
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
