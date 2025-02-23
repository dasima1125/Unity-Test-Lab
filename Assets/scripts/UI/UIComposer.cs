using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComposer : MonoBehaviour
{
    // Start is called before the first frame update
    private static UIComposer instance;
    public static UIComposer Call
    {
        get => instance ?? (instance = FindAnyObjectByType<UIComposer>());
    }


    public event Action NextEvent;
    #nullable enable
    /// <summary>
    /// 컨트롤, 또는 애니메이션 ,컨트롤 방식으로 호출 가능, 람다식으로 선언요구
    /// </summary>
    /// <param name="action1">컨트롤, 또는 애니메이션,컨트롤러 방식 가능</param>
    /// <param name="action2">대기작업, 첫번째작업도중 호출할 컨트롤 가능</param>
    public void Execute(Action ? action1 = null , Action ? action2 = null)
    {
        NextEvent = null;
        if (action1 is Action) NextEvent += () => action2?.Invoke();
        action1?.Invoke();

    }
    public void Next()
    {
        NextEvent?.Invoke();
        NextEvent = null;
    }
    public void hi () 
    {
        Debug.Log("안녕");
    }
    

    
}
