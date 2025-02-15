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
     
        }
    }
    public GameObject testPanel;
    private GameObject testPanelInstance;
    void Start()
    {
        
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
        testPanelInstance.SetActive(false); //널체크를 딱히 할필요가 없긴해..
        
    }
}
