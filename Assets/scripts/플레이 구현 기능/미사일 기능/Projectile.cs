using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float ProjectileSpeed;
    private Rigidbody2D rb;
    protected bool Override;


    public void Init(float ProjectileSpeed)
    {
        this.ProjectileSpeed = ProjectileSpeed;
    }
    protected virtual void OverrideInit(float ProjectileSpeed)
    {
        Override = true;
        this.ProjectileSpeed = ProjectileSpeed;
    }

    protected virtual void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject , 25f);
        if (!Override)
            ThrustRotation(0);

    }
    
    protected void ThrustRotation(float clamped )
    {
        rb.rotation += clamped;

        Vector2 dir = new(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
        rb.velocity = dir * ProjectileSpeed;
    }
    protected void ThrustAngular(float ? clamped)
    {
        if (clamped == null)
        {
            ThrustRotation(0);
            return;
        }
            
        rb.angularVelocity = clamped.Value;

        Vector2 dir = new(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
        rb.velocity = dir * ProjectileSpeed;
    }
    
    protected INS2DData GetINS2D()
    {
        return new INS2DData
        {
            position = rb.position,
            velocity = rb.velocity,
            rotation = rb.rotation,
            Speed = ProjectileSpeed
        };
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TestTarget")
        {
            collision.gameObject.GetComponent<Target_Test>().Hit();
            Destroy(gameObject);
        }
    }

}

public class Guided : Projectile
{
    /// Debug
    
    public bool DebugPath = false;
    Vector2 leadPoint;
    // Projectile Settings
    TestLunch_Beta Link;
    float maxAngularVelocity;
    float navigationConstant;

    // Runtime Memory
    
    INS2DData INS;
    TrackLogicType Logic;
    Vector2 targetMem;
    
    
    
    public void GuideInit(TestLunch_Beta Link, float projectileSpeed,TrackLogicType Logic ,float maxAngularVelocity, float navigationConstant = 0)
    {
        this.Link = Link;
        this.Logic = Logic;
        this.maxAngularVelocity = maxAngularVelocity;
        this.navigationConstant = navigationConstant;

        base.OverrideInit(projectileSpeed);
    }
    protected override void Start()
    {
        targetMem = Link.TargetPos();
        base.Start();
    }
    private void FixedUpdate()
    {
        INS = GetINS2D();
        LogicSelectot();
    }
    void LogicSelectot()
    {
        switch (Logic)
        {
            case TrackLogicType.Pure:
                ThrustRotation(TLogic2D.PureLeadClamped(Link.GetTNS2D(), INS, maxAngularVelocity, out leadPoint));
                break;

            case TrackLogicType.Lead:
                ThrustRotation(TLogic2D.LeadLineClamped(Link.GetTNS2D(), INS, maxAngularVelocity, out leadPoint));
                break;

            case TrackLogicType.Pn:
                float ? cacl = TLogic2D.PN(Link.GetTNS2D(), INS, ref targetMem, maxAngularVelocity, navigationConstant , out leadPoint);
                if (cacl != null) ThrustAngular(cacl);
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (leadPoint == Vector2.zero && !DebugPath) return;
      
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Link.TargetPos());
       
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leadPoint, 3f);
        Gizmos.DrawLine(transform.position, leadPoint);
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TestTarget")
        {
            base.OnTriggerEnter2D(collision);
        }
    }

}


public static class TLogic2D
{
    public static float PureLeadClamped(TNS2DData TNS, INS2DData INS, float maxAngularVelocity, out Vector2 leadPoint)
    {
        Vector2 direction = (TNS.position - INS.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float angleDiff = Mathf.DeltaAngle(INS.rotation, targetAngle);
        float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
        float clampedPure = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);

        leadPoint = TNS.position;
        return clampedPure;
    }

    public static float LeadLineClamped(TNS2DData TNS, INS2DData INS, float maxAngularVelocity, out Vector2 leadTarget)
    {
        Vector2 displacement = TNS.position - INS.position;

        float a = Vector2.Dot(TNS.velocity, TNS.velocity) - (INS.Speed * INS.Speed);
        float b = Vector2.Dot(displacement, TNS.velocity) * 2;
        float c = Vector2.Dot(displacement, displacement);
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            Debug.LogAssertion("Lead calculation failed: no valid interception solution (discriminant < 0 or degenerate geometry).");
            leadTarget = Vector2.zero;
            return 0;
        }
        float rootP = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float rootM = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t = Mathf.Max(rootP, rootM);

        Vector2 lead = TNS.position + TNS.velocity * t;
        Vector2 dir = (lead - INS.position).normalized;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angleDiff = Mathf.DeltaAngle(INS.rotation, targetAngle);
        float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
        float clampedLead = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);
        leadTarget = lead;

        return clampedLead;
    }
    
    public static float? PN(TNS2DData TNS, INS2DData INS, ref Vector2 LastPos, float maxAngularVelocity, float navigationConstant, out Vector2 leadTarget)
    {
        Vector2 caclTargetVel = (TNS.position - LastPos) / Time.fixedDeltaTime;

        Vector2 LOS = TNS.position - INS.position;
        float LOSsq = LOS.sqrMagnitude;

        if (LOSsq < 0.0001f)
        {
            Debug.LogAssertion("Unable to compute angular velocity: LOS ≈ 0 (missile near target)");
            leadTarget = Vector2.zero;
            return null;
        }

        Vector2 LOSdir = LOS.normalized;
        Vector2 relVel = caclTargetVel - INS.velocity;
        float LDot = ((LOSdir.x * relVel.y) - (LOSdir.y * relVel.x)) / LOSsq;
        float Omega = navigationConstant * INS.Speed * LDot;
        float clampedOmega = Mathf.Clamp(Omega, -maxAngularVelocity * Mathf.Deg2Rad, maxAngularVelocity * Mathf.Deg2Rad);

        LastPos = TNS.position;

        float leadTime = Mathf.Clamp(Vector2.Distance(TNS.position, INS.position) / (INS.Speed + caclTargetVel.magnitude), 0.1f, 50f);
        leadTarget = TNS.position + caclTargetVel * leadTime;

        return clampedOmega;
    }

}
public struct INS2DData
{
    public Vector2 position;
    public Vector2 velocity;
    public float rotation;
    public float Speed;

}
 
public struct TNS2DData 
{
    public Vector2 position;
    public Vector2 velocity;
 
}

