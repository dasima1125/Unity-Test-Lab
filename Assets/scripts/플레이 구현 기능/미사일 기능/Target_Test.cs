using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Target_Test : MonoBehaviour
{
    private Vector2 A, B;
    private Vector2 targetPos;
    public float speed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        A = new Vector2(transform.position.x, transform.position.y - 400f);
        B = new Vector2(transform.position.x, transform.position.y );
        targetPos = A;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            targetPos = targetPos == A ? B : A;
        }

        Vector2 direction = (targetPos - rb.position).normalized;
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90f;
    }

    public void Hit()
    {
        Debug.Log("피격당함");
    }
}