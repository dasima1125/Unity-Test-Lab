using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class sein_missle : MonoBehaviour
{
    [Header("미사일 지정속성")]
    [SerializeField] private GameObject sein;
    public Vector3 target;
    GameObject player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
    }

    void Update()
    {
        scAttack damege  =player.GetComponent<scAttack>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        foreach (Collider2D collider in colliders)
            {
                if(collider.CompareTag("enemy"))
                {
                  
                    scEnemy enemy = collider.GetComponent<scEnemy>();
                    
            
                    enemy.hitPos = transform.position;
                    enemy.TakeDamage(damege.sein_missle_Damage,2);
                   
                    Destroy(sein);
    
                }
            
                
                
            }

        if(Vector2.Distance(sein.transform.position,target) > 0.3f)
        {
            sein.transform.position = Vector2.MoveTowards(sein.transform.position,target,80f*Time.deltaTime);
        }
        if(Vector2.Distance(sein.transform.position,target) < 0.3f)
        {
            Destroy(sein);
        }
        
    }
}
