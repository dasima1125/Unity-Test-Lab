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
    private TMP_FontAsset originalFont;
    private Color originalRGB;

    [SerializeField] TMP_FontAsset newFont;
    [SerializeField] GameObject Effect_Obj;

    private GameObject Effect_Obj_Adress;
    void Start()
    {
        target       = GetComponent<RectTransform>();
        buttonText   = GetComponentInChildren<TextMeshProUGUI>(); 
        originalFont = buttonText.font;
        originalRGB  = buttonText.color;

    }
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        //초기화
        Debug.Log("진입");
        target.DOKill();
        buttonText.DOKill();
        if (Effect_Obj_Adress != null)
        {
            Effect_Obj_Adress.GetComponent<CanvasGroup>().DOKill();
            Destroy(Effect_Obj_Adress);
        }
        
        //백그라운드 효과
        if (Effect_Obj != null)
        {
           var manager = UImanager.manager;
           //패널 생성
           GameObject BG = Instantiate(Effect_Obj);
           BG.transform.SetParent(manager.canvas.transform, false);
           BG.transform.SetSiblingIndex(transform.GetSiblingIndex());
           var canvas = BG.GetComponent<CanvasGroup>();
           //스케일 컨트롤
           BG.transform.localPosition = gameObject.transform.localPosition; 
           BG.transform.localScale = new Vector2(1, 1.5f);
           //알파컨트롤
           canvas.alpha = 0;
           canvas.DOFade(1f,0.7f).SetEase(Ease.OutQuad);
           //복제된 오브젝트 주소 저장
           Effect_Obj_Adress = BG;           
        }
        
        target.DOScale(new Vector2(scaledsize,scaledsize),transformtime).SetUpdate(true);//타임스케일상 문제가? 있을수도?
        buttonText.DOColor(Color.white,1f).SetEase(Ease.InQuint).SetUpdate(true);
        buttonText.text = buttonText.text.Trim();
        buttonText.font = newFont;    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target.DOKill();
        buttonText.DOKill();

        target.DOScale(new Vector2(defaultsize,defaultsize),transformtime).SetUpdate(true);
        buttonText.font = originalFont;
        if(Effect_Obj_Adress != null)
        {
            var Effect_Obj_target = Effect_Obj_Adress.GetComponent<CanvasGroup>();
            Effect_Obj_target.DOKill();
        
            buttonText.DOColor(originalRGB,0.3f);
            Effect_Obj_target.DOFade(0f,0.3f).SetUpdate(true).OnComplete(() => 
            {
                Destroy(Effect_Obj_Adress);
            });
        
        }
        
    }
    void OnEnable() 
    {
       if(target!= null) target.localScale = new Vector3(defaultsize, defaultsize, 1);
    }
    void OnDisable()
    {
        target.DOKill();
        buttonText.DOKill();
        if (Effect_Obj_Adress != null)
        {
            Effect_Obj_Adress.GetComponent<CanvasGroup>().DOKill();
        }
        
    }
    void OnDestroy() //씬이 바뀌거나 급하게 종료될때 안전하게 모든작업 종료
    {
        //target.localScale = new Vector3(defaultsize, defaultsize, 1);
        target.DOKill();
        buttonText.DOKill();
        if (Effect_Obj_Adress != null)
        {
            Effect_Obj_Adress.GetComponent<CanvasGroup>().DOKill();
        }
    }

}
