using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem 
{
    private readonly GameManager _game;
    private readonly GameObject prefab;
    private LoadingScene _loading;
    
    private Canvas LodingSceneCanvas;
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
            Debug.LogWarning($"Scene '{name}' 접근불능, 작업정지.");
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
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = true;
        
        while (!op.isDone) yield return null;
        _isLoading = false;
    }
    IEnumerator CallScene_SceneToScene(string name)
    {
        yield return SwapSceneAsync(name,SceneTransformType.Debug);
        _isLoading = false;
    }
   
    private IEnumerator CallScene_LodingScene(string name)
    {
        yield return SwapSceneAsync("LodingScene1",SceneTransformType.Debug);

        yield return SwapSceneAsync(name,SceneTransformType.SceneToScene);
        _isLoading = false;
    }

    IEnumerator FadePanel(FadeType Fade , SceneTransformType transformType)
    {
        _loading = UnityEngine.Object.Instantiate(prefab).GetComponent<LoadingScene>();
        yield return _loading.StartCoroutine(_loading.FadeAction(Fade, transformType));
        
        if(Fade == FadeType.FadeIn)
            DOTween.KillAll();
        else
            _loading.Destroy();
    }
    void FindLodingSceneCanvers()
    {
        LodingSceneCanvas = null;
        LodingSceneCanvas = GameObject.Find("LodingSceneCanvas").GetComponent<Canvas>();
    }
    
    IEnumerator SwapSceneAsync(string targetScene ,SceneTransformType transformType)
    {
        yield return FadePanel(FadeType.FadeIn, transformType);

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false; 

        while (op.progress < 0.9f)
        {
            _loading.UpdatePer((int)(op.progress * 100f), transformType);
            yield return null;
        }
        _loading.UpdatePer(90, transformType);
        
        if(transformType != SceneTransformType.Debug)
        for (int i = 1; i <= 10 ; i++)
        {
            _loading.UpdatePer(90 + i, transformType);
            yield return new WaitForSeconds(0.1f *  UnityEngine.Random.Range(0.5f, 1f)); 
        }
        op.allowSceneActivation = true; 
        while (!op.isDone) yield return null;

        yield return FadePanel(FadeType.FadeOut, transformType);
    }

} 
public enum SceneTransformType
{
    Debug,
    SceneToScene,
    LoadingScene
}