using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class scEnemy : MonoBehaviour
{

    [Header("몹 종류")]
        [SerializeField] private int mobType;

    [Header("상태")]
        [SerializeField] private float hp = 40f;
        [SerializeField] public int damage;
        [SerializeField] private int way = 1;
        [SerializeField] private float way2 = 0;
        [SerializeField] private bool engageNow = false;
        public Vector2 enemyPos;
        public Vector2 hitPos;
        [SerializeField] public float currentHp;
        public bool isDead;
       
    [Header("임시저장 종류")]
        [SerializeField] private float moveSpeed ;
        [SerializeField] public GameObject patricle;

    [Header("AI 관련")]
        [SerializeField]private bool a1_1 = false;
        [SerializeField]private bool a2_1 = false;
    [Header("쓰로워 관련 관련")]
        [SerializeField] private GameObject throwerBullet;
    

       

    
    Rigidbody2D rigidbody_me;
    SpriteRenderer spriteRenderer;
    
        
    // Start is called before the first frame update
    void Start()
    {
        rigidbody_me   =    GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHp = hp;
        isDead = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    
    }
    void FixedUpdate() 
    {
        if(!engageNow)
        {
            enemyBasicLocgicAI();
        }
        else
        {
            enemyEngageLocgicAI();
        }
    }
    void enemyBasicLocgicAI()
    {
        switch (mobType)
        {
            //테스트 케이스
            //단순추적 
            case 0:
            //정찰구획
                Collider2D[]  groundChk_0     =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x , transform.position.y - 1f)        , new Vector2(0.8f,0.1f) , 0);
                Collider2D[]  patrolCollider_0 =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x + 0.5f *way, transform.position.y - 1f) , new Vector2(0.3f,0.2f) , 0);
                Collider2D[] SightArea_0  = Physics2D.OverlapCircleAll(transform.position,15f);
                //주 작동
                rigidbody_me.velocity =new Vector2( way *2f,rigidbody_me.velocity.y);
              
                foreach (Collider2D collider0_1B in groundChk_0)
                {
                    if(collider0_1B.CompareTag("ground") || collider0_1B.CompareTag("platform"))
                    {
                        if (patrolCollider_0.Length == 0){way *=-1;}
                        
                        foreach (Collider2D collider0_2B in patrolCollider_0)
                        {
                            if(collider0_2B.CompareTag("wall")){way *=-1;}   
                        }
                    }
                }
            //감지구획 
                foreach (Collider2D collider0_3B in SightArea_0)
                {
                    if(collider0_3B.CompareTag("player")){engageNow = true;}
                }
            break;
            
            case 1:

            break;
            
            case 2:
                Collider2D[]  groundChk_2     =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x , transform.position.y - 1f)        , new Vector2(0.8f,0.1f) , 0);
                Collider2D[]  patrolCollider_2 =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x + 0.5f *way, transform.position.y - 1f) , new Vector2(0.3f,0.2f) , 0);
                Collider2D[] SightArea_2  = Physics2D.OverlapCircleAll(transform.position,15f);
                //주 작동
                rigidbody_me.velocity =new Vector2( way *2f,rigidbody_me.velocity.y);
              
                foreach (Collider2D collider2_1B in groundChk_2)
                {
                    
                    if(collider2_1B.CompareTag("ground") || collider2_1B.CompareTag("platform"))
                    {
                        if (patrolCollider_2.Length == 0){way *=-1;}
                        
                        foreach (Collider2D collider2_2B in patrolCollider_2)
                        {
                            if(collider2_2B.CompareTag("wall")){way *=-1;}   
                        }
                    }
                }
            //감지구획 
                foreach (Collider2D collider2_3B in SightArea_2)
                {
                    if(collider2_3B.CompareTag("player"))
                    {
                        engageNow = true;
                        spriteRenderer.color = new Color(1.0f, 0.5f, 0.0f);
                    }
                    
                
                }
            
            break;
            //쓰로워
            case 3:
                Collider2D[]  groundChk_3     =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x , transform.position.y - 1f)        , new Vector2(0.8f,0.1f) , 0);
                Collider2D[]  patrolCollider_3 =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x + 0.5f *way, transform.position.y - 1f) , new Vector2(0.3f,0.2f) , 0);
                Collider2D[] SightArea_3  = Physics2D.OverlapCircleAll(transform.position,15f);
                //주 작동
                rigidbody_me.velocity =new Vector2( way *2f,rigidbody_me.velocity.y);
              
                foreach (Collider2D collider3_1B in groundChk_3)
                {
                    
                    if(collider3_1B.CompareTag("ground") || collider3_1B.CompareTag("platform"))
                    {
                        if (patrolCollider_3.Length == 0){way *=-1;}
                        
                        foreach (Collider2D collider3_2B in patrolCollider_3)
                        {
                            if(collider3_2B.CompareTag("wall")){way *=-1;}   
                        }
                    }
                }
            //감지구획 
                foreach (Collider2D collider3_3B in SightArea_3)
                {
                    if(collider3_3B.CompareTag("player"))
                    {
                        engageNow = true;
                        spriteRenderer.color = new Color(1.0f, 0.5f, 0.0f);
                    }
                    
                
                }
            
            break;
            //쓰로워 알파
            case 4:
                Collider2D[]  groundChk_4     =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x , transform.position.y - 1f)        , new Vector2(0.8f,0.1f) , 0);
                Collider2D[]  patrolCollider_4 =  Physics2D.OverlapBoxAll (new Vector2(transform.position.x + 0.5f *way, transform.position.y - 1f) , new Vector2(0.3f,0.2f) , 0);
                Collider2D[]  SightArea_4  = Physics2D.OverlapCircleAll(transform.position,15f);
                //주 작동
                rigidbody_me.velocity =new Vector2( way *2f,rigidbody_me.velocity.y);
              
                foreach (Collider2D collider4_1B in groundChk_4)
                {
                    
                    if(collider4_1B.CompareTag("ground") || collider4_1B.CompareTag("platform"))
                    {
                        if (patrolCollider_4.Length == 0){way *=-1;}
                        
                        foreach (Collider2D collider4_2B in patrolCollider_4)
                        {
                            if(collider4_2B.CompareTag("wall")){way *=-1;}   
                        }
                    }
                }
                //감지구획 
                foreach (Collider2D collider4_3B in SightArea_4)
                {
                    if(collider4_3B.CompareTag("player"))
                    {
                        engageNow = true;
                        spriteRenderer.color = new Color(1.0f, 0.5f, 0.0f);
                    }
                    
                
                }
            
            break;
          

            
        }
        
    }
    void enemyEngageLocgicAI()
    {
        if(rigidbody_me != null)//사망시 불러오기 취소
        switch (mobType)
        {
            //단순추적
            case 0:
                Collider2D[] ChaseArea0  = Physics2D.OverlapCircleAll(transform.position,20f);
                {
                    
                        foreach (Collider2D collider0_1E in ChaseArea0)
                        {
                            if(collider0_1E.CompareTag("player"))
                            {
                                Vector2 direction = (collider0_1E.transform.position - transform.position).normalized;
                                rigidbody_me.velocity = new Vector2(Mathf.Sign(direction.x)*4f,rigidbody_me.velocity.y);
                            }
                        }
                    }
            break;
            //근접 기본
            case 1:                
                Collider2D[] ChaseArea1  = Physics2D.OverlapCircleAll(transform.position,20f);
                Collider2D[] EngageArea1 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x+1.5f*way2,transform.position.y),new Vector2(2f,2f),0);

                    foreach (Collider2D collider1_1E in ChaseArea1)
                    {
                        if(collider1_1E.CompareTag("player"))
                        {
                            way2 = Mathf.Sign((collider1_1E.transform.position - transform.position).x);
                            if(!a1_1)
                            rigidbody_me.velocity = new Vector2(way2*4f,rigidbody_me.velocity.y);
                        }
                    }
                    foreach (Collider2D collider1_2E in EngageArea1)
                    {
                        if(collider1_2E.CompareTag("player"))
                        {
                            a1_1 =true;
                        }
                    }
                    if (a1_1)
                    {   
                        if(!a2_1)
                        {   rigidbody_me.velocity= Vector2.zero;
                            a2_1 =true;
                            StartCoroutine(AttackAfterDelay());
                        }
                    }
            break;
            //슬라임
            case 2:
                Collider2D[] ChaseArea2  = Physics2D.OverlapCircleAll(transform.position,20f);
                Collider2D[] EngageRange2 = Physics2D.OverlapCircleAll(transform.position,10f);
                    foreach (Collider2D collider2_1E in ChaseArea2)
                    {
                        //range out
                        //if(ChaseArea2 == null)
                            //engageNow =false;

                        if(collider2_1E.CompareTag("player"))
                        {
                            way2 = Mathf.Sign((collider2_1E.transform.position - transform.position).x);
                            if(!a1_1)
                            rigidbody_me.velocity = new Vector2(way2*4f,rigidbody_me.velocity.y);
                        }
                    }
                    foreach (Collider2D collider2_2E in EngageRange2)
                    {
                        if(collider2_2E.CompareTag("player"))
                        {
                            a1_1 =true;
                        }
                    }
                    if (a1_1)
                    {   
                        
                        if(!a2_1)
                        {   rigidbody_me.velocity= Vector2.zero;
                            a2_1 =true;
                            StartCoroutine(AttackAfterDelay());
                        }
                    }

            break;
            //쓰로워
            case 3:
                Collider2D[] ChaseArea3  = Physics2D.OverlapCircleAll(transform.position,20f);
                Collider2D[] EngageRange3 = Physics2D.OverlapCircleAll(transform.position,20f);
                    foreach (Collider2D collider3_1E in ChaseArea3)
                    {

                        if(collider3_1E.CompareTag("player"))
                        {
                            way2 = Mathf.Sign((collider3_1E.transform.position - transform.position).x);
                            if(!a1_1)
                            rigidbody_me.velocity = new Vector2(way2*4f,rigidbody_me.velocity.y);
                        }
                    }
                    foreach (Collider2D collider3_2E in EngageRange3)
                    {
                        if(collider3_2E.CompareTag("player"))
                        {
                            a1_1 =true;
                        }
                    }
                    if (a1_1)
                    {   
                        
                        if(!a2_1)
                        {   rigidbody_me.velocity= Vector2.zero;
                            a2_1 =true;
                            if(!isDead)
                            StartCoroutine(AttackAfterDelay());
                        }
                    }
            break;
            //쓰로워 알파
            case 4:
                Collider2D[] ChaseArea4  = Physics2D.OverlapCircleAll(transform.position,20f);
                Collider2D[] EngageRange4 = Physics2D.OverlapCircleAll(transform.position,15f);
                foreach(Collider2D collider4_1E in ChaseArea4)
                {
                    if(collider4_1E.CompareTag("player"))
                    {
                        if(collider4_1E.CompareTag("player"))
                        {
                            way2 = Mathf.Sign((collider4_1E.transform.position - transform.position).x);
                            if(!a1_1)
                            rigidbody_me.velocity = new Vector2(way2*4f,rigidbody_me.velocity.y);
                        }
                    }
                }
                foreach (Collider2D collider4_2E in EngageRange4)
                {
                    if(collider4_2E.CompareTag("player"))
                    {
                        a1_1 =true;
                    }
                }
                if (a1_1)
                {                   
                    if(!a2_1)
                    {   rigidbody_me.velocity= Vector2.zero;
                        a2_1 =true;
                        if(!isDead)
                        StartCoroutine(AttackAfterDelay());
                    }
                }            
            break;
        }

    }
    
    public void TakeDamage(float damageAmount , int type)
    { 
        if (!isDead)
        {
            currentHp -= damageAmount;

            //루트 1  
            switch (type)
            {
                case 0:
                break;
                case 1://근접공격
                    
                    Debug.Log("근접 피격");
                    
                    rigidbody_me.velocity = Vector2.zero;
                    
                    if(transform.position.x - hitPos.x >0)
                       rigidbody_me.velocity = new Vector2(1, 10f);    
                    
                    if(transform.position.x - hitPos.x <0)
                       rigidbody_me.velocity = new Vector2(-1, 10f);

                    break;
                case 2://미사일
                    
                    rigidbody_me.velocity = Vector2.zero;
                    rigidbody_me.AddForce(((Vector2)transform.position - hitPos).normalized, ForceMode2D.Impulse);

                    break;
            }
            if (currentHp <= 0)
            {
                currentHp = 0;
                Death();
            }
        }
    }
    
    void Death()
    {
        isDead = true;
        
        particleUnit();
        
    }
    //사망 표현
    void particleUnit()
    {
        int debrisCount = 35;
        GameObject[] objects = new GameObject[debrisCount];
        for (int i = 0; i < debrisCount; i++)
        {
           // 초기설정
            objects[i] = new GameObject("debris")
            {
                layer = LayerMask.NameToLayer("particle"),
                tag   = "debris"
            };
        
            SpriteRenderer spriteRenderer           = objects[i].AddComponent<SpriteRenderer>();
            SpriteRenderer currentSpriteRenderer    = patricle.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = currentSpriteRenderer.sprite;
     
            objects[i].transform.localScale = new Vector2(Random.Range(0.05f, 0.15f), Random.Range(0.05f, 0.15f));
            objects[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            objects[i].transform.position = transform.position;
      
            Rigidbody2D rb = objects[i].AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;

            objects[i].AddComponent<BoxCollider2D>();
            float explodePower = 4;
            rb.velocity = new Vector2(Random.Range(-explodePower,explodePower),Random.Range(-explodePower,explodePower));
            
            StartCoroutine(debriskiller(objects[i]));
        }
        //오브젝트비활성화
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Rigidbody2D>());
        
    }
    IEnumerator debriskiller(GameObject debris)
    {
        float delayTime = Random.Range(1.5f, 3f);
        yield return new WaitForSeconds(delayTime);

        Destroy(debris);
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    
    }

    IEnumerator AttackAfterDelay()
    {   
        switch (mobType)
        {   
            //테스트
            case 0:
            break;
            
            //기본 공격 로직
            case 1:
                Collider2D[] attackRange1 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x+1.5f*way2,transform.position.y),new Vector2(2f,2f),0);
            
                yield return new WaitForSeconds(1f);
                // attackRange 범위안에 있는 적에게 데미지 부여
                yield return new WaitForSeconds(1f);
                a1_1 = false;
                a2_1 = false;
            break;

            //슬라임
            case 2:
                float C1_2 = 0;
                
                spriteRenderer.color = new Color(1.0f, 0.5f, 0.0f);
                spriteRenderer.color = Color.red;
                
                yield return new WaitForSeconds(1f);
                
            
                Collider2D[] attackRange2 = Physics2D.OverlapCircleAll(transform.position,10f);
                foreach (Collider2D collider2_1C in attackRange2)
                {
                    if(collider2_1C.CompareTag("player")&&!isDead)
                    C1_2 = collider2_1C.transform.position.x - rigidbody_me.position.x;
                }

                if(C1_2 == 0)
                {
                    a1_1 = false;
                    a2_1 = false;
                    break;
                }
                rigidbody_me.AddForce(new Vector2(C1_2,25),ForceMode2D.Impulse);
                
                yield return new WaitForSeconds(1f); 
                a1_1 = false;
                a2_1 = false;
                
            
            break;
            //쓰로워
             case 3:
                yield return new WaitForSeconds(1f);
                Collider2D[] attackRange3 = Physics2D.OverlapCircleAll(transform.position, 23f);
    
    // aim 배열을 한 번 생성하여 재사용
                Collider2D[] aim2 = Physics2D.OverlapCircleAll(transform.position, 23f);
    
                foreach (Collider2D collider3_1C in attackRange3)
                {
                    if (collider3_1C != null && collider3_1C.CompareTag("player"))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            foreach (Collider2D locker in aim2)
                            {
                                if (locker != null && locker.CompareTag("player"))
                                {
                                    float C1_3 = locker.transform.position.x - rigidbody_me.position.x;
                                    GameObject thorwing = Instantiate(throwerBullet);
                                    Rigidbody2D throwingRigid = thorwing.GetComponent<Rigidbody2D>();
                                    thorwing.transform.position = transform.position;
                                    throwingRigid.AddForce(new Vector2(C1_3, 27), ForceMode2D.Impulse);
                                }
                                else
                                {
                                    Debug.Log("몰라레후");
                                }
                            }
                
                            yield return new WaitForSeconds(0.3f);
                        }
                    }
                }
    
                Debug.Log("끝");
                yield return new WaitForSeconds(1f);
                a1_1 = false;
                a2_1 = false;
            break;

            //쓰로워 알파
            case 4:

                yield return new WaitForSeconds(1f);
                
                Collider2D[] attackRange4 = Physics2D.OverlapCircleAll(transform.position,23f);
                foreach (Collider2D collider4_1C in attackRange4)
                {
                    if(collider4_1C != null && collider4_1C.CompareTag("player")&&!isDead)
                    {
                        Collider2D[] aim = Physics2D.OverlapCircleAll(transform.position,23f);
                        foreach (Collider2D locker in aim)
                        {
                            if(locker.CompareTag("player")&&!isDead)//isdead논리식 없으면 이부분에서 오브젝트 미스 오류 발견 증상발현원리 알아야할거같아
                            {
                                    float C1_4 = locker.transform.position.x - rigidbody_me.position.x;
                                    
                                    bool targetright = (C1_4 > 0) ? true : false;
                                    for(int i = -1; i < 2; i++) {
                                
                                    if(targetright)
                                    {
                                        float angleInRadians = (45 + (30*i)) * Mathf.Deg2Rad; 
                                        GameObject throwing1 = Instantiate(throwerBullet);
                                        Rigidbody2D throwingRigid = throwing1.GetComponent<Rigidbody2D>();
                                        
                                        throwingRigid.gravityScale =3f;
                                        throwing1.transform.position =transform.position;
                                        
                                        float xForce = Mathf.Cos(angleInRadians) * 20;
                                        float yForce = Mathf.Sin(angleInRadians) * 20;
                                        
                                        throwingRigid.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
                                    }
                                    else
                                    {
                                        float angleInRadians = (180 - 45 - (30*i)) * Mathf.Deg2Rad;
                                        GameObject throwing1 = Instantiate(throwerBullet);
                                        Rigidbody2D throwingRigid = throwing1.GetComponent<Rigidbody2D>();
                                        
                                        throwingRigid.gravityScale =3f;
                                        throwing1.transform.position =transform.position;
                                        
                                        float xForce = Mathf.Cos(angleInRadians) * 20;
                                        float yForce = Mathf.Sin(angleInRadians) * 20;
                                        
                                        throwingRigid.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
                                    
                                    }
                                                                            
                                }

                            }
                        }
                            
                        yield return new WaitForSeconds(2f); 
                    }
                    
                }
                Debug.Log("끝");
                yield return new WaitForSeconds(1f); 
                a1_1 = false;
                a2_1 = false;
            
            break;

            
            
        }
         
    }
    //사망 표현 종료
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hitPos, 0.5f);
    
    }
}
