using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimplePNMissile2D : MonoBehaviour
{
    public Transform target;
    public float missileSpeed = 10f;
    public float navigationConstant = 3f;
    public float maxAngularVelocity = 180f; // 초당 선회가능각도

    public bool usePN = true;

    private Rigidbody2D rb;
    private Vector2 lastTargetPosition;
    private Vector2 leadPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (target != null)
        {
            lastTargetPosition = target.position;

            Vector2 dir = ((Vector2)target.position - rb.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            Vector2 forward = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rb.velocity = forward * missileSpeed;
        }
    
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 missilePos = rb.position;
        Vector2 targetPos = target.position;

        Vector2 targetVel = (targetPos - lastTargetPosition) / Time.fixedDeltaTime;
        Vector2 missileVel = rb.velocity;

        if (usePN)
        {
            Vector2 LOS = targetPos - missilePos;
            float LOSsq = LOS.sqrMagnitude;
            if (LOSsq < 0.0001f) return;

            Vector2 LOSdir = LOS.normalized;
            Vector2 relVel = targetVel - missileVel;

            float LDot = (LOSdir.x * relVel.y - LOSdir.y * relVel.x) / LOSsq;
            float O = navigationConstant * missileSpeed * LDot;
            float clampedO = Mathf.Clamp(O, -maxAngularVelocity * Mathf.Deg2Rad, maxAngularVelocity * Mathf.Deg2Rad);
            rb.angularVelocity = clampedO * Mathf.Rad2Deg;

            Vector2 forward = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
            rb.velocity = forward * missileSpeed;

            float distance = Vector2.Distance(targetPos, missilePos);
            float relativeSpeed = missileSpeed + targetVel.magnitude;
            float leadTime = Mathf.Clamp(distance / relativeSpeed, 0.1f, 5f);

            leadPoint = targetPos + targetVel * leadTime;
        }
        else
        {
            Vector2 direction = (targetPos - missilePos).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);
            float maxDelta = maxAngularVelocity * Time.fixedDeltaTime;
            float clampedDelta = Mathf.Clamp(angleDiff, -maxDelta, maxDelta);
            rb.rotation += clampedDelta;

            Vector2 forward = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
            rb.velocity = forward * missileSpeed;

            leadPoint = targetPos;
        }

        lastTargetPosition = targetPos;
    }

    void OnDrawGizmos()
    {
        if (target == null) return;

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
            Debug.Log("도달 : pn알고리즘 여부 " + usePN);
            Destroy(gameObject);
        }
    }
}
