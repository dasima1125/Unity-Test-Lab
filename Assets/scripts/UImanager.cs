using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UImanager : MonoBehaviour
{
    // Start is called before the first frame update
    private static UImanager instance;
    public static UImanager UI_Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UImanager>();
                // 방어코드긴한데 이게 사라질일이.. 없을꺼같긴해
                /**
                if(instance == null) 
                {
                    GameObject Object = new GameObject(nameof(UImanager));
                    instance = Object.AddComponent<UImanager>();
                }
                */
            }
            return instance;  
        }
    }
    public Dictionary<string, GameObject> popupUIDictionary = new();//게임 시작시 딱한번 로드하고 저장 팝업ui 오브젝트 저장용
    public Stack<GameObject> currentPopupUI = new Stack<GameObject>(); //씬이 바뀔때는 ui스택을 전부 날려야함.. 근데 모르겟네,,,
    public Canvas canvas;
    void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UIUpdater.Updater.RegisterPopupUI();
        }
    }
   
    
    void Start()
    {
        //Debug.Log("hello world");
        
    }

    //레거시 부분 
    //
    //딱히.. 쓸일이없음.. 그래서 잠궈둠,,
    //
    //public GameObject testPanel;
    //private GameObject testPanelInstance;
    /**
    public void ShowPanel()
    {

        if (testPanelInstance == null) // 패널이 없으면 생성
        {
            GameObject prefab = testPanel; //프리팹 로드
            
            if (prefab != null)
            {
                testPanelInstance = Instantiate(prefab);
                testPanelInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false); // UI 계층 정리
            }
            else
            {
                Debug.LogError("패널 불러오기 실패");
            }
        }
        else
        {
            testPanelInstance.SetActive(true);
        }

       
    }

    public void HidePanel()
    {
        Debug.Log("왜 작동되는거?");
        testPanelInstance.SetActive(false); //널체크를 딱히 할필요가 없긴해..
    }
    */
    public void ShowPanel_popup_info(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            Debug.LogError("해당팝업이 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.UI_Instance.showPopUp(uiName);
    }
    public void HidePanel_popup_info()
    {
        if (currentPopupUI.Count < 1) 
        {
            Debug.LogError("팝업UI가 씬에 존재하지 않습니다.");
            return;
        }
        UImanager_PopupUI.UI_Instance.hidePopUp();
    }
    public void SceenUpdate()
    {
        UIUpdater.Updater.NewSceenUpdate();
    }
}
