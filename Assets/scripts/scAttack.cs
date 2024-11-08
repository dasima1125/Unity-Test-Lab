using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class scAttack : MonoBehaviour
{

    [Header("세인 관련")]
        [SerializeField] private GameObject sein;
        [SerializeField] private GameObject sein_missle;
        [SerializeField] public float sein_missle_Damage;
        [SerializeField] private float sein_range = 10f;
        [SerializeField] private bool attackAble = false;
        Vector2 enemyPosition;
        //private bool isSeinActive = false;
    [Header("근접공격 관련")]
        [SerializeField] private GameObject player;
        [SerializeField] public float meleeAttackDamage;
        [SerializeField] public bool IsAttack = false;
        public float way;
    [Header("점프공격 관련")]
        [SerializeField] public bool jumpAtk;

        [SerializeField] public bool hardfalling = false;




    //짬통
    Vector2 meleeBoxPosition;
    Vector2 meleeBoxSize;

    Vector2 jumpatkBoxPostion;
    Vector2 jumpatkBoxSize;

    
    float shaketime = 1;
    float shakerange = 0.4f;


    void Start()
    {
        
        jumpAtk = false;
    }

    void Update()
    {
        
        //seinAttackAnlogrism();
        seinAttackAnlogrismBeta();
        meleeAttack();
        JumpAttack();
    
    }
    void FixedUpdate() 
    {

        
        
        
        
        //meleeAttack();
    }
    void meleeAttack()
    {
        sc attack = player.GetComponent<sc>();
        
        meleeBoxPosition = new Vector2(transform.position.x+1f*way,transform.position.y);; // 사각형의 중심 위치
        meleeBoxSize     = new Vector2(2f, 1f); // 사각형의 크기 (가로, 세로)
        way = attack.myWay;

        //공격 선언
        if(Input.GetKeyDown(KeyCode.Z) && !IsAttack && !attack.confusion)
        {
            
            IsAttack       = true;
            attack.enabled = false;            

            Invoke("meleeAttackSub1",0.2f);
            Invoke("meleeAttackSub2",0.075f);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(meleeBoxPosition, meleeBoxSize, 0f);
            
            //공격효과 
            foreach (Collider2D collider in colliders)
            {
                if(collider.CompareTag("enemy"))
                {
                    scEnemy enemy = collider.GetComponent<scEnemy>();
                    enemy.hitPos = transform.position;//공격받은곳 원점을 적 오브젝트 컨트롤 스크립트로 넘김
                    enemy.TakeDamage(meleeAttackDamage,1);
    
                }
                if(collider.CompareTag("attackAble_Unit"))
                {   
                    scThing things = collider.GetComponent<scThing>();
                    
                    things.HitAbleActionThing(0);

                }
            }
        }

    }
    void meleeAttackSub1()
    {
        sc          attack     =          player.GetComponent<sc>();
        Rigidbody2D playerMove = player.GetComponent<Rigidbody2D>();
        
        playerMove.gravityScale = 5;
        playerMove.velocity     =  Vector2.zero;
        attack.enabled = true;
        IsAttack       = false;
    }
    void meleeAttackSub2()
    {
      
        Rigidbody2D playerMove = player.GetComponent<Rigidbody2D>();
        playerMove.velocity     =  Vector2.zero;

    }
    void JumpAttack()
    {
        sc          playerstatus = player.GetComponent<sc>();
        Rigidbody2D playeraction = player.GetComponent<Rigidbody2D>();
        
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        scCamera camera = mainCameraObject.GetComponent<scCamera>();
        
        

        if(jumpAtk)
           if(playerstatus.IsWound) 
            jumpAtk = false;
        
        
        if(!playerstatus.isGround)
        {   
            
            if(Input.GetKey(KeyCode.S) && Input.GetMouseButtonDown(0) && !playerstatus.confusion && !playerstatus.IsWound)
            {
                playerstatus.confusion = true;
                jumpAtk = true;

                playeraction.velocity = Vector2.zero;
                playeraction.gravityScale = 0;
        
                StartCoroutine(JumpAtk());
               
            }

        }
        
        if(hardfalling)
        jumpAtkfalling();

        if(jumpAtk && playerstatus.isGround)
        {
            jumpAtk = false;
            //hardfalling = false;//이것도 리피터좀 줘봐야하나 >>해결됨 근데 맘에는안드네 ,,

            StartCoroutine(camera.shakeCamera(0.1f,0.2f));
            //인보크로 리피터주니 해결
            Invoke("jumpAtkafterwave",0.1f);
        
            if(playerstatus.confusion) //임시구조
                playerstatus.confusion = false;
        
        }
            
    }
    void jumpAtkfalling()
    {
        jumpatkBoxPostion  = new Vector2(transform.position.x,transform.position.y-0.85f);
        jumpatkBoxSize     = new Vector2(1.5f, 0.5f); 
  
        Collider2D[] colliders = Physics2D.OverlapBoxAll(jumpatkBoxPostion, jumpatkBoxSize, 0f);
        foreach (Collider2D collider in colliders)
        {

            if(collider.CompareTag("attackAble_Unit_ground")) 
                {
                    scThing thing = collider.GetComponent<scThing>();
                    thing.HitAbleActionThing(1);


                }
        
        }


    }

    void jumpAtkafterwave()
    {
        hardfalling = false;

        jumpatkBoxPostion  = new Vector2(transform.position.x,transform.position.y-0.85f);
        jumpatkBoxSize     = new Vector2(7, 0.3f); 
        
        
  
        Collider2D[] colliders = Physics2D.OverlapBoxAll(jumpatkBoxPostion, jumpatkBoxSize, 0f);
        foreach (Collider2D collider in colliders)
        {
             
                if(collider.CompareTag("enemy"))
                {
                    scEnemy enemy = collider.GetComponent<scEnemy>();
                    enemy.hitPos = transform.position;
                    enemy.TakeDamage(20,1);
               
                }

                if(collider.CompareTag("ground_breakble")) 
                {
                    scThing thing = collider.GetComponent<scThing>();
                    thing.HitAbleActionThing(1);


                }

                if(collider.CompareTag("attackAble_Unit_Ground_active")) 
                {
                    scThing thing = collider.GetComponent<scThing>();
                    thing.HitAbleActionThing(2);


                }

            }


    }
    void seinAttackAnlogrismBeta()
    {
        bool enemyInRange = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sein_range);
        Vector2 enemyPosition = Vector3.zero; // 초기 위치 설정
        float closestDistance = float.MaxValue;
         // 가장 가까운 적의 거리

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("enemy"))
            {
                enemyInRange = true;
                Vector2 currentEnemyPosition = collider.transform.position;
                float distanceToEnemy = Vector2.Distance(transform.position, currentEnemyPosition);

                // 현재 적이 가장 가까운 적이면 위치와 거리를 
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    enemyPosition = currentEnemyPosition;
                }
            }
        }
        
        if (Input.GetMouseButtonDown(0) && enemyInRange)
        {
           
            GameObject missileInstance = Instantiate(sein_missle,sein.transform.position,quaternion.identity);
            sein_missle missileScript = missileInstance.GetComponent<sein_missle>();
                    
            Vector2 directionToEnemy = (enemyPosition - (Vector2)sein.transform.position).normalized;
            Vector2 oppositeDirection = -directionToEnemy;

            Vector2 newPosition = (Vector2)sein.transform.position + oppositeDirection * 1f;
            sein.transform.position = newPosition;
                    
            if (missileScript != null)
            {
                missileScript.target = enemyPosition;
            }
            
        }

            
        attackAble = enemyInRange;
        
        if (attackAble)
           sein.GetComponent<SpriteRenderer>().color = Color.green;
        else if(!attackAble) 
           sein.GetComponent<SpriteRenderer>().color = Color.white;


    }



    void seinAttackAnlogrism()
    {
        bool enemyInRange = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sein_range);

        foreach (Collider2D collider in colliders)
        {
            //세인 감지 알고리즘
            if (collider.CompareTag("enemy"))
            {

                enemyInRange = true;
                enemyPosition = collider.transform.position;
                

                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(enemyPosition);
                    GameObject missileInstance = Instantiate(sein_missle,sein.transform.position,quaternion.identity);
                    sein_missle missileScript = missileInstance.GetComponent<sein_missle>();
                    
                    Vector2 directionToEnemy = (enemyPosition - (Vector2)sein.transform.position).normalized;
                    Vector2 oppositeDirection = -directionToEnemy;

                    Vector2 newPosition = (Vector2)sein.transform.position + oppositeDirection * 1f;
                    sein.transform.position = newPosition;
                    
                    if (missileScript != null)
                    {
                        missileScript.target = enemyPosition;
                    }
    
                }

            }

        }
        attackAble = enemyInRange;
        
        if (attackAble)
           sein.GetComponent<SpriteRenderer>().color = Color.green;
        else if(!attackAble) 
           sein.GetComponent<SpriteRenderer>().color = Color.white;
    }

