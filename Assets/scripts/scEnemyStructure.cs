using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scEnemyStructure : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject test1;
    [SerializeField] GameObject test2;
    [SerializeField] GameObject test3;
    [SerializeField] GameObject test4;
    [SerializeField] GameObject reward;

    bool life;
    bool rewardpoint;
    void Start()
    {
        life = true;
        rewardpoint = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (life && AreAllObjectsDead())
        {
            // 모든 오브젝트가 비활성화되면 endAction 호출
            StartCoroutine(endAction());
            life = false;
             // endAction이 한 번만 호출되도록 설정
        }
        
    }
    bool AreAllObjectsDead()
    {
        return test1.GetComponent<scEnemy>().isDead &&
               test2.GetComponent<scEnemy>().isDead &&
               test3.GetComponent<scEnemy>().isDead &&
               test4.GetComponent<scEnemy>().isDead;
    }
    
    IEnumerator endAction()
    {
        
        

        float elapsedTime1 = 0f;
        float resizeDuration = 0.5f;
        Vector2 nowPostion     = transform.position;
        Vector2 targetPosition = nowPostion - new Vector2(0f, 5.8f);
       
        /*
        while (elapsedTime1 < resizeDuration)
        {

            transform.position = Vector2.Lerp(nowPostion, targetPosition, elapsedTime1 / resizeDuration);
            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        */// Ease 인 Ease로  구현
        while (elapsedTime1 < resizeDuration)
        {
            float t = elapsedTime1 / resizeDuration;
            
            float smoothT = Mathf.Pow(t, 3) / (Mathf.Pow(t, 3) + Mathf.Pow(1 - t, 3));
            transform.position = Vector2.Lerp(nowPostion, targetPosition, smoothT);

            if (Mathf.Abs(t - 0.6f) < 0.01f && rewardpoint)
            {
                Debug.Log("보상생성");
                GameObject SpawnedReward = Instantiate(reward, targetPosition+ new Vector2(0f, 4f), Quaternion.identity);
                Rigidbody2D spawnedRigidbody = SpawnedReward.GetComponent<Rigidbody2D>();
                   
                spawnedRigidbody.AddForce(new Vector2(0,10f) ,ForceMode2D.Impulse);
                rewardpoint = false;
            }

            elapsedTime1 += Time.deltaTime;
            yield return null;
        }
        
       
    }
}
