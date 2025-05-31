using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float ProjectileSpeed;
    private Rigidbody2D rb;
    protected bool Override;


    public void init(float ProjectileSpeed)
    {
        this.ProjectileSpeed = ProjectileSpeed;
    }
    protected virtual void overrideInit(float ProjectileSpeed)
    {
        Override = true;
        this.ProjectileSpeed = ProjectileSpeed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!Override)
            bust(0);

    }
    

    protected void bust(float clamped)
    {
        rb.rotation += clamped;

        Vector2 dir = new(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
        rb.velocity = dir * ProjectileSpeed;
    }
    protected INS2DData GetINS2D()
    {
        return new INS2DData
        {
            position = rb.position,
            velocity = rb.velocity,
            rotation = rb.rotation
        };
    }

    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TestTarget")
        {
            Destroy(gameObject);
        }
    }

}

public class PassiveGuided : Projectile
{
    TestLunch_Beta Link;
    INS2DData INS;
    float maxAngularVelocity;
    public void Guideinit(TestLunch_Beta Link, float projectileSpeed, float maxAngularVelocity)
    {
        Debug.Log("확장 모듈 시행");
        this.Link = Link;
        this.maxAngularVelocity = maxAngularVelocity; 
        base.overrideInit(projectileSpeed);

    }
    private void FixedUpdate()
    {
        //bust(0)
        INS = GetINS2D();

        Debug.Log("유도중");
        
        //bust(TLogic.RotationStright2D(Link.TargetPos(),INS.position));
        bust(TLogic.PureLeadClamped(Link.TargetPos(),INS.position,INS.rotation ,maxAngularVelocity ));
        
    }


    //충돌자 구현
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TestTarget")
        {
            Debug.Log("작동");
            base.OnTriggerEnter2D(collision);
        }
    }

}


public static class TLogic
{
    public static float PureLead(Vector2 targetPos, Vector2 startPos)
    {
        Vector2 direction = (targetPos - startPos).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    }
    public static float PureLeadClamped(Vector2 targetPos, Vector2 startPos, float NowRotate, float maxAngularVelocity)
    {
        Vector2 direction = (targetPos - startPos).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float angleDiff = Mathf.DeltaAngle(NowRotate, targetAngle);
        float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
        float clampedPure = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);

        return clampedPure;
    }

}
public struct INS2DData
{
    public Vector2 position;
    public Vector2 velocity;
    public float rotation;

}
