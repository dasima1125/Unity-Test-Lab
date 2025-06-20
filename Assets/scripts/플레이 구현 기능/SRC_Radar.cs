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
        SweepBarTransForm = transform.Find("SweepBar");
        radarSpeed = 240;
        radarDistance = 150f;

        ContectList = new();
        SignalList = new();

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
        CheckTargetTrackingValid();

        Action();

        //HardLock_STT();

        //기능정리구조


    }
    void Search()
    {
        SweepBarTransForm.eulerAngles -= new Vector3(0, 0, radarSpeed * Time.deltaTime);

        RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(transform.position, GetVectorFromAngle(SweepBarTransForm.eulerAngles.z), radarDistance);
        foreach (RaycastHit2D rayhit in rayhitArr)
        {
            if (rayhit.collider != null)
                if (!ContectList.Contains(rayhit.collider))
                {
                    ContectList.Add(rayhit.collider);
                    DOVirtual.DelayedCall(1f, () => ContectList.Remove(rayhit.collider));

                    if (rayhit.collider.gameObject.GetComponent<Signal>() != null && !rayhit.collider.gameObject.GetComponent<Signal>().Track())
                    {
                        RadarPing Ping = Instantiate(RadarPing, rayhit.point, Quaternion.identity).GetComponent<RadarPing>();
                        float angle = (SweepBarTransForm.eulerAngles.z - 90f + 360f) % 360f;
                        angle = 360f - angle;

                        Ping.SetAngle(angle);
                        Ping.SetColor(Color.green);
                        SignalList.Add(Ping);

                        Ping.SetDisappearTimer(360f / radarSpeed);
                    }
                }
        }

    }
    [Header("추적 레이더 기능")]
    [SerializeField] private Transform HeadOnBeam; //해드온 방향 빔 오브젝트
    [SerializeField] private Transform TrackArea;  //추적 범위 아크 오브젝트
    [SerializeField, Range(0, 360)] private float trackBeamWidth_beta = 20;//추적범위 넒이 
    [SerializeField] private float TrackAreaRotateSpeed = 10f;//추적 범위 속도
    [SerializeField] private float TrackAreaAngle = 0f; //지금 트랙 범위가보고있는 범위
    private float DebugAngle;
    void TrackRadarArea()
    {
        TrackAreaAngle = Mathf.MoveTowardsAngle(TrackAreaAngle, HeadOnBeam.localRotation.eulerAngles.z, 360f / TrackAreaRotateSpeed * Time.deltaTime);
        TrackArea.GetComponent<Image>().fillAmount = trackBeamWidth_beta / 360f;
        TrackArea.localRotation = Quaternion.Euler(0, 0, TrackAreaAngle + (trackBeamWidth_beta / 2f));

        DebugAngle = 360f - TrackAreaAngle;
    }
    [SerializeField] float ManualRotationSpeed; // 회전 속도 (초당 도 단위)
    void MoveHeadonbeam()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input = 1f;
        else if (Input.GetKey(KeyCode.D)) input = -1f;

        if (input != 0f) HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, input * ManualRotationSpeed * Time.deltaTime);

    }
    void Action()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TrackCall());
            //Debug.Log("현재 빔 각도 :"+ DebugAngle);
            float LeftAngle = TrackAreaAngle + (trackBeamWidth_beta / 2);
            if (LeftAngle > 359)
                LeftAngle -= 360f;

            float RightAngle = TrackAreaAngle - (trackBeamWidth_beta / 2);
            if (RightAngle < 0)
                RightAngle = 360f + RightAngle;

            //Debug.Log("표현 각, 좌현 각도 :" + (360f -LeftAngle) +" 우현각도 : "+ (360f -RightAngle)); //이건 리스트상 핑검색에 사용될거
            //Debug.Log("실제 각, 좌현 각도 :" + LeftAngle +" 우현각도 : "+ RightAngle); //이건 레이더상 범위 구현에 사용할거

            //StartCoroutine(RotateOnce(LeftAngle,RightAngle));
            //StartCoroutine(ACQ(LeftAngle,trackBeamWidth_beta));
        }

    }
    [SerializeField] private Transform TrackBeam;
    [SerializeField] private float TrackBeamSpeed;

    IEnumerator ACQ(float startAngle, float angleWidth)
    {

        HashSet<(float angle, Collider2D)> Coll = new();

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

            totalRotation -= deltaRotation;
            yield return null;
        }

        var sortedColl = Coll.OrderBy(t => Mathf.Abs(Mathf.DeltaAngle(t.angle, beamShotMiddleAngle))).ToList();

        if (Lockedtarget_STT != null)
        {
            Lockedtarget_STT.GetComponent<Signal>().UnLock(); //기존 처리
            TrackLine.Decupuling();
        }
        Lockedtarget_STT = sortedColl.Count > 0 ? sortedColl[0].Item2.gameObject : null;
        if (Lockedtarget_STT != null)
        {
            Lockedtarget_STT.GetComponent<Signal>().Lock(); // 차후 지정
            TrackLine.Cupuling(Lockedtarget_STT.transform);

        }

    }
    bool TrackStandby = true;
    public TrackType trackType = TrackType.Manual;
    IEnumerator TrackCall()
    {
        if (!TrackStandby) yield break;
        TrackStandby = false;
        var Targets = new List<(Collider2D collider, float score)>();

        yield return New_ACQ(TrackAreaAngle, trackBeamWidth_beta / 2, 3, 0.3f, (callback) =>
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
            Debug.Log($"{i + 1}번째 색인.");

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

            if (hits.Length > 0) Debug.Log("  목표 감지!");
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



    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadian = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
    }

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
