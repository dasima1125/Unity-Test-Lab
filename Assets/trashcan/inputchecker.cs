using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputchecker : MonoBehaviour
{
    // Start is called before the first frame update
    public bool check  = true;
    [SerializeField] Sprite on;
    [SerializeField] Sprite off;


    

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void swapBtn()
    {
        if (check)
        GetComponent<Image>().sprite = on;
        if (!check)
        GetComponent<Image>().sprite = off;
    }
}
