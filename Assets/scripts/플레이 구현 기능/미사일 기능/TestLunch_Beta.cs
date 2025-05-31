using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class TestLunch_Beta : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject Projectile;
    public Transform Target;


    [Header("투사체 정보기입")]
    public float projectileSpeed = 0;
    public float maxAngularVelocity = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void LaunchTest()
    {
        if (Target == null)
        {
            Debug.Log("타겟이 없음");
            return;
        }
        //발사 로직
        //Projectile testBullet = Instantiate(Projectile, transform.position, transform.rotation).AddComponent<Projectile>();
        PassiveGuided testBullet = Instantiate(Projectile, transform.position, transform.rotation).AddComponent<PassiveGuided>();
        testBullet.Guideinit(this,projectileSpeed,maxAngularVelocity);
    }

    void FixedUpdate()
    {
        rb.rotation = TraceStright2D(TargetPos(), transform.position);
        //rb.rotation = TraceRead2D(TargetPos(), transform.position, TargetVelocity(), projectileSpeed);
    }



    float TraceStright2D(Vector2 targetPos, Vector2 startPos)
    {
        Vector2 direction = (targetPos - startPos).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    }
    float TraceRead2D(Vector2 targetPos, Vector2 startPos, Vector2 targetVel, float ProjectileSpeed)
    {
        Vector2 displacement = targetPos - startPos;
        float a = Vector2.Dot(targetVel, targetVel) - (ProjectileSpeed * ProjectileSpeed);
        float b = Vector2.Dot(displacement, targetVel) * 2;
        float c = Vector2.Dot(displacement, displacement);

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            //Debug.Log("예측 불가 : 방정식 해 존재 x");
            float fallbackTime = 7f;

            Vector2 fallbackPos = targetPos + targetVel * fallbackTime;
            Vector2 fallbackDir = (fallbackPos - startPos).normalized;

            return Mathf.Atan2(fallbackDir.y, fallbackDir.x) * Mathf.Rad2Deg;
        }
        float rootP = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float rootM = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float solution = Mathf.Max(rootP, rootM);

        Vector2 leadPos = targetPos + targetVel * solution;
        Vector2 dir = (leadPos - startPos).normalized;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public Vector3 TargetPos()
    {
        if (Target == null) return Vector2.zero;

        return Target.position;
    }
    Vector3 TargetVelocity()
    {
        if (Target == null) return Vector3.zero;

        return Target.GetComponent<Rigidbody2D>().velocity;
    }
    

    void OnDrawGizmos()
    {
        if (Target == null) return;

        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, TargetPos());

    }
    
}