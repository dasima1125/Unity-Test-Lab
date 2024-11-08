using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class sc : MonoBehaviour
{
    
    Rigidbody2D rigidbody_me;

    [Header("상호작용 관련")]
        
        [SerializeField] private GameObject connecter;
        [SerializeField] private float itemGrabRange;

    [Header("이동 관련")]
        [SerializeField] private CapsuleCollider2D playercolider;
        [SerializeField] private Transform groundChk;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float move_Speed;
        [SerializeField] public bool isGround;
        [SerializeField] public float myWay = 1;

        private GameObject OnewayPlatform;
        float inputKey;
    [Header("상태 관련")]
        
        [SerializeField] public bool IsWound = false;
        [SerializeField] public bool confusion = false;
        [SerializeField] private Vector2 bumpVector;
        [SerializeField] private float invinclbleTime = 3f;
        [SerializeField] private float confuseTime = 0.5f;



    [Header("점프 관련")]

        [Range (5f,40f)]
        [SerializeField] private float jumpPower = 20f;
        [SerializeField] private float jumpCount = 1;

        [SerializeField] private GameObject wallSenser;
        [SerializeField] private LayerMask wallMask;
        [SerializeField] public bool isWall;


       
        //public bool isWall;
        private bool isJump;
        
        public bool isfalling;
        public float jumpTime;
        private float jumpTimeCounter;


    [Header("대쉬 관련")]
        [SerializeField] private bool isDashing = false;
        [SerializeField] private float dashSpeed = 40f;
        [SerializeField] private float dashTime = 0.4f;
        [SerializeField] private int airDashCount = 1;
        [Range (0f,10f)]
        [SerializeField] private float dashCoolTime;
        [SerializeField] private bool ghosting = false;
        public bool canDashing = true; //퍼블릭으로 변경 대쉬중 죽을때 리젠시 대쉬허용을 켜줘야함

    [Header("배쉬 관련")]
        
        
        [SerializeField] private GameObject BashAbleObj;
        [SerializeField] private GameObject Arrow;
        Vector3 BashDir;
        
        [SerializeField] private float Raduis;
        
        private bool NearToBashAbleObj;
        private bool IsChosingDir;
        private bool IsBashing;
        [SerializeField] private float BashPower;
        [SerializeField] private float BashTime;
    
    
        private float BashTimeReset;
    [Header("애니메이션 테스트 유닛")]
        [SerializeField] private float key;
        Animator  animator;

    [Header("외부 상태연결 노드")]
        [SerializeField] public float key2;
    
    
    
    
    
    

    void Start()
    {
        rigidbody_me = GetComponent<Rigidbody2D>();
        animator     = transform.Find("animation_Test").GetComponent<Animator>();
        
        //배쉬 관련 나중에 개선요망
        BashTimeReset = BashTime;
        
    }

  
    /// <summary>
    /// 센서? 
    /// </summary>
    void Update()
    {
        //Passive Ctrl
        LayerSenser();
        

        //Active Ctrl
        if(!isWall){
            if(!confusion)
            Jump();
        }
        
        Bash();
        wallJump();

        if (Input.GetKeyDown(KeyCode.LeftControl) && airDashCount > 0)
                if(canDashing && !IsWound)
                {
                    
                    scManager insert = connecter.GetComponent<scManager>();
                    if((insert.player_Nowresource-insert.resource_usemode_1)>=0)
                    {
                        insert.player_Nowresource -= insert.resource_usemode_1;
                        insert.insertUtilresource();
                        StartCoroutine(RepeatedlyInvoke_dashEnd(move_dash, dashTime));
                    }
                }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(OnewayPlatform !=null)
            {
                StartCoroutine(DisableCollision());
            }
        }
        //Arrow Ctrl << 임시땜빵으로만해야징
        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    

    /// <summary>
    /// 입력제어
    /// </summary>
    void FixedUpdate()
    {
        Bump();
        if(!IsBashing)
        Move();
    }

    /// <summary>
    /// 이동 메커니즘,
    /// 경사관련은 다 날린상태임 어짜피 데브모드니깐?
    /// </summary>
    void Move()
    {
        //입력값 기입 
        inputKey =Input.GetAxisRaw("Horizontal"); 
        if (inputKey != 0 && !confusion)
            myWay = inputKey;
       
        if(inputKey != 0 && isGround && !isJump  && !confusion)
        {
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("run"))//유후 상태로 복귀
            {
                animator.Play("run",0,0);   
            }
            
        }
       
       
        //좌우이동 메커니즘

        //우측 이동
        if(inputKey == 1 && !confusion && !isWall) {
            rigidbody_me.velocity =new Vector2(inputKey*move_Speed,rigidbody_me.velocity.y);
            
            
        } 
        //좌측이동
        if(inputKey == -1 && !confusion){
            rigidbody_me.velocity =new Vector2(inputKey*move_Speed,rigidbody_me.velocity.y);
        }
        //우휴상태시 
        if (inputKey == 0 && isGround && !isJump && !isDashing && !confusion){
            
            rigidbody_me.velocity = new Vector2(rigidbody_me.velocity.normalized.x * 0.01f,rigidbody_me.velocity.y);
            
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("player_idle 0") && !confusion)
            animator.SetTrigger("idleRetrun");   
            
        }

        //이게 맞는진 모르겠다 논리적으로 좀 이상하다 ...
        if(inputKey > 0 && !confusion){
            transform.eulerAngles = new Vector3(0, 0, 0);
            }
        else if(inputKey < 0 && !confusion){
            transform.eulerAngles = new Vector3(0, 180, 0);
            }
        
        
        
    }
    
    /// <summary>
    /// 점프제어.
    /// 나중에 벽점프도 통합해야될거같은데...
    /// </summary>
    void Jump()
    {
    // 공중에 점프하기
        if (Input.GetKeyDown(KeyCode.Space) && !isGround && jumpCount >= 1 && !isJump){
        
            jumpCount--;
            rigidbody_me.velocity = new Vector2(rigidbody_me.velocity.x, 0f);
            rigidbody_me.AddForce(Vector2.up * 20f, ForceMode2D.Impulse);

            animator.Play("jump",0,0);
            
        }

    
        if (isGround){
            if (isGround && rigidbody_me.velocity.y == 0){
            isJump = false;
            }
            jumpCount = 1;

        }

        // 점프 시작하기
        if (isGround && Input.GetKeyDown(KeyCode.Space)){
            isJump = true;
            jumpTimeCounter = jumpTime;

            animator.Play("jump",0,0);  
            
        }
        

        // 점프 버튼을 누른 채로 더 오래 점프하기
        if (Input.GetKey(KeyCode.Space) && isJump){
            if (jumpTimeCounter > 0){
            rigidbody_me.velocity = new Vector2(rigidbody_me.velocity.x, jumpPower);
            jumpTimeCounter -= Time.deltaTime;  
            } 
        }
        // 점프 버튼을 떼면 점프 멈추기
        if (Input.GetKeyUp(KeyCode.Space)){ 
        isJump = false;
        }

        //추락갱신
        isfalling = false;
        if(rigidbody_me.velocity.y < -4f)
        {
            isfalling =true;

            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
            animator.Play("fall",0,0);
        }

        
    }

    void wallJump()
    {
        if(isWall)
        {
            if(rigidbody_me.velocity.y <0)
            {
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("wallSlide") && !isGround)
                animator.Play("wallSlide",0,0);

                rigidbody_me.velocity=new Vector2(rigidbody_me.velocity.x , -3f);

            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                //여기서 벽타고 계속올라갈려면 점프모델을 두분할시켜야하겠지??
                //    ㄴ 벽방향으로 오면 반동을 좀 줄이고 위로 올라가게 해야겠네 잠만 이러면 벽위치를 가져와야하나 ?
                //         ㄴ 에드포스를 쓰는게좀더 자연스러울려나?
                if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D))
                {
                    Debug.Log("벽점프 - 클라이밍");
                    confusion=true;
                    airDashCount =1;
                    jumpCount = 1;
                    rigidbody_me.velocity =Vector2.zero;
                    rigidbody_me.velocity=new Vector2(-myWay * 8  , 1f * 25 );
                    
                    transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
                    myWay *= -1;

                    animator.Play("jump",0,0);

                }
                else
                {
                    Debug.Log("벽점프 - 리코쉐");
                    confusion=true;
                    airDashCount =1;
                    jumpCount = 1;
                    rigidbody_me.velocity =Vector2.zero;
                    rigidbody_me.velocity=new Vector2(-myWay * 13  , 1f * 15 );
        
                    transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
                    myWay *= -1;

                    animator.Play("jump",0,0);
                }
                
                StartCoroutine(ResetConfusion());
                
            }
            
        }

    }
    /// <summary>
    /// 센서역활, 
    /// 뭐 나중에 다른거 추가하든가
    /// </summary>
    /// 
    void LayerSenser()
    {
        isGround = Physics2D.OverlapCircle(groundChk.position, 0.20f, groundMask);
       
        
        BoxCollider2D wallCollider = wallSenser.GetComponent<BoxCollider2D>();
        isWall = Physics2D.OverlapBox(wallCollider.bounds.center, wallCollider.bounds.size, 0f, wallMask);
        
    
        
        //Debug.DrawRay(new Vector2(transform.position.x,transform.position.y), Vector2.right * myWay * 0.55f, Color.red);

    }


    void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("platform")) 
            OnewayPlatform = collision.gameObject;
        
        //if(collision.gameObject.CompareTag("enemy")) 
        //    gameObject.layer = LayerMask.NameToLayer("player_ghosting");

    }
    void OnCollisionExit2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("platform"))
            OnewayPlatform = null;
        
        
    }
    public void TakeDamage()
    {
        
        BumpActive();

    }
    void Bash()
    {
        RaycastHit2D[] Rays = Physics2D.CircleCastAll(transform.position, Raduis,Vector3.forward);
        
        //배쉬 가능인자 서칭
        foreach(RaycastHit2D ray in Rays)
        {
            NearToBashAbleObj =false; //배쉬 가능인자 서칭을 초기화 
            if(ray.collider.CompareTag("enemy") || ray.collider.CompareTag("bashableObj"))
            {
                //감지될 경우
                NearToBashAbleObj =true;
                BashAbleObj = ray.collider.gameObject;
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && NearToBashAbleObj == true)//눌렀을때 ==>준비상태 또한 범위안에있을때 <== 이거 지정안해서 오류가 터진거구나 
        {
            Time.timeScale = 0;// 시간정지
            Arrow.SetActive(true);// 방향 표시용 화살표 생성
            Arrow.transform.position =BashAbleObj.transform.position;// 위치 동기화
            IsChosingDir = true;// 위치 검색중 상태 온라인

            ghosting     =  true;// 배쉬중 데미지 입으면안되니 일단 고스팅으로 구현함   ㄱ
            gameObject.layer = LayerMask.NameToLayer("player_ghosting"); // 변경 레이어로 애당초 서로 간섭이안일어나야할거같음

            //자연스러운 움직임 개선
            //중력 초기화
            rigidbody_me.gravityScale = 0f;
            
        }
        else if (IsChosingDir && Input.GetKeyUp(KeyCode.Mouse1))
        {
            Time.timeScale = 1;// 시간재개
            Arrow.SetActive(false); // 화살표 다시 비활성화
            IsChosingDir = false;// 대상 검색 종료
            IsBashing    =  true;// 배쉬중임을 선언
            rigidbody_me.velocity = Vector2.zero;// 가속 초기화
            transform.position = BashAbleObj.transform.position; //위치 동기화
            BashDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;//배쉬방향 지정 커서방향을 가져옴

            BashDir= BashDir.normalized; // 배쉬방향 값을 일반화 
            BashDir.z =0;


            Rigidbody2D rb = BashAbleObj.GetComponent<Rigidbody2D>();
            if (rb != null) 
            {   
                rb.velocity = Vector2.zero; // 속도초기화
                rb.AddForce(-BashDir * 30, ForceMode2D.Impulse); // 배쉬방향으로 이동 애드포스로 구현

            }
            
        
        }
        if(IsBashing)
        {
            
            if(BashTime > 0 )
            {
                //이동 출력
                BashTime -= Time.deltaTime;
                rigidbody_me.AddForce(BashDir * BashPower,ForceMode2D.Impulse);
            }
            else 
            {
                ghosting  = false;// 고스팅 상태 회수                     ㄱ
                gameObject.layer = LayerMask.NameToLayer("player"); //레이어 전환법으로 가자
                IsBashing = false;
                BashTime  = BashTimeReset;
                rigidbody_me.gravityScale = 5f;
                if (rigidbody_me.velocity.magnitude > 15) 
                {
                    rigidbody_me.velocity = rigidbody_me.velocity.normalized * 15;
                }        
            }    
        }
    }
    /// <summary>
    /// 적과의 상호작용,
    /// 데미지 입거나 충돌
    /// 아마 개선을 좀해봐야해 
    /// 정리좀 해야하는데..
    /// </summary>
    void Bump()
    {
        Collider2D a1 = GetComponent<Collider2D>();
        Vector2 a2 =a1.bounds.size*1.2f;

        Collider2D[] colliders1 = Physics2D.OverlapCapsuleAll(transform.position,a2,CapsuleDirection2D.Vertical,0f); 
        foreach (Collider2D collider in colliders1)
        { 
            
            if (collider.CompareTag("enemy") && !ghosting && !IsWound)
            {
                //원래는 레이어간 상호작용안할려했는데 이거쓰면 레이어도 마크가 안되는거같네
                scManager   insert        =   connecter.GetComponent<scManager>();
                scEnemy     damageinsert  =      collider.GetComponent<scEnemy>();

                bumpVector = (transform.position - collider.transform.position).normalized;//위치값 저장
                
                insert.insertDeamge(damageinsert.damage);
                BumpActive();
                
            }
            if (collider.CompareTag("items"))
            {
                scThing insert = collider.GetComponent<scThing>();
               
                if(insert != null && insert.catchable) 
                {
                    insert.CanTake_Items();
                }
            
            }
            if(collider.CompareTag("hazardThing")&& !ghosting && !IsWound)
            {
                scManager   insert        =   connecter.GetComponent<scManager>();
                scHazard    damgeinsert   =     collider.GetComponent<scHazard>();

                bumpVector = (transform.position - collider.transform.position).normalized;

                insert.insertDeamge(damgeinsert.damage);
                BumpActive();
            }
            
        }
        Collider2D b1 = GetComponent<Collider2D>();
        Vector2 b2 =b1.bounds.size*itemGrabRange;
        Collider2D[] colliders2 = Physics2D.OverlapCapsuleAll(transform.position,b2,CapsuleDirection2D.Vertical,0f);
        foreach(Collider2D collider in colliders2)
        {

            if (collider.CompareTag("items"))
            {
                Rigidbody2D itemTag  = collider.GetComponent<Rigidbody2D>();
                scThing    itemInfo  = collider.GetComponent<scThing>();
                // 수정들어감
                if(itemInfo !=null  &&itemInfo.canDrag)
                itemTag.velocity = (transform.position - collider.transform.position).normalized*5f; 
              
            }
        }

    }
    void BumpActive()
    {   
        
        Color spriteColor = GetComponent<SpriteRenderer>().color;
        spriteColor.a = 0.2f; 
        GetComponent<SpriteRenderer>().color = spriteColor;

        gameObject.layer = LayerMask.NameToLayer("player_ghosting"); 
        confusion = true;
        IsWound   = true;
        //float inner =bumpVector.x.normalized;
        float inner = Mathf.Sign(bumpVector.x);
        
        
        rigidbody_me.velocity = Vector2.zero;
        
        rigidbody_me.AddForce(new Vector2(inner * 10f, 10f), ForceMode2D.Impulse);
        

        Invoke("BumpSub1",invinclbleTime);
        Invoke("BumpSub2",confuseTime);

    }
    void BumpSub1()
    {

        Color spriteColor = GetComponent<SpriteRenderer>().color;
        spriteColor.a = 1f; 
        GetComponent<SpriteRenderer>().color = spriteColor;

        IsWound = false;
        gameObject.layer = LayerMask.NameToLayer("player"); 


    }
    void BumpSub2()
    {
        confusion = false;
    }
//////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////            코루틴          //////////////////////////////////////////
    IEnumerator move_dash()
    {
        
        rigidbody_me.velocity =new Vector2(dashSpeed*myWay, 0);
        yield return null;

    }
    IEnumerator DisableCollision()
    {
        
        BoxCollider2D platformCollider = OnewayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playercolider,platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playercolider,platformCollider,false);

    }

    IEnumerator ResetConfusion()
    {
        yield return new WaitForSeconds(0.2f);
        confusion = false;
    }

