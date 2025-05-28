using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

[RequireComponent(typeof(Rigidbody))]
public class TestBullet : MonoBehaviour
{
    private Vector3 target;
    private Rigidbody2D rb;
    private float speed = 50f;
    private float rotatespeed = 100;
    private float selfDestroy = 5f;

    bool fire = false;

    TestLunch luncher;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

#nullable enable
    public void Init(Vector3 target, TestLunch? testLunch = null)
    {
        this.target = target;
        luncher = testLunch;
        Destroy(gameObject, selfDestroy);
    }

    void FixedUpdate()
    {
        Homing();

        //Straight();
    }
    private void Straight()
    {
        if (!fire) LunchWay();
        rb.velocity = transform.up * speed;
    }
    private void Homing()
    {
        if (!fire) LunchWay();

        if (luncher != null)
        {
            /// 순수추적
            //Vector2 newTarget = luncher.TargetPos();
            //if (newTarget != Vector2.zero) target = newTarget;

            /// 비례추적
            
            Debug.Log("위치 호출함");
            Vector2 Target = luncher.TargetPos();
            Vector2 Velocity = luncher.TargetVelocity();
            Vector3 Pos = TestLunch.CalculateLeadPosition(transform.position,Target,Velocity,speed);
            if (Pos != Vector3.zero) target = Pos;
        }
        else
        {
            Debug.Log("런쳐를 찾을수 없음");

        }

        Vector2 dir = ((Vector2)target - rb.position).normalized;
        float ratateAmount = Vector3.Cross(dir, transform.up).z;

        rb.angularVelocity = -ratateAmount * rotatespeed;
        rb.velocity = transform.up * speed;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var target= collision.GetComponent<Target_Test>();
        if (target != null)
        {
            target.Hit();
            Destroy(gameObject);
        }
        
    }
    void LunchWay()
    {
        Vector2 direction = ((Vector2)target - rb.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        fire = true;
    }
    
}