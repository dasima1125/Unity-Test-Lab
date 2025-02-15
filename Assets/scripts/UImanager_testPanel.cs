using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager_testPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public Button testBtn;
    void Start()
    {
        testBtn.onClick.AddListener(OnShowButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnShowButtonClicked()
    {
        UImanager.UI_Instance.HidePanel();
    }
}
