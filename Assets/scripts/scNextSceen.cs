using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;

public class scNextSceen : MonoBehaviour
{
    [SerializeField] private string nextSceen;
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool isTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadSceneWithDelay());
        }
    }
    private IEnumerator LoadSceneWithDelay()
    {
        yield return null;
        GameManager.SceneSystem.LoadSceneAsync(nextSceen,SceneTransformType.LoadingScene);
    }
}
