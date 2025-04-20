using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRC_Radar : MonoBehaviour
{
    private Transform SweepBarTransForm;
  
    private float radarSpeed;
    private float radarDistance;
    private int size = 1;
    
    
    private List<Collider2D> ContectList;
    [SerializeField] private Transform RadarPing;
    [SerializeField] private Camera RadarCam;
    
    void Awake()
    {
        SweepBarTransForm   = transform.Find("SweepBar");
        radarSpeed = 240;
        radarDistance = 150f;
        
        ContectList= new();
        
    }

    // Update is called once per frame
    void Update()
    {
        float previousRoutation = (SweepBarTransForm.eulerAngles.z % 360) - 180f;
        SweepBarTransForm.eulerAngles -= new Vector3(0, 0, radarSpeed * Time.deltaTime);
        float CurrentRoutation = (SweepBarTransForm.eulerAngles.z % 360) - 180f;

        if (previousRoutation < 0 && CurrentRoutation >= 0)
        {
            ContectList.Clear();

        }

        RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(transform.position,GetVectorFromAngle(SweepBarTransForm.eulerAngles.z),radarDistance);
        foreach(RaycastHit2D rayhit in rayhitArr) 
        {
            if(rayhit.collider != null)
            if(!ContectList.Contains(rayhit.collider))
            {
                ContectList.Add(rayhit.collider);
                RadarPing Ping = Instantiate(RadarPing,rayhit.point,Quaternion.identity).GetComponent<RadarPing>();
                if(rayhit.collider.gameObject.GetComponent<Signal>() != null)
                {
                    Ping.SetColor(Color.green);
                    Ping.SetSize(size);
                }
                Ping.SetDisappearTimer(360f/radarSpeed);
            }
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            SetDistenceSeach();
        }
        
    }

    void SetDistenceSeach()
    {
        if(radarDistance == 150f)
        {
            RadarCam.orthographicSize *= 2;
            
            transform.localScale = new Vector3(2, 2);
            radarDistance *= 2;
            size = 2;
            foreach(Collider2D obj in ContectList) 
            {
                obj.GetComponent<RadarPing>().SetSize(2f);
            }
            
        }
        else
        {
            RadarCam.orthographicSize /= 2;
            
            transform.localScale = new Vector3(1, 1);
            radarDistance /= 2;
            size = 1;
            foreach(Collider2D obj in ContectList) 
            {
                obj.GetComponent<RadarPing>().SetSize(0.5f);
            }

        }
        
    }
    void SetDistenceTrack()
    {

    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadian = angle * (Mathf.PI/180f); // or Deg2Rad 내장상수를 써도됨 0.01745329f
        return new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
    }
    void OnDrawGizmos()
    {
        if (SweepBarTransForm == null) return;

        Vector3 dir = GetVectorFromAngle(SweepBarTransForm.eulerAngles.z);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, dir.normalized * radarDistance);
    }
}
