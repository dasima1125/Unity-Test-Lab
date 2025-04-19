using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem 
{
    private const float MaxProgress = 90f;
    private readonly GameManager _game;
    private readonly GameObject prefab;
    private LoadingScene _loading;
    
    private bool _isLoading = false;
    
    
    public SceneSystem (GameManager Game)
    {
        prefab = Resources.Load<GameObject>("ExUIs/SceneSysCanvas");
        _game = Game;
    }
    public void LoadSceneAsync(string name , SceneTransformType type)
    {
        if(_isLoading || !Application.CanStreamedLevelBeLoaded(name))
        { 
            Debug.LogWarning($"Scene '{name}' 접근불능 : 작업중단.");
            return;
        }
        _isLoading = true;
        switch (type)
        {
            case SceneTransformType.Debug:
                _game.StartCoroutine(CallScene_Debug(name));
                return;

            case SceneTransformType.SceneToScene:
                _game.StartCoroutine(CallScene_SceneToScene(name));
                return;

            case SceneTransformType.LoadingScene:
                _game.StartCoroutine(CallScene_LodingScene(name));
                return;

        }
    }

    IEnumerator CallScene_Debug(string name)
    {
        yield return SwapSceneAsync(name);
        _isLoading = false;
    }
    IEnumerator CallScene_SceneToScene(string name)
    {
        yield return FadePanelCreate(FadeType.FadeIn);
        yield return CallProgress(SceneTransformType.SceneToScene);
        yield return SwapSceneAsync(name, SceneTransformType.SceneToScene);
        yield return FadePanelCreate(FadeType.FadeOut);
        _isLoading = false;
    }
   
    private IEnumerator CallScene_LodingScene(string name)
    {
        yield return FadePanelCreate(FadeType.FadeIn);
        yield return SwapSceneAsync("LodingScene1");
        yield return FadePanelCreate(FadeType.FadeOut);
        yield return CallProgress(SceneTransformType.LoadingScene);
        
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            SetProgress(op.progress * 100f, SceneTransformType.LoadingScene);
            yield return null;
        }
        SetProgress(MaxProgress, SceneTransformType.LoadingScene);

        for (int i = 1; i <= 10 ; i++)
        {
            SetProgress(MaxProgress + i, SceneTransformType.LoadingScene);
            yield return new WaitForSeconds(0.1f *  UnityEngine.Random.Range(0.5f, 1f)); 
        }
        yield return _loading.UpdateInfo();
        yield return FadePanelCreate(FadeType.FadeIn);
        
        op.allowSceneActivation = true; 
        while (!op.isDone) yield return null;
        yield return FadePanelCreate(FadeType.FadeOut);

        _isLoading = false;
    }

    IEnumerator FadePanelCreate(FadeType Fade)
    {
        UnityEngine.Object.Instantiate(prefab);
        SetLoadingClass();
        yield return _loading.Fade(Fade);
        
        if(Fade == FadeType.FadeIn)
            DOTween.KillAll();
        else
            _loading.DestroySelf();
        
        yield return null;
    }
    IEnumerator SwapSceneAsync(string targetScene, SceneTransformType type = SceneTransformType.Debug)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false; 

        while (op.progress < 0.9f)
        {
            SetProgress(op.progress * 100f ,type);
            yield return null;
        }
        
        if(type != SceneTransformType.Debug)
        {
            SetProgress(MaxProgress ,type);

            for (int i = 1; i <= 10 ; i++)
            {
                SetProgress(MaxProgress + i ,type);
                yield return new WaitForSeconds(0.1f *  UnityEngine.Random.Range(0.5f, 1f)); 
            }
        }
        op.allowSceneActivation = true; 
        while (!op.isDone) yield return null;
    }
    IEnumerator CallProgress(SceneTransformType type)
    {
        if(type == SceneTransformType.SceneToScene)
            yield return _loading.CallProgressSceneToScene();
        else if(type == SceneTransformType.LoadingScene)
        {
            SetLoadingClass();
            yield break;
        }
    }
    void SetLoadingClass()
    {
        _loading = null;
        _loading = LoadingScene.Call;
    }
    
    
    void SetProgress(float percent, SceneTransformType type = SceneTransformType.Debug)
    {
        int intPercent = (int)percent;
        if (type != SceneTransformType.Debug)
        {
            _loading.UpdatePer(intPercent, type);

        }
            
    }
} 
public enum FadeType
{
    FadeIn,
    FadeOut
}

public enum SceneTransformType
{
    Debug,
    SceneToScene,
    LoadingScene
}