using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Call
    { 
        get => FindObjectOfType<LoadingScene>(); 
    }
    CanvasGroup canvasGroup;
    CanvasGroup canvasGroup_Loding;

    TMP_Text loadingSceneTMP;


    void Awake()
    {
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        canvasGroup_Loding = transform.GetChild(1).GetComponent<CanvasGroup>();
    }
    public void UpdatePer(int percent,SceneTransformType type)
    {
        if(type == SceneTransformType.SceneToScene)
        {
            var taget = canvasGroup_Loding.transform.GetChild(1).GetComponent<TMP_Text>();
            taget.text = percent.ToString() + "%";
        }
        else if(type == SceneTransformType.LoadingScene)
        {
            if(loadingSceneTMP == null)
            {
                loadingSceneTMP = transform.GetChild(1).GetComponent<TMP_Text>();
            }
            if(loadingSceneTMP == null)
            {
                return;
            }
            
            loadingSceneTMP.text = percent.ToString() + "%";
        }
    }
    public IEnumerator UpdateInfo()
    {
        string maybe = "Complete";

        loadingSceneTMP = transform.GetChild(2).GetComponent<TMP_Text>();
        while(loadingSceneTMP.text.Length > 0)
        {
            loadingSceneTMP.text = loadingSceneTMP.text.Substring(0, loadingSceneTMP.text.Length - 1);
            yield return new WaitForSeconds(0.015f);

        }
        for(int i = 0; i < 3; i++) 
        {
            yield return new WaitForSeconds(0.5f);
            loadingSceneTMP.text = "_";
            yield return new WaitForSeconds(0.5f);
            loadingSceneTMP.text = "";
        }
        for(int i = 0; i < maybe.Length; i++) 
        {
            loadingSceneTMP.text += maybe[i];
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    public IEnumerator Fade(FadeType type)
    {
        if(canvasGroup == null)
        {
            DestroySelf();
            yield break;
        } 
        gameObject.SetActive(true);
        canvasGroup.alpha = (float)type;
        
        if (type == FadeType.FadeIn)
            yield return canvasGroup.DOFade(1, 1f).WaitForCompletion();
        
        else if(type == FadeType.FadeOut)
            yield return canvasGroup.DOFade(0f, 1f).WaitForCompletion();
    }
    public IEnumerator CallProgressSceneToScene()
    {
        yield return canvasGroup_Loding.DOFade(1, 1f).WaitForCompletion();
    }
    public void DestroySelf()
    {
        if (this == null || gameObject == null) return;
        Destroy(gameObject);
    }
}