///////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////         코루틴  리피터       ////////////////////////////////////////////
    IEnumerator RepeatedlyInvoke_dashEnd(Func<IEnumerator> func, float duration)//대쉬용
    {
        Color spriteColor = GetComponent<SpriteRenderer>().color;
        Vector2 al=new Vector2(0,0);

        float orignalGravityScale =rigidbody_me.gravityScale;
        float timer = 0f;

        //dash start
        
        isDashing = true;
        confusion = true;
        canDashing =false;
        rigidbody_me.velocity =Vector2.zero;
        rigidbody_me.gravityScale =0f;

        animator.Play("dash");
        
        
        if (ghosting){
            spriteColor.a = 0.2f; 
            GetComponent<SpriteRenderer>().color = spriteColor;
            rigidbody_me.gameObject.layer = LayerMask.NameToLayer("player_ghosting");  
        }
        while (timer < duration)
        {
            if(IsWound)
            {
                timer += duration;
                //insert.insertDeamge(10);
                BumpActive();
                break;
            }
   
            yield return StartCoroutine(func.Invoke());
            timer += Time.deltaTime;
        }

        //dash end
        if (rigidbody_me.velocity.magnitude > 20f) 
            rigidbody_me.velocity = rigidbody_me.velocity.normalized * 20f;
        else
            if(!confusion)
                rigidbody_me.velocity =Vector2.zero;

        if(ghosting) {
            spriteColor.a = 1.0f; 
            GetComponent<SpriteRenderer>().color = spriteColor;
            rigidbody_me.gameObject.layer = LayerMask.NameToLayer("player");
        }
        
        rigidbody_me.gravityScale = orignalGravityScale;
        isDashing =false;
        if(!IsWound)
        confusion = false;
        
        yield return new WaitForSeconds(dashCoolTime);
        canDashing =true;
    }

    //디버그 표시용
    private void OnDrawGizmos()
    {
        
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, Raduis);
        
        //Gizmos.color = Color.yellow;
        Collider2D b1 = GetComponent<Collider2D>();

        if (b1 != null)
        {
            Vector2 b2 = b1.bounds.size * itemGrabRange;
            //Gizmos.DrawWireCube(transform.position, b2);
        }

        //Gizmos.color = Color.green;
        Collider2D a1 = GetComponent<Collider2D>();
        if (a1 != null)
        {
            Vector2 a2 =a1.bounds.size*1.2f;
            //Gizmos.DrawWireCube(transform.position, a2);
        }
  
    
    }
  

}
// ocjp
// ocp
// 정보처리산업기사

//===============================
// 토익 
// 알바 
//    
