using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System.Collections;
using System.Linq;


public class SRC_Radar : MonoBehaviour
{
    private Transform SweepBarTransForm;
    private float radarSpeed;
    private float radarDistance;
    [HideInInspector]
    public int size = 1;
    private HashSet<Collider2D> ContectList;
    
    [SerializeField] private Transform RadarPing;
    [SerializeField] private Camera RadarCam;
    
    
    void Awake()
    {
        SweepBarTransForm   = transform.Find("SweepBar");
        radarSpeed = 240;
        radarDistance = 150f;
        
        ContectList= new();
        SignalList = new();
        
    }
    void Start()
    {
    }
    //[HideInInspector]
    [NonSerialized] //또는 프리베이트 설정
    public List<RadarPing> SignalList; 
    //public HashSet<RadarPing> SignalList; //리스트를 사용하면 문제가생김
    //문제해결 직렬화 문제 인스펙터 조작하면 아모르겠다 문제가한두개가아님 직렬화가 문제의중심임
    // 블로그에서 따로 정리를 해볼예정 이건 좀,., 중요할듯
    void Update()
    {
        Search();
        TrackRadarArea();
        MoveHeadonbeam();

        Action();

        //HardLock_STT();

        //기능정리구조
        
        
    }
    void Search()
    {
        SweepBarTransForm.eulerAngles -= new Vector3(0, 0, radarSpeed * Time.deltaTime);

        RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(transform.position,GetVectorFromAngle(SweepBarTransForm.eulerAngles.z),radarDistance);
        foreach(RaycastHit2D rayhit in rayhitArr) 
        {
            if(rayhit.collider != null)
            if(!ContectList.Contains(rayhit.collider))
            {
                ContectList.Add(rayhit.collider);
                DOVirtual.DelayedCall(1f, () => ContectList.Remove(rayhit.collider));

                if(rayhit.collider.gameObject.GetComponent<Signal>() != null && rayhit.collider.gameObject.GetComponent<Signal>().Tracked == false)
                {
                    RadarPing Ping = Instantiate(RadarPing,rayhit.point,Quaternion.identity).GetComponent<RadarPing>();
                    float angle = (SweepBarTransForm.eulerAngles.z - 90f + 360f) % 360f;  
                    angle = 360f - angle;

                    Ping.SetAngle(angle);
                    Ping.SetColor(Color.green);   
                    SignalList.Add(Ping);
                    
                    Ping.SetDisappearTimer(360f/radarSpeed);
                }
            }
        }

    }
    [Header("추적 레이더 기능")]
    [SerializeField] private Transform HeadOnBeam; //해드온 방향 빔 오브젝트
    [SerializeField] private Transform TrackArea;  //추적 범위 아크 오브젝트
    [SerializeField, Range(0, 360)] private float trackBeamWidth_beta = 20;//추적범위 넒이 
    [SerializeField]private float TrackAreaRotateSpeed = 10f;//추적 범위 속도
    private float TrackAreaAngle = 0f;
    private float DebugAngle;
    void TrackRadarArea()
    {
        TrackAreaAngle = Mathf.MoveTowardsAngle(TrackAreaAngle, HeadOnBeam.localRotation.eulerAngles.z, 360f / TrackAreaRotateSpeed * Time.deltaTime);
        TrackArea.GetComponent<Image>().fillAmount = trackBeamWidth_beta / 360f;
        TrackArea.localRotation = Quaternion.Euler(0, 0, TrackAreaAngle + (trackBeamWidth_beta / 2f));

        DebugAngle = 360f - TrackAreaAngle;
    }
    [SerializeField]float ManualRotationSpeed; // 회전 속도 (초당 도 단위)
    void MoveHeadonbeam()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A))input = 1f;
        else if (Input.GetKey(KeyCode.D)) input = -1f;

        if (input != 0f)
        {
            HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, input * ManualRotationSpeed * Time.deltaTime);
        }
    }
    void Action()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("현재 빔 각도 :"+ DebugAngle);
            float LeftAngle =   TrackAreaAngle + (trackBeamWidth_beta/2);
            if (LeftAngle > 359) 
                LeftAngle -= 360f;

            float RightAngle =  TrackAreaAngle -  (trackBeamWidth_beta/2);
            if (RightAngle < 0) 
                RightAngle = 360f + RightAngle;
            
            //Debug.Log("표현 각, 좌현 각도 :" + (360f -LeftAngle) +" 우현각도 : "+ (360f -RightAngle)); //이건 리스트상 핑검색에 사용될거
            //Debug.Log("실제 각, 좌현 각도 :" + LeftAngle +" 우현각도 : "+ RightAngle); //이건 레이더상 범위 구현에 사용할거

            //StartCoroutine(RotateOnce(LeftAngle,RightAngle));
            StartCoroutine(RotateOnce2(LeftAngle,trackBeamWidth_beta));
        }

    }
    [SerializeField] private Transform TrackBeam;
    [SerializeField] private float TrackBeamSpeed;
    IEnumerator RotateOnce(float startAngle, float endAngle)
    {
        List<Collider2D> Coll = new();
        float currentAngle = startAngle;
        Debug.Log("시작각도" + startAngle + "종단각" + endAngle);
        //커런트 값이 종단값보다  작아질경우
 
        //while (Mathf.Abs(currentAngle - endAngle) > 1f) //근사값 해결
        while (currentAngle >= endAngle)
        {
            currentAngle -= TrackBeamSpeed * Time.deltaTime;
    
            if (currentAngle < 0) currentAngle = 360f - currentAngle; 
            TrackBeam.localEulerAngles = new Vector3(0, 0, currentAngle);

            Vector3 startPosition = TrackBeam.position;
            RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(startPosition, GetVectorFromAngle(currentAngle + transform.eulerAngles.z), radarDistance);
            foreach (var rayhit in rayhitArr)
            {
                if(!Coll.Contains(rayhit.collider))
                {
                    Coll.Add(rayhit.collider);
                }
            }

            Vector3 endPosition = startPosition + GetVectorFromAngle(currentAngle + transform.eulerAngles.z) * radarDistance;
            Debug.DrawLine(startPosition, endPosition, Color.red);
            yield return null;
        }
        foreach (var collider in Coll) Debug.Log(collider.name); // 딕셔너리로 어째뜬알아서 처리
    }
    IEnumerator RotateOnce2(float startAngle, float angleWidth)
    {
        
        HashSet<(float angle ,Collider2D)> Coll = new();
      
        float currentAngle = startAngle;
        float totalRotation = Mathf.Abs(angleWidth);
        float beamShotMiddleAngle = TrackAreaAngle;

        while (totalRotation > 0f)
        {
            float deltaRotation = TrackBeamSpeed * Time.deltaTime;
        
            if (totalRotation < deltaRotation)
                deltaRotation = totalRotation;

            currentAngle -= Mathf.Sign(angleWidth) * deltaRotation;
            currentAngle = (currentAngle + 360f) % 360f;
            TrackBeam.localEulerAngles = new Vector3(0, 0, currentAngle);

            Vector3 startPosition = TrackBeam.position;
            RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(startPosition, GetVectorFromAngle(currentAngle + transform.eulerAngles.z), radarDistance);
            foreach (var rayhit in rayhitArr)
            {  
                var tuple = (currentAngle, rayhit.collider);
                if (!Coll.Any(t => t.Item2 == rayhit.collider))
                {
                    Coll.Add(tuple);
                }
            }
            //Vector3 endPosition = startPosition + GetVectorFromAngle(currentAngle + transform.eulerAngles.z) * radarDistance;
            //Debug.DrawLine(startPosition, endPosition, Color.red);

            totalRotation -= deltaRotation;
            yield return null;
        }
        
        var sortedColl = Coll.OrderBy(t => Mathf.Abs(Mathf.DeltaAngle(t.angle, beamShotMiddleAngle))).ToList();
       
        if(Lockedtarget_STT != null) Lockedtarget_STT.GetComponent<Signal>().Tracked = false; //기존 처리
       
        Lockedtarget_STT = sortedColl.Count > 0 ? sortedColl[0].Item2.gameObject : null;
        if(Lockedtarget_STT != null) Lockedtarget_STT.GetComponent<Signal>().Tracked = true; // 차후 지정
        
    }
    public GameObject Lockedtarget_STT;
    void HardLock_STT()
    {
        if (Lockedtarget_STT != null)
        {
            var STT =Lockedtarget_STT.GetComponent<Signal>();
            STT.Locked();
        }
        
    }
    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadian = angle * (Mathf.PI/180f); 
        return new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
    }
    
    // 트랙 빔 구현
    /**
    

    void SetDistenceSeach()
    {
        if(Input.GetKeyDown(KeyCode.T))
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
    //track system
    [SerializeField]
    private float trackBeamRotateSpeed = 10f;
    private float currentTrackBeamAngle = 0f;
    //track beam
    [SerializeField]private Transform trackBeamTransForm;
    [SerializeField]private Transform HeadOnBeamTransForm;
    [SerializeField, Range(0, 359)] private float trackBeamAngle = 0; 
    [SerializeField, Range(0, 360)] private float trackBeamWidth = 20;//왜임진짜?
    public float rotationSpeed = 1.5f; // 회전 속도 (초당 도 단위)
    public float nowAngle = 0;

    void TrackRadarRotate()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A))input = 1f;
        else if (Input.GetKey(KeyCode.D)) input = -1f;

        currentTrackBeamAngle = Mathf.MoveTowardsAngle(currentTrackBeamAngle,nowAngle,360f / trackBeamRotateSpeed * Time.deltaTime);
        
        trackBeamTransForm.GetComponent<Image>().fillAmount = trackBeamWidth / 360f;
        trackBeamTransForm.localRotation = Quaternion.Euler(0, 0, currentTrackBeamAngle + (trackBeamWidth / 2f) - 180f);
        
        if (input != 0f)
        {
            HeadOnBeamTransForm.localRotation *= Quaternion.Euler(0, 0, input * rotationSpeed * Time.deltaTime);
            nowAngle = HeadOnBeamTransForm.localRotation.eulerAngles.z;
        }
           
    }
    void SetTrack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("중심각 :" + currentTrackBeamAngle);
            float LeftAngle =   currentTrackBeamAngle + (trackBeamWidth/2);
            if (LeftAngle > 359) 
                LeftAngle -= 360f;

            float RightAngle =  currentTrackBeamAngle -  (trackBeamWidth/2);
            if (RightAngle < 0) 
                RightAngle = 360f + RightAngle;

            

            Debug.Log("시작각도 :" + RightAngle + "종단각도 :" +LeftAngle);
            StartCoroutine(RotateOnce(LeftAngle,RightAngle));
        } 


    }
    */


}
