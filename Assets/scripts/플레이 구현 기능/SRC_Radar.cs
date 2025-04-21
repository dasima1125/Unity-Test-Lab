using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SRC_Radar : MonoBehaviour
{
    private Transform SweepBarTransForm;
  
    private float radarSpeed;
    private float radarDistance;
    public int size = 1;
    
    
    private List<Collider2D> ContectList;
    
    [SerializeField] private Transform RadarPing;
    [SerializeField] private Camera RadarCam;

    //track beam
    [SerializeField]private Transform trackBeamTransForm;
    [SerializeField, Range(0, 359)] private float trackBeamAngle = 0;
    [SerializeField, Range(0, 180)] private float trackBeamWidth = 20;
    
    
    void Awake()
    {
        SweepBarTransForm   = transform.Find("SweepBar");
        radarSpeed = 240;
        radarDistance = 150f;
        
        ContectList= new();
        
        SignalList = new();
        
    }
    public List<RadarPing> SignalList;
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
                    SignalList.Add(Ping);
                }
                Ping.SetDisappearTimer(360f/radarSpeed);
            }
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            SetDistenceSeach();
        }
        SetTrackWayTest();

        
    }

    void SetDistenceSeach()
    {
        if(radarDistance == 150f)//중거리
        {
            RadarCam.orthographicSize *= 2;
            
            transform.localScale = new Vector3(2, 2);
            radarDistance *= 2;
            size = 2;

        }
        else
        {
            RadarCam.orthographicSize /= 2;//장거리
            
            transform.localScale = new Vector3(1, 1);
            radarDistance /= 2;
            size = 1; 

        }
    }
    public float trackBeamRotateSpeed = 1f;
    private float currentTrackBeamAngle = 0f;
    void SetTrackWayTest()     // 목표 각도 설정
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) trackBeamAngle = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) trackBeamAngle = 90;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) trackBeamAngle = 180;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) trackBeamAngle = 270;

        currentTrackBeamAngle = Mathf.MoveTowardsAngle(currentTrackBeamAngle, trackBeamAngle, 360 / trackBeamRotateSpeed * Time.deltaTime);         

        if (trackBeamTransForm != null)
        {
            trackBeamTransForm.GetComponent<Image>().fillAmount = trackBeamWidth / 360f;
            trackBeamTransForm.localRotation = Quaternion.Euler(0, 0, -currentTrackBeamAngle + ((trackBeamWidth / 2f) - 90));
            
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
