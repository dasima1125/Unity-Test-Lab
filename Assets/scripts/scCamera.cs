using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scCamera : MonoBehaviour
{
    Transform target;

  
    [SerializeField] private Vector3 positionOffset;
    Vector3 velocity = Vector3.zero;
  
    [Range (0,1)] public float chaseTime;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition =target.position+positionOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity,chaseTime); 
    }





    public IEnumerator shakeCamera(float time , float shakerange)
    {
        float shakeEndtime = 0;
        while (shakeEndtime < time)
        {
            Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * shakerange;
            transform.position = transform.position + new Vector3(shakeOffset.x, shakeOffset.y, 0);
            

            shakeEndtime += Time.deltaTime;
            yield return null;    
        }


        
    }
}
