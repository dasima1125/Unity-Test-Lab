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
                if(instance == null)
                {
                    GameObject Object = new GameObject(nameof(UImanager));
                    instance = Object.AddComponent<UImanager>();
                }
            }
            return instance;  
        }
    }
    private Dictionary<string, Object> popupUIDictionary = new();//게임 시작시 딱한번 로드하고 저장 팝업ui 오브젝트 저장용
    private Stack<Object> currentPopupUI = new Stack<Object>(); //씬이 바뀔때는 ui스택을 전부 날려야하기때문에  정적선언 X
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
            
            Object[] popupUIs = Resources.LoadAll<Object>("PopupUIs");
            foreach (Object popupUI in popupUIs)
            {
                if (!popupUIDictionary.ContainsKey(popupUI.name)) popupUIDictionary.Add(popupUI.name, popupUI);
            }
        }
    }
    public GameObject testPanel;
    private GameObject testPanelInstance;
    void Start()
    {
        Debug.Log("hello world");
        
    }

    // Update is called once per frame
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
    public void ShowPanel_popup_info(string uiName)
    {
        if (!popupUIDictionary.ContainsKey(uiName)) 
        {
            Debug.LogError("존재하지 않는 팝업UI");
            return;
        }
        

        GameObject panelInstance = Instantiate(popupUIDictionary[uiName] as GameObject);
        panelInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        
        if(currentPopupUI.Count > 0)
        {
            Debug.Log("덮어쓰기");
            GameObject newPosObject = currentPopupUI.Peek() as GameObject;
            Vector2 newPos = newPosObject.transform.position;
            panelInstance.transform.position = new Vector2(newPos.x + 20, newPos.y - 20);

        }

        currentPopupUI.Push(panelInstance);

        

        //Vector3 newPosition = panelInstance.transform.position;
        //Debug.Log(newPosition);
        //newPosition.x += 10; // 오른쪽으로 10
        //newPosition.y += 10; // 위로 10
        //panelInstance.transform.position = newPosition;
        
        

        Debug.Log("저장된 작업 스택: " + currentPopupUI.Count);

    }
    public void HidePanel_popup_info()
    {
        if (currentPopupUI.Count < 1) 
        {
            Debug.LogError("팝업UI가 씬에 존재하지 않습니다.");
            return;
        }
        GameObject panelInstance = currentPopupUI.Pop() as GameObject;
       
        Destroy(panelInstance);
        Debug.Log("저장된 작업 스택: " + currentPopupUI.Count);
        
    }
}
