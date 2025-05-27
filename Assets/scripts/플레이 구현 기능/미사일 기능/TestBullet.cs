using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    private Vector3 target;
    private float speed = 1000f; 
    private float selfDestroy = 1f;

    void Start()
    {
        Debug.Log("시작");
    }

    public void init(Vector3 target)
    {
        this.target = (target - transform.localPosition).normalized;
        Destroy(gameObject, selfDestroy);
    }

    void FixedUpdate()
    {
        Vector3 movement = target * speed * Time.fixedDeltaTime;
        transform.localPosition += movement;
    }
}