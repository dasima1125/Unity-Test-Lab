using UnityEngine.UI;
using UnityEngine;

public class AddTrackingRadar : SearchRadar
{
    [Header("--SRC & TRK 레이더 기능--")]
    [Header("--------------------------------------------------------------------------")]
    [SerializeField] float ManualRotationSpeed;
    [SerializeField] TrackType trackType = TrackType.Manual;
    [SerializeField] private Signal hardLockTarget;

    
    [SerializeField, Range(0, 360)] private float TrackZoneWidth;
    [SerializeField] private float TrackAreaRange = 0f;
    [SerializeField] private float TrackAreaRotateSpeed = 0f;         
    private float TrackAreaAngle = 0f;
    

    public GameObject trail;//이거도 나중에 습득형으로 개선
    public LineLenderTest TrackLine;//이거도
    bool TrackStandby;


    private Transform HeadOnBeam;
    private Transform TrackArcRange;

    // 점선 구획
    private LineRenderer DottedLine;
    private Vector3 baseScale;
    private float baseTextureScaleX = 0.06f;
    private float baseLineWidth = 0.7f;


    protected override void Start()
    {
        base.Start();

        HeadOnBeam = transform.Find("HeadOnWay");
        HeadOnBeam.gameObject.SetActive(true);

        TrackArcRange = transform.Find("TrackArea").GetChild(0);
        TrackArcRange.gameObject.SetActive(true);

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
        UpdateTrackAreaDirection();
        ValidHardLock();
    }
    protected override void Action()
    {
        base.Action();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            MoveHeadonbeamTrack(Input.GetKey(KeyCode.A) ? 1f : -1);
        }

    }
    void ValidHardLock()
    {
        if (hardLockTarget == null) return;
        float distance = Vector3.Distance(transform.position, hardLockTarget.transform.position);

        if (distance > TrackAreaRange ||!rangeChecker(GetRelativeAzimuth(hardLockTarget.transform)))
        {
            ReleaseHardLock();
            hardLockTarget.UnLock();
            hardLockTarget = null;
        }
        

    }
   


    void MoveHeadonbeamTrack(float way = 0f)
    {
        if (way != 0f) HeadOnBeam.localRotation *= Quaternion.Euler(0, 0, way * ManualRotationSpeed * Time.deltaTime);
    }

    bool rangeChecker(float angle)
    {
        var startAngle = NormalizeAngle(TrackAreaAngle - TrackZoneWidth * 0.5f);
        var endAngle = NormalizeAngle(TrackAreaAngle + TrackZoneWidth * 0.5f);
        var sector = NormalizeAngle(endAngle - startAngle);
        var delta = NormalizeAngle(angle - startAngle);

        //Debug.Log($"방향각 : {angle} , 시작각 {startAngle}, 종단각 {endAngle}, 조건여부 : { delta <= sector}");
        return delta <= sector;
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

    void UpdateTrackAreaDirection()
    {
        TrackAreaAngle = (360f - TrackAreaAngle + WorldAngleOffset) % 360f;

        TrackAreaAngle = Mathf.MoveTowardsAngle(TrackAreaAngle, HeadOnBeam.localRotation.eulerAngles.z, TrackAreaRotateSpeed * Time.deltaTime);
        TrackArcRange.GetComponent<Image>().fillAmount = TrackZoneWidth / 360f;
        TrackArcRange.localRotation = Quaternion.Euler(0, 0, TrackAreaAngle + (TrackZoneWidth / 2f));
        
        TrackAreaAngle = 360 - (TrackAreaAngle + 360f - WorldAngleOffset)% 360f;

    }
 
}
