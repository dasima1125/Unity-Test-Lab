using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class scThing : MonoBehaviour
{
    Rigidbody2D rigidbody_me;
    [Header("상호 관련")]
        
        [SerializeField] private GameObject playerManager;
    
    [Header("오브젝트 정보 관련")]
        
        [SerializeField] public bool catchable;
        [SerializeField] public bool canDrag;
        [SerializeField] public int item;

        [SerializeField] public int sizeX;
        [SerializeField] public int sizeY;
        
    [Header("유닛 정보 관련")]
        [SerializeField] private float hp;
        [SerializeField] private float currentHp;
        [SerializeField] private float invinclbleTime;
        private bool isDead;
    [Header("NPC 오브젝트 정보")]
        [SerializeField] private bool iminner;
       
        bool endtime;
        bool Groundinteraction = true;


    [Header("NPC 오브젝트 참조용(문이나 이벤트 트리거 용도임)")]
        [SerializeField] public GameObject object1;
        [SerializeField] public GameObject object2;
        [SerializeField] public GameObject object3;
        [SerializeField] public GameObject object4;        
        [SerializeField] public GameObject reward;
        [SerializeField] public int needkey;




        private Color originalColor; // 원래 색상 저장
        private SpriteRenderer spriteRenderer;
        

    // Start is called before the first frame update
    void Awake() 
    {
        if(playerManager == null){
            playerManager = GameObject.Find("game Manager"); //프리팹 사용용도
            
            //ArgumentNullException: Value cannot be null. Parameter name: _unity_self
            //이오류 계속발생 해결방법은 시작할때 아무것도 건들지말고하라는데 ....왜그러지 뭐가잘못된거같기도 reward 프리팹넣을때생긴거같아 아마
            //
            //스크립트가 있는 GameObject가 선택되었을 때 발생하는 듯합니다(다른 사용자가 제안한 대로). 또한 재생을 누른 후 표시된 오류 메시지의 수가 특정 선택된 GameObject가 스크립트 구성 요소 아래에 가지고 있는 [SerializedField]의 수와 동일하다는 것을 알아챘습니다(우연일 수도 있음). 
            //"[SerializedField]가 틀렸을 수도 있지만, 제가 말하는 것은 편집하고 드래그 앤 드롭할 수 있는 모든 곳을 말합니다.
            //
            //라고 하는데요?
        }
    }
    void Start()
    {
        rigidbody_me= GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        
        

        //currentHp = hp;
        isDead  = false;
        endtime = false;
        iminner = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (item == 3 || item == 4)
        {
            NPCdetect();
        }
       
       
    }
    private void FixedUpdate() 
    {
        if(!isDead && CompareTag("bashableObj"))
        caution();
    }
    void caution()
    {
        BoxCollider2D size = GetComponent<BoxCollider2D>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size.bounds.size, 0f);
        foreach (Collider2D collider in colliders)
        {
                if(collider.CompareTag("wall"))
                {
                    Rigidbody2D box = GetComponent<Rigidbody2D>();
                    if(box.velocity.magnitude >= 25)
                    {
                       
                        isDead = true;
                        //Time.timeScale = 0;
                        HitAbleActionThing_end();
                       
                    }
                    
               
                }

                

            }

    }

    public void HitAbleActionThing(int type) 
    {
        switch (type)
        {
            case 0://근접공격
            Color spriteColor = GetComponent<SpriteRenderer>().color;
    
            spriteColor.a = 0.2f; 
            GetComponent<SpriteRenderer>().color = spriteColor;
            hp -= currentHp;
            Invoke("HitAbleActionThing_sub1",1);
            break;

            case 1://찍기공격
            hp = 0;
            //보상관련 존재할수도있으니깐 리워드 메소드를  준비해야할려나 ?
            break;
            
            case 11://찍기공격 파괴되는 오브젝트 
            hp = 0;
            //
            
            
            break;

            case 2://찍기 상호작용
            //흔들거나 뭐 아무런 작용하면되것지뭐 
            if(Groundinteraction) 
            {
                Groundinteraction = false;
                GroundAttackinteraction();
            }
            
            break;

            case 3://베쉬관련
            break;
        }

        if(hp <= 0)
        {
            isDead = true;
            HitAbleActionThing_end();   
        }
        
        Invoke("HitAbleActionThing_sub1",1);

    }
    void HitAbleActionThing_sub1()
    {
        if(!isDead)
        {
        Color spriteColor = GetComponent<SpriteRenderer>().color;
        spriteColor.a = 1f; 
        GetComponent<SpriteRenderer>().color = spriteColor;
        }

    }
    void HitAbleActionThing_end()
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
           
            SpriteRenderer spriteRenderer = objects[i].AddComponent<SpriteRenderer>();
            SpriteRenderer currentSpriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = currentSpriteRenderer.sprite;
     
            objects[i].transform.localScale = new Vector2(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f));
            objects[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            objects[i].transform.position = transform.position;
      
            Rigidbody2D rb = objects[i].AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;

            objects[i].AddComponent<BoxCollider2D>();
            float explodePower = 4;
            rb.velocity = new Vector2(Random.Range(-explodePower,explodePower),Random.Range(-explodePower,explodePower));
            
            //objects[i]의 데브리 삭제 코루틴 실행
            StartCoroutine(HitAbleDebriskiller(objects[i]));
        }
        //오브젝트비활성화
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Rigidbody2D>());
        
    }
    void GroundAttackinteraction()
    {
        Debug.Log("작동함");
        StartCoroutine(Shake());

    }

    [SerializeField] private string itemName;
    [SerializeField] private int itemQuantity;
    [SerializeField] private Sprite sprite;   

    [TextArea]
    [SerializeField] private string itemDescription;

    
    public void CanTake_Items() 
    {
        
        switch(item) 
        {   //회복 아이템 
            case 0:
                //되도록 이부분은 함수내에서 처리해야할거같아
                scManager insert0= playerManager.GetComponent<scManager>();

                insert0.player_NowHealth += 25;
                insert0.hp.fillAmount = insert0.player_NowHealth / insert0.player_Health;
        
                if(insert0.player_NowHealth > insert0.player_Health)
                insert0.player_NowHealth = insert0.player_Health;

                string [] set1 ={"체력" , "+ 25"};

                UImanager.manager.ShowItemInfo(set1);
                // 아이템 얻는 로직 //
                int leftoverItme = InventoryManager.Inventory.Add(itemName,itemQuantity,sprite,itemDescription);
                
                if(leftoverItme <= 0)
                {
                    Destroy(gameObject);
                } 
                else
                {
                    itemQuantity = leftoverItme; //이거 없어될거같은데 << ㄴㄴ있어야함
                    //Debug.Log("남은 수량 : "+leftoverItme);
                }
                    
                
                //
            break;
            //탄약 아이템
            case 1:
                //되도록 이부분은 함수내에서 처리해야할거같아
                scManager insert1= playerManager.GetComponent<scManager>();
                insert1.player_Nowresource += 15;
                insert1.rp.fillAmount = insert1.player_Nowresource / insert1.resource;
        
                if(insert1.player_Nowresource > insert1.resource)
                insert1.player_Nowresource = insert1.resource;

                string [] set2 ={"탄약" , "+ 15"};

                UImanager.manager.ShowItemInfo(set2);
                Destroy(gameObject);
            break;
            //키 아이템
            case 2:
                Debug.Log("키");
                scManager insert2= playerManager.GetComponent<scManager>();
                

                insert2.key_count += 1;
                Debug.Log(insert2.key_count);
                gameObject.SetActive(false);
  
            break;
            
            case 3:
                Debug.Log("오브젝트1");
            break;
            //미정 오브젝트 문용으로 테스트 시작
            case 4:
            Debug.Log("오브젝트 2");
            break;

            
        }
        
    }

    void NPCdetect()
    {
        scManager keycount= playerManager.GetComponent<scManager>();

        iminner = false;
        Collider2D a1 = GetComponent<Collider2D>();
        Vector2 colliderSize = a1.bounds.size;
        Vector2 detectionSize = new Vector2(colliderSize.x * sizeX, colliderSize.y * sizeY);
        Vector2 detectionCenter = (Vector2)transform.position - new Vector2(1, 0);//이동용나중에 이값도 올려야지 
        Collider2D[] colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, 0f);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("player")&& !endtime)//door
            {
                iminner = true;
                if(keycount.key_count >= needkey) {
                    spriteRenderer.color = Color.green;
                    if (Input.GetKeyDown(KeyCode.E) && keycount.key_count >= needkey)
                    {
                        keycount.key_count -= needkey;
                        iminner = false;
                        endtime = true;
                        StartCoroutine(MoveToTarget());
                    }
                }
                if(keycount.key_count < needkey)
                    spriteRenderer.color = Color.red;
                
            }       
        }
        if(!iminner)
        spriteRenderer.color = Color.white; 
    }




