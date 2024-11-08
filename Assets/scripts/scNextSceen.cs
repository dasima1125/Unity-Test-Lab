using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class scNextSceen : MonoBehaviour
{
    [SerializeField] private string nextSceen;
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
        
        if (other.CompareTag("player")) // 예: "Player" 태그를 가진 오브젝트와 충돌하면
        {
            Debug.Log("포탈작동");
            SceneManager.LoadScene(nextSceen); // "NextScene"은 이동하고자 하는 씬 이름입니다.
        }
    }
}
