using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    private static LoadingScene instance;
    public static LoadingScene Call
    { 
        get => instance ?? (instance = FindObjectOfType<LoadingScene>()); 
    }
    CanvasGroup canvasGroup;
    CanvasGroup canvasGroup_Loding;
    void Awake()
    {
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        canvasGroup_Loding = transform.GetChild(1).GetComponent<CanvasGroup>();
    }
    public void UpdatePer(int percent,SceneTransformType ? type = null)
    {
        if(type == SceneTransformType.SceneToScene)
        {
            var taget = canvasGroup_Loding.transform.GetChild(1).GetComponent<TMP_Text>();
            taget.text = percent.ToString() + "%";
        }
        else if(type == SceneTransformType.LoadingScene)
        {

        }
        
        
    }

    public IEnumerator FadeAction(FadeType type , SceneTransformType SceneToScene)
    {
        gameObject.SetActive(true);
        if(canvasGroup == null)
        {
            Debug.Log("캔버스 증발"); 
            Destroy(gameObject);
            yield break;
        }
        canvasGroup.alpha = (type == FadeType.FadeOut) ? 1f : 0f;
        
        if (type == FadeType.FadeIn)
        {
            yield return canvasGroup.DOFade(1, 1f).WaitForCompletion();
            if(SceneToScene == SceneTransformType.SceneToScene)
                yield return canvasGroup_Loding.DOFade(1, 1f).WaitForCompletion();
            yield break;
        }
        else if(type == FadeType.FadeOut)
        {
            yield return canvasGroup.DOFade(0f, 1f).WaitForCompletion();
            yield break;
        }
    }
    public IEnumerator Fade(FadeType ? type = null)
    {
        if(type == null)
        {
            Destroy();
            yield break;
        } 
            
        gameObject.SetActive(true);
        if(canvasGroup == null)
        {
            Destroy(gameObject);
            yield break;
        }
        canvasGroup.alpha = 0f;
        
        if (type == FadeType.FadeIn)
        {
            yield return canvasGroup.DOFade(1, 1f).WaitForCompletion();
            yield break;
        }
        else if(type == FadeType.FadeOut)
        {
            yield return canvasGroup.DOFade(0f, 1f).WaitForCompletion();
            yield break;
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
public enum FadeType
{
    FadeIn,
    FadeOut
}
