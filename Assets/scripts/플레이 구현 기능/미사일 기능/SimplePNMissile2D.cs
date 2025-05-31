using System;
using UnityEngine;
using UnityEngine.VFX;
public enum TrackType
{
    None ,Pure ,Lead ,Pn
}
[RequireComponent(typeof(Rigidbody2D))]
public class SimplePNMissile2D : MonoBehaviour
{
    public Transform target;
    public float missileSpeed = 10f;
    public float navigationConstant = 3f;
    public float maxAngularVelocity = 180f; // 초당 선회가능각도

    public TrackType trackType;

   

    /// <summary>
    ///  신규 시스템
    /// </summary>

    // non - trace << basic data
    private float projectileSpeed;

    // use - trace
    private Vector2 targetPos;
    private Vector2 targetVel;

    private float maxAngularVel;
    private float PNgain;

    //memory
    private Rigidbody2D rb;
    private Vector2 lastTargetPosition;
    private Vector2 leadPoint;
    private Action LeadAI;
    


    public void init()
    {

    }

    void Start()
    {
        if (target == null)
        {
            Debug.Log("정보없음 : 객체 삭제실시");
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        KickBack();
        switch (trackType)
        {
            case TrackType.None:
                LeadAI += None;
                break;
            case TrackType.Pure:
                LeadAI += PurePursuit;
                break;
            case TrackType.Lead :
                LeadAI += LeadLine;
                break;
            case TrackType.Pn:
                LeadAI += ProportionalNavigation;
                break;        
        }
    
    }
    void KickBack()
    {
        lastTargetPosition = target.position;
        rb.rotation = CallRotationStright2D(target.position,transform.position);
        //rb.rotation = CallRotationRead2D(target.position,transform.position,target.GetComponent<Rigidbody2D>().velocity,missileSpeed);
        
    }
    float CallRotationStright2D(Vector2 targetPos , Vector2 startPos)
    {
        Vector2 direction = (targetPos - startPos).normalized;
        return Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
    }
    float CallRotationRead2D(Vector2 targetPos, Vector2 startPos, Vector2 targetVel, float ProjectileSpeed)
    {
        Vector2 displacement = targetPos - startPos;
        float a = Vector2.Dot(targetVel, targetVel) - (ProjectileSpeed * ProjectileSpeed);
        float b = Vector2.Dot(displacement, targetVel) * 2;
        float c = Vector2.Dot(displacement, displacement);

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            Debug.Log("예측 불가 : 방정식 해 존재 x");
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

    void FixedUpdate()
    {
        LeadAI();
    }
    void None()
    {
        ApplyFire(-1f);
    }
    void PurePursuit() // 목표위치 , 투사체위치
    {
        Vector2 missilePos = rb.position;
        Vector2 targetPos = target.position;

        Vector2 direction = (targetPos - missilePos).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);
        float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
        float clampedPure = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);

        ApplyFire(clampedPure);

        leadPoint = targetPos;
    }
    void LeadLine()// 목표위치 , 투사체위치 , 타겟 속도 ,자기속도
    {
        Vector3 missilePos = rb.position;
        Vector3 targetPos = target.position;
        Vector3 targetVelocity = target.GetComponent<Rigidbody2D>().velocity;

        Vector3 displacement = targetPos - missilePos;

        float a = Vector3.Dot(targetVelocity, targetVelocity) - (missileSpeed * missileSpeed);
        float b = Vector3.Dot(displacement, targetVelocity) * 2;
        float c = Vector3.Dot(displacement, displacement);

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            Debug.Log("도달 불가 : 순수추적으로 대체");
            PurePursuit();
            return;
        }
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t = Mathf.Max(t1, t2);

        Vector3 lead = targetPos + targetVelocity * t;
        Vector3 dir = (lead - missilePos).normalized;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);
        float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
        float clampedLead = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);

        ApplyFire(clampedLead);

        leadPoint = lead;

    }
    void ProportionalNavigation() // 목표위치 , 투사체위치 , 투사체속도
    {
        Vector2 missilePos = rb.position;
        Vector2 targetPos = target.position;
        Vector2 targetVel = (targetPos - lastTargetPosition) / Time.fixedDeltaTime;
        Vector2 missileVel = rb.velocity;

        Vector2 LOS = targetPos - missilePos;
        float LOSsq = LOS.sqrMagnitude;
        if (LOSsq < 0.0001f)
        {
            Debug.Log("추적 실패");
            return;
        }    

        Vector2 LOSdir = LOS.normalized;
        Vector2 relVel = targetVel - missileVel;
        float LDot = (LOSdir.x * relVel.y - LOSdir.y * relVel.x) / LOSsq;
        float Omega = navigationConstant * missileSpeed * LDot;
        float clampedOmega = Mathf.Clamp(Omega, -maxAngularVelocity * Mathf.Deg2Rad, maxAngularVelocity * Mathf.Deg2Rad);

        lastTargetPosition = targetPos;
        ApplyFire(clampedOmega);

        /// 예측위치 전달
        float leadTime = Mathf.Clamp(Vector3.Distance(targetPos, missilePos) / (missileSpeed + targetVel.magnitude), 0.1f, 5f);
        leadPoint = targetPos + targetVel * leadTime;

    }

    void ApplyFire(float clamped)
    {
        switch (trackType)
        {
            case TrackType.None:
                break;

            case TrackType.Pure:
                rb.rotation += clamped;
                break;

            case TrackType.Lead:
                rb.rotation += clamped;
                break;

            case TrackType.Pn:
                rb.angularVelocity = clamped * Mathf.Rad2Deg;
                break;
        }
        Vector2 dir = new(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
        rb.velocity = dir * missileSpeed;
    }

    void OnDrawGizmos()
    {
        if (target == null || leadPoint == Vector2.zero) return;
      
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
       
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leadPoint, 3f);
        Gizmos.DrawLine(transform.position, leadPoint);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TestTarget")
        {
            Debug.Log("도달 : 알고리즘 여부 " + trackType.ToString());
            Destroy(gameObject);
        }
    }
}
