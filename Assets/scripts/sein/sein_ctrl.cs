using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sein_ctrl : MonoBehaviour
{

    public Transform target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      transform.position = Vector2.Lerp(transform.position,target.position,Time.deltaTime * speed);  
    }
}
