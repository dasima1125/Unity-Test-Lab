using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class TestLunch_Beta : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject Projectile;
    public Transform Target;
    [Header("Firing Control")]
    public bool LockMode;
    public AimingMode TrackMode;
    [Header("Guidance Configuration")]
    //public 
    public GuidanceAuthority GuidanceType;
    public TrackLogicType InjectLogic;

    [Header("Projectile Parameters")]
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
            Debug.Log("Non Target");
            return;
        }
        //발사 로직
        TrackLogicInjector();
    }

    void FixedUpdate()
    {
        aim(LockMode);
    }
    void aim(bool trace)
    {
        if (!trace) return;
        switch (TrackMode)
        {
            case AimingMode.Intuitive:
                rb.rotation = TraceStright2D(TargetPos(), transform.position);
                break;

            case AimingMode.Lead:
                rb.rotation = TraceRead2D(TargetPos(), transform.position, TargetVelocity(), projectileSpeed);
                break;
        }
    }
    void TrackLogicInjector()
    {
        switch (GuidanceType)
        {
            case GuidanceAuthority.None:
                Projectile Projectile_None = Instantiate(Projectile, transform.position, transform.rotation).AddComponent<Projectile>();
                Projectile_None.Init(projectileSpeed);
                break;

            case GuidanceAuthority.Command:
                Guided Projectile_Command = Instantiate(Projectile, transform.position, transform.rotation).AddComponent<Guided>();
                Projectile_Command.GuideInit(this, projectileSpeed, InjectLogic, maxAngularVelocity, 10);
                break;
        }

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
    public Vector3 TargetVelocity()
    {
        if (Target == null) return Vector3.zero;

        return Target.GetComponent<Rigidbody2D>().velocity;
    }
    public TNS2DData GetTNS2D()
    {
        return new TNS2DData
        {
            position = Target.position,
            velocity = Target.GetComponent<Rigidbody>().velocity,
        };
    }

}

public enum AimingMode
{
    Intuitive ,Lead

}
public enum GuidanceAuthority
{
    None,
    Command          
}
public enum TrackLogicType
{
    Pure ,Lead ,Pn
}