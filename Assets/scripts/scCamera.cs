using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class scCamera : MonoBehaviour
{
    Transform target;

  
    [SerializeField] private Vector3 positionOffset;
    Vector3 velocity = Vector3.zero;
    
    public bool isZoom = false;
  
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
    public void zoomIn()
    {
        if (!isZoom)
        {
            isZoom = true;
            Camera camera = gameObject.GetComponent<Camera>();
            float targetSize = camera.orthographicSize / 2f;
            DOTween.To(() => camera.orthographicSize, x => camera.orthographicSize = x, targetSize, 1f); 
        }
        
    }
    public void zoomOut()
    {
        if (isZoom)
        {
            isZoom = false;
            Camera camera = gameObject.GetComponent<Camera>();
            float targetSize = camera.orthographicSize * 2f;
            DOTween.To(() => camera.orthographicSize, x => camera.orthographicSize = x, targetSize, 1f); 
        }
          
        
    }
}
