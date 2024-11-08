using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainmenu_play : MonoBehaviour
{
    public GameObject pause_darkScreen;
    public GameObject Next_darkScreen;
    [SerializeField] CanvasGroup canvasGroupPause;
    [SerializeField] CanvasGroup canvasGroupNext;
    
    // Start is called before the first frame update
    public void playBtn()
    {

        Next_darkScreen.SetActive(true);
        canvasGroupNext.DOFade(1, 0.7f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync("testzoon").completed += (AsyncOperation asyncOperation) =>
            {
        
    
                if(Next_darkScreen == null)
                {
                    //Debug.Log(GameObject.Find("Canvas"));
                    
                    GameObject canvasObject = GameObject.Find("UI_veiwer");
                    Transform childTransform = canvasObject.transform.Find("nextsceen");

                    childTransform.gameObject.SetActive(true);

                    Next_darkScreen = GameObject.Find("nextsceen");
                    canvasGroupNext = Next_darkScreen.GetComponent<CanvasGroup>();
                }
                canvasGroupNext.alpha = 1.0f;

                Sequence fadeOutSequence = DOTween.Sequence();
                
                fadeOutSequence.AppendInterval(0.5f)
                .Append(canvasGroupNext.DOFade(0, 1f).OnComplete(() => 
                {
                    Next_darkScreen.SetActive(false);
                }).SetUpdate(true));

                fadeOutSequence.Play(); // 이게맞나 ㅅㅂ
               
            };
        });
        
    }

    public void load()
    {
        SceneManager.LoadSceneAsync("level1");


    }
    public void pause_btn()
    {
        Time.timeScale = 0;
        
        pause_darkScreen.SetActive(true);
        canvasGroupPause.DOFade(1,0.3f).SetUpdate(true);
        
    }
    public void pause_resume()
    {
        canvasGroupPause.DOFade(0,0.3f).OnComplete(() => 
        {
            pause_darkScreen.SetActive(false);
            Time.timeScale = 1;
        }).SetUpdate(true);

    }

    public void pause_restart()
    {
        Next_darkScreen.SetActive(true);
        Time.timeScale = 1f;
        canvasGroupNext.DOFade(1, 0.7f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name).completed += (AsyncOperation asyncOperation) =>
            {
                if(Next_darkScreen == null)
                {
                    GameObject canvasObject = GameObject.Find("UI_veiwer");
                    Transform childTransform = canvasObject.transform.Find("nextsceen");

                    childTransform.gameObject.SetActive(true);

                    Next_darkScreen = GameObject.Find("nextsceen");
                    canvasGroupNext = Next_darkScreen.GetComponent<CanvasGroup>();
                }
                canvasGroupNext.alpha = 1.0f;

                Sequence fadeOutSequence = DOTween.Sequence();
                
                fadeOutSequence.AppendInterval(0.5f)
                .Append(canvasGroupNext.DOFade(0, 1f).OnComplete(() => 
                {
                    Next_darkScreen.SetActive(false);
                }).SetUpdate(true));

                fadeOutSequence.Play(); // 이게맞나 ㅅㅂ
                
                

               
            };
        });
        

    }
    
    public void option_mute () 
    {
        inputchecker checker = GameObject.Find("sound btn").GetComponent<inputchecker>();

        checker.check = !checker.check;
        checker.swapBtn();
    }

    public void pause_mainMenu()
    {
        Next_darkScreen.SetActive(true);
        Time.timeScale = 1f;
        canvasGroupNext.DOFade(1, 0.7f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync("MainMenu").completed += (AsyncOperation asyncOperation) =>
            {
                if(Next_darkScreen == null)
                {
                    //Debug.Log(GameObject.Find("Canvas"));
                    
                    GameObject canvasObject = GameObject.Find("Canvas");
                    Transform childTransform = canvasObject.transform.Find("nextsceen");

                    childTransform.gameObject.SetActive(true);

                    Next_darkScreen = GameObject.Find("nextsceen");
                    canvasGroupNext = Next_darkScreen.GetComponent<CanvasGroup>();
                }
                canvasGroupNext.alpha = 1.0f;

                Sequence fadeOutSequence = DOTween.Sequence();
                
                fadeOutSequence.AppendInterval(0.5f)
                .Append(canvasGroupNext.DOFade(0, 1f).OnComplete(() => 
                {
                    Next_darkScreen.SetActive(false);
                }).SetUpdate(true));

                fadeOutSequence.Play(); // 이게맞나 ㅅㅂ
                
                

               
            };
        });
        
    }
    public void pause_option()
    {
       
        Next_darkScreen.SetActive(true);
        canvasGroupNext.DOFade(1, 0.3f).SetUpdate(true).OnComplete(() =>
        {
            
            GameObject.Find("fade panel").SetActive(false);//기존 퍼즈화면 내리기        
            GameObject.Find("UI_veiwer").transform.Find("option Panel").gameObject.SetActive(true);//새 패널 교체

    

            Sequence fadeOutSequence = DOTween.Sequence();
            
                
            fadeOutSequence.AppendInterval(0.3f).Append(canvasGroupNext.DOFade(0, 0.3f).OnComplete(() => 
            {
                Next_darkScreen.SetActive(false);

                
            }).SetUpdate(true));

            fadeOutSequence.SetUpdate(true).Play();
         
        });

        
        
    }
    public void pause_return()
    {

        Next_darkScreen.SetActive(true);
        canvasGroupNext.DOFade(1, 0.3f).SetUpdate(true).OnComplete(() =>
        {
            
            GameObject.Find("option Panel").SetActive(false);//기존 옵션화면 내리기       
            GameObject.Find("UI_veiwer").transform.Find("fade panel").gameObject.SetActive(true);//새 패널 교체

    

            Sequence fadeOutSequence = DOTween.Sequence();
            
                
            fadeOutSequence.AppendInterval(0.3f).Append(canvasGroupNext.DOFade(0, 0.3f).OnComplete(() => 
            {
                Next_darkScreen.SetActive(false);

                
            }).SetUpdate(true));

            fadeOutSequence.SetUpdate(true).Play();
         
        });
        
    }

    public void option()
    {
        Debug.Log("옵션 작동");
        Time.timeScale = 0;
        
        pause_darkScreen.SetActive(true);
        canvasGroupPause.DOFade(1,0.3f).SetUpdate(true);
        
    }

    public void option_resume()
    {
        canvasGroupPause.DOFade(0,0.3f).OnComplete(() => 
        {
            pause_darkScreen.SetActive(false);
            Time.timeScale = 1;
        }).SetUpdate(true);
        
    }
    ///페이드인 아웃 효과 
    ///아웃시 가장자리부터 어두워짐 
    ///인 진입시 중앙부터 밝아짐
    ///
   
}
