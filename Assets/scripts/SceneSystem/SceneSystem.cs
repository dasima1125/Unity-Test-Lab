using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem 
{
    private readonly GameManager _game;
    private readonly GameObject prefab;
    private LoadingScene _loading;
    
    
    public SceneSystem (GameManager Game)
    {
        prefab = Resources.Load<GameObject>("ExUIs/LoadingPanel");
        _game = Game;
        
    }
    void call()
    {
        _loading = UnityEngine.Object.Instantiate(prefab,GameObject.Find("UI_veiwer").transform,false).GetComponent<LoadingScene>();
    }
    
    public void LoadSceneAsync(string name, Action onComplete = null)
    {
        //call();
        //_loading.debug();
        Debug.Log("신규시스템 진입");
        _game.StartCoroutine(LoadRoutine(name, onComplete));
    }

    // private IEnumerator LoadRoutine(string name, Action onComplete)
    // {
    //     yield return new WaitForSeconds(2);
    //     Debug.Log("씬 로딩 시작");
    //     AsyncOperation op = SceneManager.LoadSceneAsync(name);
    //     op.allowSceneActivation = false;
    //     while (!op.isDone)
    //     {
    //         yield return null;
    //     }
    //     Debug.Log("곧 씬 넘어감");
    //     yield return new WaitForSeconds(2);
    //     op.allowSceneActivation = true;
    //     onComplete?.Invoke();
    // }
    private IEnumerator LoadRoutine(string name, Action onComplete)
    {
        yield return new WaitForSeconds(1); // TODO: fade-in 등

        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = false; 

        while (op.progress < 0.9f)
        {
            Debug.Log($"Loading... {op.progress * 100f}%");
            yield return null;
        }

        Debug.Log("로딩 완료. 전환 준비됨");

        yield return new WaitForSeconds(1); 

        op.allowSceneActivation = true; 

        while (!op.isDone)
        {
            // 로딩이 끝나면 progress 값이 1로 갈 때까지 기다림
            Debug.Log($"Scene Transition Progress: {op.progress * 100f}%");
            yield return null;
        }
        

        onComplete?.Invoke(); // TODO: 씬 전환 후 처리
    }

} 