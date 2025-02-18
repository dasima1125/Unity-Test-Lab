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
        var manager = UImanager.manager;
       
        manager.currentPopupUI.Clear();// <-이거 업데이터로 넘겨야할거같네
        manager.SceenUpdate();
    }
    private void OnDisable()
    {  
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
