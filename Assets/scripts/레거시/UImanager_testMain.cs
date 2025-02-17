using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager_testMain : MonoBehaviour
{
    // Start is called before the first frame update
    public Button testBtn;
    void Start()
    {
        if(testBtn != null) testBtn.onClick.AddListener(OnShowButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnShowButtonClicked()
    {
        Debug.Log("호출 시도중 : 테스트 패널");
        //UImanager.UI_Instance.ShowPanel();
    }
}
