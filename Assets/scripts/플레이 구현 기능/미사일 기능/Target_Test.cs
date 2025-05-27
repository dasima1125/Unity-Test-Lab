using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Test : MonoBehaviour
{
    private Vector2 A, B;
    private Vector2 targetPos;
    public float speed;

    void Start()
    {
        A = new Vector2(transform.position.x, transform.position.y + 100f);
        B = new Vector2(transform.position.x, transform.position.y - 100f);

        targetPos = A;
    }


    void FixedUpdate()
    {
        if (Vector2.Distance(transform.localPosition, targetPos) < 0.05f)
        {
            targetPos = targetPos == A ? B : A;
        }
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPos, speed * Time.fixedDeltaTime);

        Vector2 direction = targetPos - (Vector2)transform.localPosition;

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
    public void Hit()
    {

    }
}
