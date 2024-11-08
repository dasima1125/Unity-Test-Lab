using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scPortal : MonoBehaviour
{

    [Header("포탈 오브젝트")]
        [SerializeField] public GameObject object1;
        [SerializeField] public GameObject object2;
        [SerializeField] private bool iminner;

        private BoxCollider2D collider1;
        private BoxCollider2D collider2;
        private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        collider1 = object1.GetComponent<BoxCollider2D>();
        collider2 = object2.GetComponent<BoxCollider2D>();

         scManager manager = FindObjectOfType<scManager>();
        if (manager != null)
        {
            //Debug.Log(" 획득 성공");
            player = manager.player; // Get the player object from scManager
        }
      
    }

    // Update is called once per frame
    void Update()
    {
        detector();
    }

    void detector()
    {

        iminner = false;
        Vector2 portalCenter1 = (Vector2)collider1.transform.position;
        Vector2 portalsize1 = collider1.bounds.size;

        Collider2D[] portal1_colliders = Physics2D.OverlapBoxAll(portalCenter1, portalsize1, 0f);
        foreach (Collider2D collider in portal1_colliders)
        {
            if (collider.CompareTag("player"))
                {
                    iminner = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Detected portal 1: " + collider.name);
                        player.transform.position = collider2.transform.position;
                    }
                          
                }
        
        }

        Vector2 portalCenter2 = (Vector2)collider2.transform.position;
        Vector2 portalsize2 = collider2.bounds.size;

        Collider2D[] portal2_colliders = Physics2D.OverlapBoxAll(portalCenter2, portalsize2, 0f);
        foreach (Collider2D collider in portal2_colliders)
        {
            if (collider.CompareTag("player"))
            {
                iminner = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Detected portal 2: " + collider.name);
                    player.transform.position = collider1.transform.position;
                }
                
            }
        
        }

    }

}
