using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scPressplate : MonoBehaviour
{

    Vector2 nowPostion;
    Vector2 targetPosition;
    public int mode;
    public bool ispress;
    public bool imdown;
    // Start is called before the first frame update

    [Header("pressStack 관련")]
    [SerializeField] GameObject right;
    [SerializeField] GameObject left;

    Vector2 nowPostion_right;
    Vector2 nowPostion_left;

    //내리는건한 -0.1f 돌리는건 한 10도쯤?
    //좌우로좀 0.2씩 옴겨야할듯
    void Start()
    {
        ispress =false;

        nowPostion   = transform.position; // 시작 위치 저장
        targetPosition = nowPostion - new Vector2(0f, 0.1f); // 내려갈위치

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pressObjSenser();

        
        
    }
    void pressObjSenser()
    {
        BoxCollider2D a1 = GetComponent<BoxCollider2D>();
        Vector2 a2_b = new Vector2(a1.bounds.size.x, a1.bounds.size.y*1.5f);

        ispress = false;
      
        Collider2D[] colliders1 = Physics2D.OverlapBoxAll(a1.transform.position, a2_b, 0f);//위치 크기
        foreach (Collider2D collider in colliders1)
        {
          
            if (collider.CompareTag("player"))
            {    
                ispress = true;
                
                switch (mode)
                {
                    case 0://눌림 감지
                        if(!imdown)
                        { 
                            imdown = true;
                            StartCoroutine(bumper(1));
                        }

                    break;

                    case 1:
                    break;
                    
                    }
            
            }

            

        }
        if (!ispress && imdown)
            {    
                imdown =false;
                //Debug.Log("감압판에서 나감");
                StartCoroutine(bumper(0));
            
            }
        
    }

    IEnumerator bumper(int calltype)//콜타입 1눌린거 0나간거 
    {
         
        float elapsedTime1 = 0f;
        float resizeDuration = 0.1f;
        switch (calltype)
        {
            case 0:
                while (elapsedTime1 < resizeDuration)
                {

                transform.position = Vector2.Lerp(targetPosition, nowPostion, elapsedTime1 / resizeDuration);
                elapsedTime1 += Time.deltaTime;
                yield return null;
                }
                        
            break;
            
            case 1:
                while (elapsedTime1 < resizeDuration)
                {

                transform.position = Vector2.Lerp(nowPostion, targetPosition, elapsedTime1 / resizeDuration);
                elapsedTime1 += Time.deltaTime;
                yield return null;
                }
              
                        
            break;
            
        }
        
        
        
    }

    
}
