using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnAnimation : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{

    public float defaultsize;
    public float scaledsize;
    public float transformtime;

    private RectTransform target;
    private TextMeshProUGUI buttonText;

    private bool control;

    private TMP_FontAsset originalFont;
    [SerializeField] TMP_FontAsset newFont;
    void Start()
    {
        target = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); 
        originalFont = buttonText.font;
      
    }
   
    private void OnEnable() 
    {
        control = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!control) return; 
        
        target.DOScale(new Vector2(scaledsize,scaledsize),transformtime);
        buttonText.text = buttonText.text.Trim();
        buttonText.font = newFont;

        //buttonText.text += ">";  폰트를 찾아야 해결가능

        
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!control) return; 

        target.DOScale(new Vector2(defaultsize,defaultsize),transformtime);
        //buttonText.text = buttonText.text.Replace(">","");

        buttonText.font = originalFont;

    }
    private void OnDisable() 
    {
        control = false;
        target.DOKill();
        target.localScale = new Vector2(defaultsize, defaultsize); // 크기 초기화
        buttonText.text = buttonText.text.Replace(">", "");  
        buttonText.font = originalFont;
       
    }

    
}
