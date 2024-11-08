using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scmeleeAttack : MonoBehaviour
{
    Animator  animator;
    sc scInfo;
    Rigidbody2D rb;
    public int attackCount = 0;  
    public int maxCount = 3;     
    public float exitTime = 0.3f; 

    public float overheat = 0.7f; 
    
    public bool max_combo = false; 
    
//픽셀 15로지정하는게좋을거같네
//스프라이트 렌더러 사이즈 2 5 로 지정 하 돌겠네진짜

    void Start()
    {
        //하위에 animation_Test 를 찾아서 지정
        //animator = GetComponent<Animator>();
        scInfo = GetComponent<sc>();
        rb = GetComponent<Rigidbody2D>();
        animator = transform.Find("animation_Test")?.GetComponent<Animator>();
    }

    void Update()
    {
        
       

    // meleeAttack 상태일 때 로그 남기기
        if(Input.GetKeyDown(KeyCode.E))
        {
            animator.Play("interaction");
            Invoke("interactionDelay",0.3f);
        }
        
        if (Input.GetKeyDown(KeyCode.Z) && attackCount < maxCount && !max_combo && scInfo.isGround && rb.velocity.magnitude < 0.1f )
        {
            animator.ResetTrigger("meleeAttackEnd");
            attackCount++;            
            if (attackCount == maxCount)
            {
                max_combo = true; 
            } 

            if (attackCount == 1 && !max_combo)
            {

                StartCoroutine(Timer());
            }

        }
    }

    private IEnumerator Timer()
    {
        

        animator.Play("player_idle 0");
      
        int loopCount = 0;

        while (attackCount > 0) // 0보다낮으면 종료
        {
            loopCount++;
            if(loopCount == 4) {
                Debug.Log("오버플로우 감지");
                
                // 복구작업 및 코루틴 탈출
                loopCount = 0;
                attackCount = 0;
                animator.SetInteger("attackCount",loopCount);
                break;     
            }
            animator.SetTrigger("meleeAttack");
            animator.SetInteger("attackCount",loopCount);
           
            yield return new WaitForSeconds(exitTime); // 공격애니메이션 출력시간겸 대기
            attackCount--; // 카운터 내리기 
            
        }
        
        // 종료지점
        // 공격 카운트가 0이 되면 최대 콤보 상태 해제
        
        animator.SetTrigger("meleeAttackEnd");
        attackCount = 0;

       
     
    
        if (max_combo)
        {
            Debug.Log("과열");
            Invoke("overheatDelay",overheat);
        } 
  
    }

    void overheatDelay()//과도한 공격입력방지
    {
        Debug.Log("과열해제");
        max_combo = false;

    }
    void interactionDelay()
    {
        animator.Play("player_idle 0");
        
    }
    
}
