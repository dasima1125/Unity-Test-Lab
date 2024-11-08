using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class scInteraction : MonoBehaviour
{
    
    
    private bool life;
    public bool pushed;

    bool ispushing;
    //maybe size
    //down w 3.6   h 0.1
    //up   w 1.9   h 1.4
    //언젠간바꿔야지뭐 

    // Start is called before the first frame update

    public float resizeDuration; 
    public Vector2 targetScale = new Vector2(3.6f, 0.1f);

    [SerializeField] GameObject smashTriger;

    public int mode;

    void Start() //모드 2는 상당히 불안정함 차후 격리 예정
    {
     
        life = true;
        pushed = false;
        ispushing = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == 2)
        {
            mode2sector(); //ㅈ같네진짜 왜 케이스문에서 작동을 안하는거야 타이밍문제인가 ,.

        }
        
        
        
    }

    void FixedUpdate()
    {
        pressObjSenser();

    }

    void pressObjSenser()
    {
        BoxCollider2D a1 = GetComponent<BoxCollider2D>();
        Vector2 a2_b = new Vector2(a1.bounds.size.x*0.8f, a1.bounds.size.y*1.1f);

        Collider2D[] colliders1 = Physics2D.OverlapBoxAll(a1.transform.position, a2_b, 0f);//위치 크기
        foreach (Collider2D collider in colliders1)
        {
            if (!collider.CompareTag("player"))
            {
                //pushed = false;
            }

            if (collider.CompareTag("player"))
            {    
                sc              info        = collider.GetComponent<sc>();
                Rigidbody2D     info_riged  = collider.GetComponent<Rigidbody2D>();
             
                
                if (info.isfalling)
                {
                    switch (mode)
                    {
                        case 0://밝으면 터짐
                        if(life) 
                        {
                            life = false;
                            a1.enabled = false;
                            StartCoroutine(bumper());
                        }
    
                        break;

                        case 1://점프패드 
                        if(life) 
                        {
                            life = false;
                            
                            StartCoroutine(jumper(info,info_riged));
                        }
                        break;
                        
                        case 2://하강공격에 터짐 아니 왜 자꾸 움직일때마다 쳐 falling 작동하는거야 ..
                        //        ㄴ 좌우이동시 작동을 안하게 해야하나 ,, 근데이러면 또문제인데  하 모르겠네 
                        //              ㄴ밑쪽방식으로 안정성은 늘리긴햇는데 ..쩝
                        break;
                        
                        case 3:
                        break;

                    }
                    
                }
                
                
                
            }
            
            
        }

    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("player")&& mode == 2)
        {
            sc info = collision.collider.GetComponent<sc>();
            if(!pushed && !ispushing && life && info.isfalling)
                            {
                                pushed = true;
                                ispushing = true;
                                StartCoroutine(lowbump());

                            }
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("player"))
        {
            pushed = false;

        }
        
    }
    
    void mode2sector()
    {
        BoxCollider2D boxCollider = smashTriger.GetComponent<BoxCollider2D>();
        Bounds bounds = boxCollider.bounds;

        // OverlapBox를 사용하여 충돌 여부를 확인합니다.
        Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("player"))
            {
                scAttack player =collider.GetComponent<scAttack>();
                if(player.hardfalling)
                {
                    boxCollider.enabled = false;
                    life = false;

                    Debug.Log("찍기공격 감지");
                    StartCoroutine(bumper());
    
                }
              
            }
        }

    }


    private IEnumerator bumper()
    {
        Vector3 initialPosition = transform.position; // 시작 위치 저장
        Vector2 initialScale = transform.localScale; // 현재 크기 저장
        //내장값으로 돌려야하나 가끔가다 이해가안가네 
        
        float elapsedTime1 = 0f;
        float elapsedTime2 = 0f;
        
        float Duration = resizeDuration;

        Debug.Log(Duration);
        while (elapsedTime1 < Duration)
        {
            transform.localScale = Vector2.Lerp(initialScale, targetScale, elapsedTime1 / Duration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }        
        while (elapsedTime2 < 2)
        {
           
            transform.position = Vector3.Lerp(initialPosition,initialPosition - new Vector3(0f, 0.5f,-1f), elapsedTime2 / 10);
            elapsedTime2 += Time.deltaTime;
            yield return null;
        }
        
        
        gameObject.SetActive(false);//종료
    }
    IEnumerator lowbump()
    {

        Vector2 targetScale_low = new Vector2(2.2f, 0.8f);
        Vector2 initialScale = transform.localScale;

        float elapsedTime1 = 0f;

    
        while (elapsedTime1 < 0.07f)
        {
            transform.localScale = Vector2.Lerp(initialScale, targetScale_low, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        elapsedTime1 = 0f;
        while (elapsedTime1 < 0.08f)
        {
            transform.localScale = Vector2.Lerp(targetScale_low, initialScale, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        while (elapsedTime1 < 0.1f)
        {
            
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        ispushing = false;
      
        

    }
   
    
    IEnumerator jumper(sc info ,Rigidbody2D info2)
    {
        float elapsedTime1 = 0f;
        
        Vector2 initialScale = transform.localScale;

        info.confusion = true;
        info2.velocity = new Vector2(info2.velocity.x, 0);
        info2.AddForce(new Vector2(0,4f) * 5,ForceMode2D.Impulse);

        while (elapsedTime1 < 0.07f)
        {
            transform.localScale = Vector2.Lerp(initialScale, targetScale, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        elapsedTime1 = 0f;
        while (elapsedTime1 < 0.08f)
        {
            transform.localScale = Vector2.Lerp(targetScale, initialScale, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        while (elapsedTime1 < 0.1f)
        {
            
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        info.confusion = false;
        life = true;
    }


}
