using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class MonoRadar : SearchRadar
{


    [Header("--단일 추적 레이더 기능--")]
    [Header("--------------------------------------------------------------------------")]
    [SerializeField] float ManualRotationSpeed;
    [SerializeField] TrackType trackType = TrackType.Manual;

    [SerializeField] private Signal hardLockTarget;//나중에 알아서 숨겨야함
    public GameObject trail;//이거도 나중에 습득형으로 개선
    public LineLenderTest TrackLine;//이거도
    bool TrackStandby;




    private Transform HeadOnBeam;
    private LineRenderer DottedLine;

    [SerializeField] private Vector3 baseScale;
    private float baseTextureScaleX = 0.06f;
    private float baseLineWidth = 0.7f;


    protected override void Start()
    {
        base.Start();

        HeadOnBeam = transform.Find("HeadOnWay");
        HeadOnBeam.gameObject.SetActive(true);
        DottedLine = transform.Find("DottedLine").GetComponent<LineRenderer>();
        baseScale = HeadOnBeam.localScale;

        WorldAngleOffset = 90;
        RadarSpeed = 150f;

        EchoStandBy = true;
        TrackStandby = true;
        AutoSearchMode = true;
    }


    protected override void Update()
    {
        base.Update();

        ValidHardLock();
    }
    protected override void Action()
    {
        base.Action();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            MoveHeadonbeamSingle(Input.GetKey(KeyCode.A) ? 1f : -1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //TODO 싱글 트랙 시퀸스 = > 이건 그냥흠... 부모 탐색레이더에넣고 액션을 자식이 올리는 이벤트로 넘기는방법을써될거같기도하고..,
            StartCoroutine(TrackOrder());
        }
    }
    #region Main Functionality
    IEnumerator TrackOrder()
    {
        if (!TrackStandby) yield break;

        if (hardLockTarget != null) // 락 걸린 상태일경우
        {
            ReleaseHardLock();
            hardLockTarget.UnLock();
            hardLockTarget = null;

            yield break;
        }

        TrackStandby = false;
        EchoStandBy = false;
        AutoSearchMode = false;
        trail.SetActive(false);

        var Targets = new List<(Collider2D collider, float score)>();
        float TargetAngle = -1f;

        foreach (var (ping, isSelected) in ReadTargetList()) //대상 앵글 획득 => 튜플이라 Find쓰기엔 모호함 어쩔수없음
            if (isSelected) { TargetAngle = ping.GetAngle(); break; }

        if (TargetAngle < 0)
        {
            ReleaseHardLock();
            TrackStandby = true;
            yield break;
        }
        RemovePingList();
        SetSearchUIInfo("ACQ");

        while (true)
        {
            float correctedAngle = (360 + WorldAngleOffset - SearchRadarAngle()) % 360f; // 450
            float delta = Mathf.DeltaAngle(correctedAngle, TargetAngle);

            if (Mathf.Abs(delta) < 0.2f)
                break;

            float direction = Mathf.Sign(delta);
            RotateBar(RadarSpeed * 2 * Time.deltaTime * direction);
            yield return null;
        }
        yield return ACQ(SearchRadarAngle() - WorldAngleOffset, 10f / 2f, 1, 0.1f, (callback) =>
        {
            Collider2D[] hits = callback;
            foreach (Collider2D coli in hits)
            {
                Vector2 direction = coli.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;

                float angleOffset = Mathf.Abs(Mathf.DeltaAngle(SearchRadarAngle() + WorldAngleOffset, angle));
                float distance = Vector2.Distance(coli.transform.position, transform.position);
                var weight = trackType.GetWeights();

                float score = (weight.weightAngle * angleOffset) + (weight.weightRange * distance);
                Targets.Add((coli, score));
            }
            Targets.Sort((a, b) => a.score.CompareTo(b.score));

        });

        if (Targets.Count <= 0) //None Target
        {
            ReleaseHardLock();
            TrackStandby = true;
            yield break;
        }
        SetSweepBarVisible(false);
        SetSearchUIInfo("TRK");

        TrackLine.Cupuling(Targets[0].collider.transform);
        hardLockTarget = Targets[0].collider.GetComponent<Signal>();
        hardLockTarget.Lock();

        RadarPing Newping = Instantiate(RadarPing, Targets[0].collider.transform.position, quaternion.identity).GetComponent<RadarPing>();
        Newping.InitTrack(Targets[0].collider);

        TrackStandby = true;
    }
    IEnumerator ACQ(float pointAngle, float ArcWidth, int sectorCount, float Duration, Action<Collider2D[]> result)
    {
        float anglePerSector = ArcWidth * 2f / sectorCount;   // 각 섹터당 각도 크기 => (아크 절대값 * 2) / 섹터 수
        float startAngle = pointAngle - ArcWidth + 90;   // 시작점 => 중심각 - 아크 절대값 => 후보정90도 꺽여있음

        HashSet<Collider2D> storage = new();

        for (int i = 0; i < sectorCount; i++)
        {
            float sectorStart = NormalizeAngle(startAngle + anglePerSector * i);
            float sectorEnd = NormalizeAngle(startAngle + anglePerSector * (i + 1));

            Collider2D[] hits = AreaScan.Arc2D(transform.position, 150f, sectorStart, sectorEnd);
            storage.UnionWith(hits);
            yield return new WaitForSeconds(Duration / sectorCount);
        }
        result?.Invoke(storage.ToArray());
        yield return null;
    }
    void ValidHardLock()
    {
        if (hardLockTarget == null) return;

        float distance = Vector3.Distance(transform.position, hardLockTarget.transform.position);
        if (distance > (float)radarDistance)
        {
            ReleaseHardLock();
            hardLockTarget.UnLock();
            hardLockTarget = null;
        }
    }
    #endregion

    #region Supporting Functionality
    // TODO 무장 자동 보정기능 추가할예정인데 이건 무장과 통합시 하자고
    void MoveHeadonbeamSingle(float way = 0f)
    {
        if (way != 0f) HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, way * ManualRotationSpeed * Time.deltaTime);
    }
    void ReleaseHardLock()
    {
        trail.SetActive(true);
        TrackLine.Decupuling();
        SetSweepBarVisible(true);

        AutoSearchMode = true;
        EchoStandBy = true;
        SetSearchUIInfo("SRC");
    }
    protected override void RangeSelect(RadarRange size)
    {
        base.RangeSelect(size);

        float baseRange = 150f;
        float scale = (float)size / baseRange;
        HeadOnBeam.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, 1);
        if (DottedLine != null)
        {
            DottedLine.widthMultiplier = baseLineWidth * scale;
            DottedLine.textureScale = new Vector2(baseTextureScaleX / scale, DottedLine.textureScale.y);
        }
    }
    #endregion
    protected override void OnDisable()
    {
        base.OnDisable();

        ReleaseHardLock();
        
        if (hardLockTarget != null)
        hardLockTarget.UnLock();
        hardLockTarget = null;
        
    }
}
