using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class scTestAction : MonoBehaviour
{
    [SerializeField] public GameObject actionconnect;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject rewardSpPoint;

    [SerializeField] int testmode;

    [Header("pressStack 관련")] // 모드 1로 사용
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [SerializeField] GameObject triggerconnect;
    
    Vector3 originalPos_left; 
    Vector3 originalangle_left;

    Vector3 originalPos_right; 
    Vector3 originalangle_right;

    //내리는건한 -0.1f 돌리는건 한 10도쯤?
    //좌우로좀 0.2씩 옴겨야할듯


    private scPressplate control; 
    private BoxCollider2D trigger; 
    private bool previousImdownState; 

    Vector2 nowPostion;
    Vector2 targetPosition;

    bool life = true;
    bool immove;
    
    void Start() // 반드시 모드 지정된 상태에서 시작되어야함
    {
        switch (testmode)
    {
        case 0:
            control = actionconnect.GetComponent<scPressplate>();
            previousImdownState = control.imdown;
        break;

        case 1:
            trigger= triggerconnect.GetComponent<BoxCollider2D>();
    
            originalPos_left = left.transform.position;
            originalangle_left = left.transform.eulerAngles;
            originalPos_right = right.transform.position;
            originalangle_right = right.transform.eulerAngles;
        break;

        
    }
        

        
        
        nowPostion   = transform.position; // 시작 위치 저장
        targetPosition = nowPostion + new Vector2(0f, 3f); // 옴길 위치

        immove = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(testmode){
            case 0:
                TestAction1();
            break;

            case 1:
                TestAction2();
            break;
        }
        
        
        
      
    }

    void TestAction1()
    {
        if (control.imdown != previousImdownState && !immove) 
        {
           
            if(control.imdown)
            {
                
                StartCoroutine(action2());

            }
            if(!control.imdown)
            {
                
                StartCoroutine(action1());
                
            }

            previousImdownState = control.imdown; 
        }
        



    }
    void TestAction2()//press stack
    {

        Bounds bounds = trigger.bounds;

        // OverlapBox를 사용하여 충돌 여부를 확인합니다.
        Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("player"))
            {
                scAttack player =collider.GetComponent<scAttack>();
                if(player.hardfalling && life)
                {
                    life = false;
                    StartCoroutine(action3(originalPos_left,originalangle_left,originalPos_right,originalangle_right));

                }

                // 여기에서 추가적인 로직을 추가할 수 있습니다.
                break;  // 플레이어를 찾으면 반복문을 종료합니다
            }
        }

        
        

    }
    IEnumerator action1()
    {
        float elapsedTime1 = 0f;
        float resizeDuration = 0.3f;
        immove = true;
        
        
        while (elapsedTime1 < resizeDuration)
        {

            transform.position = Vector2.Lerp(targetPosition, nowPostion, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        immove = false;
        
        
        
        
    }
    IEnumerator action2()
    {
        immove = true;
        float elapsedTime1 = 0f;
        float resizeDuration = 1f; // 애니메이션의 지속 시간

        Vector2 startPosition = nowPostion;
        Vector2 endPosition = targetPosition;

        while (elapsedTime1 < resizeDuration)
        {
            float t = elapsedTime1 / resizeDuration;
            
            float smoothT = Mathf.Pow(t, 3) / (Mathf.Pow(t, 3) + Mathf.Pow(1 - t, 3));
            transform.position = Vector2.Lerp(startPosition, endPosition, smoothT);

            if (Mathf.Abs(t - 0.5f) < 0.01f && life)
            {
                //Debug.Log($"절반 지점 도달: t = {t}, elapsedTime1 = {elapsedTime1}");
            
                if(reward != null)
                {
                    GameObject SpawnedReward = Instantiate(reward, rewardSpPoint.transform.position, Quaternion.identity);
                    Rigidbody2D spawnedRigidbody = SpawnedReward.GetComponent<Rigidbody2D>();
                   
                
                    spawnedRigidbody.AddForce(new Vector2(0,11f) ,ForceMode2D.Impulse);//11 이 적당한거같아
        
                   
                    life = false;
                }
            }

            elapsedTime1 += Time.deltaTime;
            yield return null;
        }

       
        transform.position = endPosition;// 마지막보정

        immove = false;
        
        //가동dad
        //
        
    }
    IEnumerator action3(Vector3 left_pos,Vector3 left_angle,Vector3 right_pos,Vector3 right_angle)
    {
        //내리는건한 -0.1f 돌리는건 한 10도쯤?
        //좌우로좀 0.2씩 옴겨야할듯
        float elapsedTime1 = 0f;
        float resizeDuration = 0.06f; // 애니메이션의 지속 시간
        
        //좌측 구현

        Vector3 targetPos_left   = new Vector3(left_pos.x + 0.2f, left_pos.y - 0.1f); 
        Vector3 targetAngle_left = new Vector3(left_angle.x, left_angle.y, left_angle.z + 15f); 
        
        
        //우측 구현 


        Vector3 targetPos_right  = new Vector3(right_pos.x - 0.1f, right_pos.y - 0.1f); 
        Vector3 targetAngle_right = new Vector3(right_angle.x, right_angle.y, right_angle.z - 15f); 
        //메인 말뚝 
        Vector2 mainStackpos = transform.position;
        Vector2 targetStackpos = mainStackpos - new Vector2(0f,2.3f);
        //-1.8정도? 그쯤?



       
        while (elapsedTime1 < resizeDuration)
        {
            left.transform.position = Vector2.Lerp(left_pos, targetPos_left, elapsedTime1 / resizeDuration);
            left.transform.eulerAngles = Vector3.Lerp(left_angle, targetAngle_left, elapsedTime1 / resizeDuration);

            right.transform.position = Vector2.Lerp(right_pos, targetPos_right, elapsedTime1 / resizeDuration);
            right.transform.eulerAngles = Vector3.Lerp(right_angle, targetAngle_right, elapsedTime1 / resizeDuration);

            transform.position = Vector2.Lerp(mainStackpos, targetStackpos, elapsedTime1 / resizeDuration);
            
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        
    }

}
