using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Linq;



public class SRC_Radar : MonoBehaviour
{
    private Transform SweepBarTransForm;
    [SerializeField] private Transform RadarPing;

    [Header("탐색 레이더 시스템")]
    public bool EchoStandBy;
    public bool AutoSearchMode;

    public float radarSpeed;
    private float radarDistance;
    [HideInInspector]
    public int size = 1;

    void Awake()
    {
        SweepBarTransForm = transform.Find("SweepBar");// 스윕 바 부여

        radarDistance = 150f;
    }
    
    void Update()
    {
        if (AutoSearchMode)
            RotateSweepBar(radarSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;  // interval만큼 뺌 (누적 오차 최소화)
            if(EchoStandBy)
            Search();
        }

        //TrackRadarArea();
        Action();
        //CheckTargetTrackingValid();
        //HardLock_STT();

    }
    //타이밍 연구중
    float timer = 0f;
    float interval = 0.02f; // 50Hz
    // 0.01 100Hz
    // 0.0075 150hz

    public void RemovePing(Collider2D collider, RadarPing ping)
    {
        pingList.RemoveAll(a => a.ping == ping);
        pingMap.Remove(collider);
        //if (ping == selectPing)
        //    selectPing = null;
    }
    public void RemovePingList()
    {
        foreach (var item in pingList.ToList())
        {
            item.ping.KillPing();
        }
    }
    void MoveHeadonbeamSingle(float way = 0)
    {
        if (way != 0f) HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, way * ManualRotationSpeed * Time.deltaTime);
    }
 
    private Dictionary<Collider2D, RadarPing> pingMap = new();// 핑 존재여부 확인용
    private List<(RadarPing ping,bool isSelected)> pingList = new(); // 핑 제어용

    //RadarPing selectPing; TODO :차후 성능 계량을 위한 리펙터링 예정
    //int selectPingIndex;        선택된 핑 인덱스 추적용 

    //연속 콜라이드 방지용
    HashSet<Collider2D> lastFrameDetected = new(); //기록용
    HashSet<Collider2D> currentDetected = new(); //갱신용 
    void Search()
    {
        currentDetected.Clear();
        RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(transform.position, GetVectorFromAngle(SweepBarTransForm.eulerAngles.z), radarDistance);
        foreach (RaycastHit2D rayhit in rayhitArr)
        {
            Collider2D collider = rayhit.collider;
            currentDetected.Add(collider);

            if (pingMap.TryGetValue(rayhit.collider, out RadarPing ping))
            {
                //ReMatch
                if (lastFrameDetected.Contains(collider)) continue;
                float angle = 360f - (SweepBarTransForm.eulerAngles.z - 90f + 360f) % 360f;
                ping.transform.position = rayhit.point;
                ping.Init_Regucy(angle, 5f, collider);
                // 핑제어부
                int i = pingList.FindIndex(x => x.ping == ping);
                if (i != -1 && !pingList.Any(item => item.isSelected))
                {
                    pingList[i] = (pingList[i].ping, true);
                    pingList[i].ping.Searchselect();
                }                    
            }
            else
            {
                //first Conctect
                var signal = collider.gameObject.GetComponent<Signal>();
                if (signal != null && !signal.IsTrack())
                {
                    RadarPing NewPing = Instantiate(RadarPing, rayhit.point, Quaternion.identity).GetComponent<RadarPing>();
                    float angle = 360f - (SweepBarTransForm.eulerAngles.z - 90f + 360f) % 360f;

                    NewPing.Init_Regucy(angle, 5f, collider);
                    pingMap.Add(collider, NewPing);

                    // 핑제어부 --> 문제는 이거 자원을 엄청먹을거같단말이지..
                    if (!pingList.Any(item => item.isSelected))
                    {
                        NewPing.Searchselect();
                        pingList.Add((NewPing, true));
                    }
                    else
                        pingList.Add((NewPing, false));
                }
            }
        }
        (currentDetected, lastFrameDetected) = (lastFrameDetected, currentDetected);
    }
    void SelectPing()
    {
        if (pingList.Count < 1) return;
        int i = pingList.FindIndex(x => x.isSelected == true);
        if (i == -1) // none
        {
            pingList[0] = (pingList[0].ping, true);
            pingList[0].ping.Searchselect();
        }
        else // change
        {
            pingList[i] = (pingList[i].ping, false);
            pingList[i].ping.Desearchselect();

            int targetIndex = (i + 1 == pingList.Count) ? 0 : i + 1;
            pingList[targetIndex] = (pingList[targetIndex].ping, true);
            pingList[targetIndex].ping.Searchselect();
        }
    }
    protected void RotateSweepBar(float deltaAngle)
    {
        SweepBarTransForm.eulerAngles -= new Vector3(0, 0, deltaAngle);
    }
    

    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadian = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
    }


    [Header("추적 레이더 기능")]
    [SerializeField] private Transform HeadOnBeam; //해드온 방향 빔 오브젝트
    [SerializeField] private Transform TrackArea;  //추적 범위 아크 오브젝트
    [SerializeField, Range(0, 360)] private float trackBeamWidth_beta = 20;//추적범위 넒이 
    [SerializeField] private float TrackAreaRotateSpeed = 10f;//추적 범위 속도
    [SerializeField] private float TrackAreaAngle = 0f; //지금 트랙 범위가보고있는 범위

    [SerializeField] private Signal hardLockTarget;

    //이건 조만간 없앨꺼임 중요
    public GameObject trail;
   

    IEnumerator TrackCallSingle()
    {
        if (hardLockTarget != null) // 이미 락이있을때
        {
            trail.SetActive(true);
           
            TrackLine.Decupuling();
            AutoSearchMode = true;
            EchoStandBy = true;
            hardLockTarget = null;
            yield break;
        }

        if (!TrackStandby) yield break; // 중복 작동 방지
        trail.SetActive(false);

        TrackStandby = false;
        EchoStandBy = false;
        AutoSearchMode = false;
        
        Debug.Log("실행");

        var Targets = new List<(Collider2D collider, float score)>();// ACQ 타겟 저장용
        float TargetAngle = 0f; // 목표 대상
        foreach (var (ping, isSelected) in pingList) //대상 앵글 획득 => 튜플이라 Find쓰기엔 모호함 어쩔수없음
        {
            if (isSelected)
            {
                TargetAngle = ping.GetAngle();
                break;
            }
        }
        RemovePingList();//신호 초기화
        while (true)
        {
            float correctedAngle = (450f - SweepBarTransForm.eulerAngles.z) % 360f;
            float delta = Mathf.DeltaAngle(correctedAngle, TargetAngle);

            if (Mathf.Abs(delta) < 0.2f)
                break;

            float direction = Mathf.Sign(delta);
            RotateSweepBar(radarSpeed * Time.deltaTime * direction);
            yield return null;
    
        }

        yield return New_ACQ(SweepBarTransForm.eulerAngles.z - 90, trackBeamWidth_beta / 2, 1, 0.1f, (callback) =>
        {
            Collider2D[] hits = callback;
            foreach (Collider2D coli in hits)
            {
               
                Vector2 direction = coli.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;
                float angleOffset = Mathf.Abs(Mathf.DeltaAngle(TrackAreaAngle + 90, angle));

                float distance = Vector2.Distance(coli.transform.position, transform.position);
                var weight = trackType.GetWeights();
                float score = (weight.weightAngle * angleOffset) + (weight.weightRange * distance);

                Targets.Add((coli, score));
            }

            Targets.Sort((a, b) => a.score.CompareTo(b.score));

        });
        Debug.Log("검색한 수량 " + Targets.Count)  ;

        
        if (Targets.Count <= 0) //목표가 없을때
        {
            trail.SetActive(true);
            AutoSearchMode = true;
            EchoStandBy = true;

            TrackStandby = true;
            TrackLine.Decupuling();            
            yield break;
        }
        TrackLine.Cupuling(Targets[0].collider.transform);
        hardLockTarget = Targets[0].collider.GetComponent<Signal>(); 

        Targets.Clear();
        
        
        //명령 복귀

        Debug.Log("종료");
        TrackStandby = true;
        yield return null;
    }
    

    void TrackRadarArea()
    {
        TrackAreaAngle = Mathf.MoveTowardsAngle(TrackAreaAngle, HeadOnBeam.localRotation.eulerAngles.z, 360f / TrackAreaRotateSpeed * Time.deltaTime);
        TrackArea.GetComponent<Image>().fillAmount = trackBeamWidth_beta / 360f;
        TrackArea.localRotation = Quaternion.Euler(0, 0, TrackAreaAngle + (trackBeamWidth_beta / 2f));
    }
    [SerializeField] float ManualRotationSpeed; // 회전 속도 (초당 도 단위)
    void MoveHeadonbeamTrack()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input = 1f;
        else if (Input.GetKey(KeyCode.D)) input = -1f;

        if (input != 0f) HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, input * ManualRotationSpeed * Time.deltaTime);
    }
    void Action()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemovePingList();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TrackCallSingle());
        }
        if (Input.GetMouseButtonDown(2))
        {
            SelectPing();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            MoveHeadonbeamSingle(Input.GetKey(KeyCode.A) ? 1f : -1);
        }
        
    }

    bool TrackStandby = true;
    public TrackType trackType = TrackType.Manual;
    IEnumerator TrackCall()
    {
        if (!TrackStandby) yield break;
        TrackStandby = false;
        var Targets = new List<(Collider2D collider, float score)>();

        yield return New_ACQ(TrackAreaAngle, trackBeamWidth_beta / 2, 1, 0.3f, (callback) =>
        {
            Collider2D[] hits = callback;
            foreach (Collider2D coli in hits)
            {
                Vector2 direction = coli.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;
                float angleOffset = Mathf.Abs(Mathf.DeltaAngle(TrackAreaAngle + 90, angle));
                float distance = Vector2.Distance(coli.transform.position, transform.position);
                var weight = trackType.GetWeights();

                float score = (weight.weightAngle * angleOffset) + (weight.weightRange * distance);
                Targets.Add((coli, score));
            }
            Targets.Sort((a, b) => a.score.CompareTo(b.score));

        });
        if (Targets.Count <= 0)
        {
            TrackLine.Decupuling();//이건 언제든 바뀔수있는조건이기도 함
            TrackStandby = true;
            yield break;
        }
        TrackLine.Decupuling();
        TrackLine.Cupuling(Targets[0].collider.transform);

        Targets.Clear();
        TrackStandby = true;
        //명령 복귀
        yield return null;


    }
    IEnumerator New_ACQ(float pointAngle, float ArcWidth , int sectorCount, float Duration, Action<Collider2D[]> result)
    {
        float anglePerSector = ArcWidth * 2f / sectorCount;   // 각 섹터당 각도 크기 => (아크 절대값 * 2) / 섹터 수
        float startAngle = pointAngle - ArcWidth + 90;   // 시작점 => 중심각 - 아크 절대값 => 후보정90도 꺽여있음

        HashSet<Collider2D> storage = new();

        for (int i = 0; i < sectorCount; i++)
        {
            float sectorStart = NormalizeAngle(startAngle + anglePerSector * i);
            float sectorEnd = NormalizeAngle(startAngle + anglePerSector * (i + 1));

            ///디버그용
            Vector3 center = transform.position;
            Vector3 startDir = new(Mathf.Cos(sectorStart * Mathf.Deg2Rad), Mathf.Sin(sectorStart * Mathf.Deg2Rad), 0);
            Vector3 endDir = new(Mathf.Cos(sectorEnd * Mathf.Deg2Rad), Mathf.Sin(sectorEnd * Mathf.Deg2Rad), 0);

            Vector3 startPoint = center + startDir * 150f;
            Vector3 endPoint = center + endDir * 150f;

            Debug.DrawLine(center, startPoint, Color.green, Duration / sectorCount);
            Debug.DrawLine(center, endPoint, Color.green, Duration / sectorCount);
            Debug.DrawLine(startPoint, endPoint, Color.green, Duration / sectorCount);
          
            ///뭐 지워도됨 성능이나 기능상 문제는 없음

            Collider2D[] hits = AreaScan.Arc2D(transform.position, 150f, sectorStart, sectorEnd);

            if (hits.Length > 0) Debug.Log($"{i + 1}섹터  목표 감지!");
            storage.UnionWith(hits);
            yield return new WaitForSeconds(Duration / sectorCount);
        }
        result?.Invoke(storage.ToArray());
        yield return null;
    }
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    public GameObject Lockedtarget_STT;
    public LineLenderTest TrackLine;
    /**

    void CheckTargetTrackingValid()
    {
        if (Lockedtarget_STT == null) return;

        float angleToTarget = Mathf.Atan2(Lockedtarget_STT.transform.position.y
                                    - transform.position.y, Lockedtarget_STT.transform.position.x
                                    - transform.position.x) * Mathf.Rad2Deg;

        angleToTarget -= transform.eulerAngles.z;

        if (angleToTarget < 0) angleToTarget += 360f;
        else if (angleToTarget >= 360) angleToTarget -= 360f;


        //Debug.Log("추적중 : " + Lockedtarget_STT.name + ", 각도 : " + angleToTarget);
    }
    */



    

}
// TODO 차후 위치 분할 
public enum TrackType
{
    TWS,
    Manual,
    ProximityDefense
}
public static class TrackTypeExtensions
{
    public static (float weightAngle, float weightRange) GetWeights(this TrackType type)
    {
        return type switch
        {
            TrackType.TWS => (1.0f, 0.01f),
            TrackType.Manual => (0.7f, 0.3f),
            TrackType.ProximityDefense => (0.1f, 1.0f),
            _ => (1.0f, 1.0f)
        };
    }
}
