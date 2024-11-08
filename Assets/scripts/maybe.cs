using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class maybe : MonoBehaviour
{

    public GameObject t;
    public float C1_2;
    private Rigidbody2D rb;

    private void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        //jumpatk();
        jumpatktest();
    
    }
    void jumpatk()
    {
        
        float target = t.transform.position.x - rb.position.x;
        Debug.Log(target);

        rb.AddForce(new Vector2(target,25),ForceMode2D.Impulse);
    }
    void jumpatktest()
{
    Collider2D[] attackRange2 = Physics2D.OverlapCircleAll(transform.position, 10f);
     foreach (Collider2D collider1_2C in attackRange2)
{
    if(collider1_2C.CompareTag("player"))
    {
    float myX = transform.position.x;

    // 상대 오브젝트의 x 위치
    float opponentX = collider1_2C.transform.position.x;

    // 현재 오브젝트와 상대 오브젝트 간의 x 좌표 차이 계산
    float xDistance = Mathf.Abs(myX - opponentX);

    // x 좌표 차이를 출력하거나 다른 작업을 수행할 수 있습니다.
    Debug.Log("x 좌표 차이: " + xDistance);
}
}
}
}