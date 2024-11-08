using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_thrower_bullet : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rigidbody_me;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        test1();
    }
    void test1()
    
    {
        Collider2D[] colliders1 = Physics2D.OverlapCircleAll(transform.position,0.24f); 
        foreach (Collider2D collider in colliders1)
        {
            sc insert = collider.GetComponent<sc>();
            if (collider.CompareTag("player") && !insert.IsWound)
            {
                insert.TakeDamage();
                Destroy(gameObject);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("platform")||collision.gameObject.CompareTag("ground")) 
        Destroy(gameObject);
            
        
        //if(collision.gameObject.CompareTag("enemy")) 
        //    gameObject.layer = LayerMask.NameToLayer("player_ghosting");

    }
}
