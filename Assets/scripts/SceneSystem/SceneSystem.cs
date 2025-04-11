using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem 
{
    private readonly GameManager _game;  
    public SceneSystem (GameManager Game)
    {
        _game = Game;
    }
    
    public void LoadSceneAsync(string name, Action onComplete = null)
    {
        // MonoBehaviour 쪽에서 대신 코루틴 돌림
        _game.StartCoroutine(LoadRoutine(name, onComplete));
    }

    private IEnumerator LoadRoutine(string name, Action onComplete)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        while (!op.isDone)
        {
            yield return null;
        }
        onComplete?.Invoke();
    }
    
}
