using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    private static UIUpdater instance;
    public static UIUpdater Updater
    {
        get 
        {
            if(instance == null) instance = FindObjectOfType<UIUpdater>();
            return instance; 
        }
    }
    public void DynamicUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] dynamicUIs = Resources.LoadAll<GameObject>("DynamicUIs");
        
      
        foreach(GameObject dynamicUI in dynamicUIs)
        {
            if(!manager.DynamicUIs.ContainsKey(dynamicUI.name)) 
            {
                manager.DynamicUIs.Add(dynamicUI.name,dynamicUI);   
            }
        }
        
    }
    public void ChatBoxUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] ChatBoxUIs = Resources.LoadAll<GameObject>("ChatBoxUIs");
      
        foreach(GameObject ChatBox in ChatBoxUIs)
        {
            if(!manager.chatBoxUIs.ContainsKey(ChatBox.name)) 
            {
                manager.chatBoxUIs.Add(ChatBox.name,ChatBox);   
            }
        }
        
    }
    public void PanelUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] exUIs = Resources.LoadAll<GameObject>("exUIs");
        foreach(GameObject panels in exUIs)
        {
            if(!manager.exUIDictionary.ContainsKey(panels.name)) 
            {
                manager.exUIDictionary.Add(panels.name,panels);   
            }
        }
    }
    public void PopupUIs() 
    {
        var manager = UImanager.manager;
        GameObject[] popupUIs = Resources.LoadAll<GameObject>("PopupUIs");
        foreach (GameObject popupUI in popupUIs)
            {
                if (!manager.popupUIDictionary.ContainsKey(popupUI.name)) 
                {
                    manager.popupUIDictionary.Add(popupUI.name, popupUI);
                }
            }
    }
    public void fullScreenUIs()
    {
        var manager = UImanager.manager;
        GameObject[] fullScreenUIs = Resources.LoadAll<GameObject>("FullScreenUIs");
        foreach(GameObject fullScreenUI in fullScreenUIs)
        {
            if(!manager.fullScreenUIDictionary.ContainsKey(fullScreenUI.name))
            {
                manager.fullScreenUIDictionary.Add(fullScreenUI.name, fullScreenUI);
            }
        }
    }
    public void NewSceenUpdate()
    {
        Debug.Log("씬 전환");
        UImanager.manager.canvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None).FirstOrDefault();
        //이방식도 불확실성에 의존함, TODO 씬이 넘어갈때 메인캔버스를 물어주는 로직을 보다 직접적으로 타겟팅해줘야함
        
        //Debug.Log(UImanager.manager.canvas.gameObject.name);

        // UImanager.manager.canvas = FindObjectOfType<Canvas>();
        // 캔버스에서 이건 첫번째 발견한거를 가져오는거긴한데 문제가있다..
        // 바로 사실 하이리키순서든 이름순이든 내부자식순이든 상관이없다 이건 GetInstanceID자체가 낮은순으로 잡힌다
        // 이사실로보아 마지막으로 생성될수록 GetInstanceID가 낮아져서 그걸로 잡히게된다
        // 테스트 방법 : 캔버스 원본, 2차사본 , 3차본 , 4차본 테스트
        // 테스트 결과 : 하이리키에 나중에올라온순(생성,복사,수정 상관없음)으로 올라온다 하이리키에올라온순간 인스턴스 id가 부여되고
        //              그 ID기반으로 낮은 순으로 잡음
        // 테스트 요약 : 결과적으로 FindObjectsOfType은 나중에생성된걸 가져오는 경향이있다 즉 예측이 힘들다 
        //              결과적으로 참조를 이방식으로쓰는건 지향하도록 한다.. 그냥 이방식을쓰지말자
                    
        
        while (UImanager.manager.currentPopupUI.Count > 0)
        {
            GameObject popup = UImanager.manager.currentPopupUI.Pop(); // 스택에서 하나씩 꺼냄
            if (popup != null) 
            {
                Destroy(popup); // 오브젝트 삭제
            }
        }
        UImanager.manager.currentPopupUI.Clear();
        UImanager.manager.ItemPanelQueue.Clear();
        //Debug.Log(UImanager.manager.currentPopupUI.Count);
        
    }
}
