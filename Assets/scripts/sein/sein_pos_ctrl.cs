using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sein_pos_ctrl : MonoBehaviour
{
    public Transform A, B;
    public float speed;
    public Vector2 targetPos;
    
    void Start()
    {
        targetPos = B.localPosition; // 로컬 좌표계에서의 위치로 지정
    }

    void FixedUpdate()
    {
        if(Vector2.Distance(transform.localPosition, A.localPosition) < 0.05f)
        {
            targetPos = B.localPosition;
        }
        if(Vector2.Distance(transform.localPosition, B.localPosition) < 0.05f)
        {
            targetPos = A.localPosition;
        }
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);
    }
}