//////////////////////디버깅용 기즈모 구획 //////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////
    
    
    private void OnDrawGizmosSelected()
    
    {

        Gizmos.color = Color.green;
     
       Gizmos.DrawWireCube(new Vector2(transform.position.x,transform.position.y-0.85f), new Vector2(1, 0.7f));

       if (IsAttack)
        {
           
           Gizmos.color = Color.yellow;
           Gizmos.DrawCube(meleeBoxPosition, meleeBoxSize);
        }

        
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, sein_range);
        //Gizmos.DrawCube(meleeBoxPosition,meleeBoxSize);
    }
    

//////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////            코루틴          //////////////////////////////////////////
    IEnumerator JumpAtk()
    {
        sc playerstatus = player.GetComponent<sc>();
        Rigidbody2D playeraction = player.GetComponent<Rigidbody2D>();

        float elapsedTime = 0f;
    
        while (elapsedTime < 1/3f)
        {
           
            if(playerstatus.IsWound)
            {
              
                jumpAtk = false;
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                playeraction.gravityScale = 5;
                Debug.Log("피격당함");
                yield break;
            }

            transform.Rotate(0f, 0f, 1080 * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        playeraction.AddForce(Vector2.down * 40f , ForceMode2D.Impulse);
        playeraction.gravityScale = 5;
        hardfalling = true;
 
    }

    



}
