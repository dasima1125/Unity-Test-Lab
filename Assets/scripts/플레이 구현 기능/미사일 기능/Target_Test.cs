using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Target_Test : MonoBehaviour
{
    private Vector2 A, B;
    private Vector2 targetPos;
    public float speed;
    public string way;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        A = new Vector2(transform.position.x, transform.position.y + 120f);
        B = new Vector2(transform.position.x, transform.position.y - 120f);

        targetPos = A;
    }

    void FixedUpdate()
    {

        // 목표 위치 근접 시 방향 전환
        if (Vector2.Distance(rb.position, targetPos) < 1f)
        {
            targetPos = targetPos == A ? B : A;
            way = targetPos == A ? "A" : "B";
      
        }
        Vector2 direction = (targetPos - rb.position).normalized;
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle - 90f);
    }

    public void Hit()
    {
        Debug.Log("피격당함");
    }
}
