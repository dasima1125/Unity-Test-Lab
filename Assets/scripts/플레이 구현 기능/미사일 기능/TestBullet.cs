using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

[RequireComponent(typeof(Rigidbody))]
public class TestBullet : MonoBehaviour
{
    private Vector3 target;
    private Rigidbody2D rb;
    private float speed = 100f;
    private float rotatespeed = 200;
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
    private Vector3 targetLastPos;
    private void Homing()
    {
        if (!fire) LunchWay();
        if (luncher != null)
        {
            
            /// 순수추적
            /*
            Vector2 newTarget = luncher.TargetPos();
            if (newTarget != Vector2.zero) target = newTarget;
            */

            /// 선도추적
            Vector2 Target = luncher.TargetPos();
            if (targetLastPos == Vector3.zero) targetLastPos = Target;
            Vector2 Velocity = luncher.TargetVelocity();
            Vector3 Pos = TestLunch.Calculate_LeadPosition(transform.position, Target, Velocity, speed);
            if (Pos != Vector3.zero)
            {
                target = Pos;
                gizmolead = Pos;
            }
            else
            {
                Vector2 pure = luncher.TargetPos();
                if (pure != Vector2.zero)
                {
                    target = pure;
                    gizmolead = pure;
                }
            }
            /// 비례추적

            //속도 좀 주고 추적해야함
            if (rb.velocity.magnitude < 0.1f)
            {
                //순수추적말고 선도추적을 하는거도좋겠네 아마도?
                Vector2 pure = luncher.TargetPos();
                if (pure != Vector2.zero)
                {
                    target = pure;
                    gizmolead = pure;
                }
            }
            else
            {
                Vector3 Pos2 = TestLunch.Calculate_PNAcceleration(Target, Target, transform.position, rb.velocity, 3);
            }

        }

        Vector2 dir = ((Vector2)target - rb.position).normalized;
        float ratateAmount = Vector3.Cross(dir, transform.up).z;

        rb.angularVelocity = -ratateAmount * rotatespeed;
        rb.velocity = transform.up * speed;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.GetComponent<Target_Test>();
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
    Vector3 gizmolead = Vector3.zero;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(luncher.TargetPos(), gizmolead);
        Gizmos.DrawSphere(gizmolead, 3f);
    }
    
}