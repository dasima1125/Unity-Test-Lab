using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scManager : MonoBehaviour
{

    [Header("플레이어 정보")]
        [SerializeField] public GameObject player;
        [SerializeField] public GameObject sein;
        
        [SerializeField] public float player_Health;//최대 체력
        [SerializeField] public float player_NowHealth;//현재 체력
        
        [SerializeField] public float resource;//최대 게이지
        [SerializeField] public float player_Nowresource;//현재 게이지
        [SerializeField] public float resource_usemode_1;//소모량 지정 

        [SerializeField] public float key_count = 0;
        [SerializeField] public bool player_Death;
        [SerializeField] public bool player_MasterMode;
    [Header("사용기술 정보 UI 컨트롤")]
        [SerializeField] public bool attackAccept;
        [SerializeField] public bool dashAccept;
        [SerializeField] public bool sainAccept;
        [SerializeField] public bool mode_1_Accept;
    
    [Header("스폰포인트 정보")]
        [SerializeField] public GameObject mainSpawnPoint;
        [SerializeField] public GameObject subSpawnPoint;
        [SerializeField] public bool subSpwanisHere;

        [SerializeField] private GameObject particle;

        



        
    [Header("_________________________________________________")]
            [SerializeField] public Image hp;
            [SerializeField] public Image rp;
            [SerializeField] public TextMeshProUGUI keycount;

            



    
        
    
    // Start is called before the first frame update
    void Start()
    {
        player_Health = player_NowHealth;
       
        player_MasterMode    =   false;
        player_Death         =   false;

        
    }

    // Update is called once per frame
    void Update()
    {
       //테스트좀 해보자~
       hp.fillAmount = player_NowHealth / player_Health;
       rp.fillAmount = player_Nowresource / resource;
      
       if(key_count != 0){
            keycount.text = key_count.ToString();
       }
       



       subSpawn();
       
    }
  
    void playerDEATH()
    {
        player_Death = true;
        particleUnit();
        player.SetActive(false);
        sein.SetActive(false);
        
        Debug.Log("사망");


        StartCoroutine(revive());
    }
    public void insertDeamge(int i)
    {
       player_NowHealth -= i;
       hp.fillAmount = player_NowHealth / player_Health;
       if (player_NowHealth <= 0)
       {
        playerDEATH();
       }
    }
    public void insertUtilresource()
    {
        rp.fillAmount =  player_Nowresource / resource;
    }
    public void abilityOnUI()
    {


    }
    public void subSpawn()
    {

        float keyHoldTime = 0f;

        keyHoldTime = Input.GetKey(KeyCode.Q) ? keyHoldTime + Time.deltaTime : 0f;

        // 키를 3초 동안 누른 경우 디버그 메시지 출력 후 시간 초기화
        if (keyHoldTime >= 3)
        {
            Debug.Log("키를 3초 동안 눌렀습니다!");
            keyHoldTime = 0f;  // 이벤트 발생 후 시간 초기화
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (GameObject.FindGameObjectWithTag("subSP") != null)
            {
                Destroy(GameObject.FindGameObjectWithTag("subSP"));
                Debug.Log("이미존재함");
                Instantiate(subSpawnPoint, player.transform.position, Quaternion.identity);
            }
            else
            {

                Debug.Log("처음임");
                Instantiate(subSpawnPoint, player.transform.position, Quaternion.identity);
            }
            
            
        }
        
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
            SpriteRenderer currentSpriteRenderer    = particle.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = currentSpriteRenderer.sprite;
     
            objects[i].transform.localScale = new Vector2(Random.Range(0.1f, 0.15f), Random.Range(0.05f, 0.15f));
            objects[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            objects[i].transform.position = player.transform.position;
      
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

    


    IEnumerator revive()
    {
        float elapsedTime = 0f;//부활시간
    
        while (elapsedTime < 3)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("부활");

        if(GameObject.FindGameObjectWithTag("subSP") != null)
        player.transform.position =  GameObject.FindGameObjectWithTag("subSP").transform.position;
        else
        player.transform.position =  mainSpawnPoint.transform.position;
        player_NowHealth = player_Health;

        //대쉬중 죽으면 생기는 버그 땜빵용
        player.GetComponent<Rigidbody2D>().gravityScale = 5f;
        player.GetComponent<sc>().canDashing = true;
        

        player.SetActive(true);
        sein.SetActive(true);
        
    }

    IEnumerator debriskiller(GameObject debris)
    {
        float delayTime = Random.Range(1.5f, 3f);
        yield return new WaitForSeconds(delayTime);

        Destroy(debris);
       
    
    }
    
}