//////////////////////디버깅용 기즈모 구획 //////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////
private void OnDrawGizmos()
{
    Gizmos.color = Color.green;

    Collider2D a1 = GetComponent<Collider2D>();
    if (a1 != null)
    {
        Vector2 colliderSize = a1.bounds.size;
        Vector2 gizmoSize = new Vector2(colliderSize.x * sizeX, colliderSize.y * sizeY);
        Vector2 gizmoCenter = (Vector2)transform.position - new Vector2(1, 0);

        Gizmos.DrawWireCube(gizmoCenter, gizmoSize);
    }
    
  
}

////////////////////////////////////////코루틴////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator HitAbleDebriskiller(GameObject debris)
    {
        float delayTime = Random.Range(1.5f, 3f);
        yield return new WaitForSeconds(delayTime);

        // 데브리 삭제
        Destroy(debris);
        // 오브젝트 삭제
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    
    }

    IEnumerator MoveToTarget()
    {
        float elapsedTime = 0f;
        Vector2 upPostion = object1.transform.position + new Vector3(0, 3, 0);
        Vector2 downPostion = object2.transform.position + new Vector3(0, -3, 0);
        while (elapsedTime < 5)
        {
            object1.transform.position  = Vector3.Lerp(object1.transform.position,upPostion   ,elapsedTime / 25);
            object2.transform.position  = Vector3.Lerp(object2.transform.position,downPostion ,elapsedTime / 25);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    ////////////////////////////////////////테스트 코루틴////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator Shake()
    {
        // 하드코딩된 흔들림 설정 값
        float shakeAmount = 0.3f; // 흔들림의 크기
        float shakeSpeed = 40f;  // 흔들림의 속도
        float duration = 0.5f;   // 흔들림의 지속 시간

        Vector3 originalPosition = transform.position; // 초기 위치를 저장합니다.
        float elapsedTime = 0f; // 경과 시간을 추적합니다.

        while (elapsedTime < duration)
        {
            // 흔들림의 offset을 계산합니다.
            float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

            // 현재 위치를 원래 위치에서 흔들림 offset을 추가하여 설정합니다.
            transform.position = new Vector3(originalPosition.x + shakeOffset, originalPosition.y, originalPosition.z);

            elapsedTime += Time.deltaTime; // 경과 시간을 업데이트합니다.
            yield return null; // 다음 프레임까지 대기합니다.
        }
        Debug.Log("작동끝 보상대기");
        Instantiate(reward, transform.position + new Vector3(0, 4, -0.015f), transform.rotation);
    
    }
}


