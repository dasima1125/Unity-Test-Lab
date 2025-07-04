using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class SearchRadar : MonoBehaviour
{
    private Transform SweepBarTransForm;
    protected Transform RadarPing;
    private Camera camera;
    [Header("-- 탐색 레이더 시스템 --")]
    [Header("--------------------------------------------------------------------------")]
    public bool EchoStandBy;
    public bool AutoSearchMode;
    public float RadarSpeed;
    public RadarRange radarDistance;
    public int WorldAngleOffset;
    private float timer = 0f;
    [SerializeField] private float interval = 0.02f; // 50Hz
    [SerializeField]private Transform RadarInfo;

    protected virtual void Start()
    {
        //TODO 어드레서블이나 리소스 로드로 개선할예정
        SweepBarTransForm = transform.Find("SweepBar");
        RadarPing = Resources.Load<Transform>("Radar/RadarPing_ver2");
        camera = GameObject.Find("SRC_RaderCam").GetComponent<Camera>();

        if (radarDistance == 0)
            radarDistance = RadarRange.Range50Km;

        SetSearchUIInfo("SRC");
    }


    protected virtual void Update()
    {
        Action();
        RangeSelect(radarDistance);
        
        if (AutoSearchMode)
            RotateBar(Time.deltaTime * RadarSpeed);

        if (EchoStandBy && TickTimer(ref timer, interval))
            Search();


    }
    Dictionary<Collider2D, RadarPing> pingMap = new();            //ping Track
    List<(RadarPing ping, bool isSelected)> pingList = new();      // ping Control
    // Prevent consecutive collisions
    HashSet<Collider2D> lastFrameDetected = new();                // Records previous detections
    HashSet<Collider2D> currentDetected = new();                  // Stores current detections


    #region Main Functionality
    void Search()
    {
        currentDetected.Clear();
        RaycastHit2D[] rayhitArr = Physics2D.RaycastAll(transform.position, GetVectorFromAngle(SweepBarTransForm.eulerAngles.z), (float)radarDistance);
        float angle = 360f - (SweepBarTransForm.eulerAngles.z + 360f - WorldAngleOffset) % 360f;
        foreach (RaycastHit2D rayhit in rayhitArr)
        {
            Collider2D collider = rayhit.collider;
            currentDetected.Add(collider);

            if (pingMap.TryGetValue(rayhit.collider, out RadarPing ping))
            {   //ReMatch

                if (lastFrameDetected.Contains(collider)) continue;

                ping.transform.position = rayhit.point;
                ping.InitSearch(angle, 5f, collider, this);

                int i = pingList.FindIndex(x => x.ping == ping);
                if (i != -1 && !pingList.Any(item => item.isSelected))
                {
                    pingList[i] = (pingList[i].ping, true);
                    pingList[i].ping.Searchselect();
                }
            }
            else
            {   //first Conctect

                var signal = collider.gameObject.GetComponent<Signal>();
                if (signal != null && !signal.IsTrack())
                {
                    RadarPing Newping = Instantiate(RadarPing, rayhit.point, quaternion.identity).GetComponent<RadarPing>();

                    Newping.InitSearch(angle, 5f, collider, this);
                    pingMap.Add(collider, Newping);

                    bool isSelected = !pingList.Any(item => item.isSelected);
                    if (isSelected)
                        Newping.Searchselect();

                    pingList.Add((Newping, isSelected));
                }
            }
        }
        (currentDetected, lastFrameDetected) = (lastFrameDetected, currentDetected);
    }
    public void RotateBar(float deltaAngle)
    {
        SweepBarTransForm.eulerAngles -= new Vector3(0, 0, deltaAngle);
    }
    #endregion

    #region Supporting Functions
    public void RemovePing(Collider2D collider, RadarPing ping)
    {
        pingList.RemoveAll(a => a.ping == ping);
        pingMap.Remove(collider);
    }
    public void RemovePingList()
    {
        var copyList = pingList.ToList();
        foreach (var item in copyList)
        {
            item.ping.KillPing();
        }
    }
    public void SelectPing()
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
    protected List<(RadarPing ping, bool isSelected)> ReadTargetList()
    {
        return pingList.ToList(); // 복사본 반환 (안전)
    }
    protected float SearchRadarAngle()
    {
        return SweepBarTransForm.eulerAngles.z;
    }
    protected void SetSweepBarVisible(bool OnOff)
    {
        SweepBarTransForm.gameObject.SetActive(OnOff);
    }
    protected void SetSearchUIInfo(string type)
    {
        if (RadarInfo == null) return;
        List<GameObject> UIs = new();
        foreach (Transform i in RadarInfo)
        {
            UIs.Add(i.gameObject);
            i.gameObject.SetActive(false);
        }
        
        var Status = RadarInfo.GetChild(0).GetComponent<TMP_Text>();
        var Radius = RadarInfo.GetChild(1).GetComponent<TMP_Text>();
        var Distance = RadarInfo.GetChild(2).GetComponent<TMP_Text>();

        switch (type)
        {
            case "SRC":
                UIs[0].SetActive(true);
                Status.text = type;
                UIs[1].SetActive(true);
                Radius.text = "360° x90°"; // 일단은 탐지범위는 냅두자.. 이건뭐 어쩔수가,,
                UIs[2].SetActive(true);
                Distance.text = (int)radarDistance + " Km*";
                break;
            case "ACQ":
                UIs[0].SetActive(true);
                Status.text = type;
                UIs[1].SetActive(true);
                Radius.text = "10° x10°"; 
                UIs[2].SetActive(true);
                Distance.text = (int)radarDistance + " Km*";
                break;

            case "TRK":
                UIs[0].SetActive(true);
                Status.text = type;
                UIs[2].SetActive(true);
                Distance.text = (int)radarDistance + " Km*";
                break;
        }
    }
    protected virtual void RangeSelect(RadarRange size)
    {
        SweepBarTransForm.localScale = new Vector3((float)size, SweepBarTransForm.localScale.y, SweepBarTransForm.localScale.z);
        Transform trail = SweepBarTransForm.Find("Trail");
        trail.localScale = new Vector3(trail.localScale.x, (float)size, trail.localScale.z);
        camera.orthographicSize = (float)size;
        SetSearchUIInfo("SRC"); //TODO 내일 반드시 개선 스트링문이 아닌 상태를 변수로 지정해야함

        foreach (var item in pingList.ToList())
        {
            item.ping.SetSize((float)size);
        }

    }
    protected virtual void Action()
    {
        if (Input.GetMouseButtonDown(2))
        {
            SelectPing();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var values = (RadarRange[])Enum.GetValues(typeof(RadarRange));
            int i = Array.IndexOf(values, radarDistance);
            radarDistance = values[(i + 1) % values.Length];  
        }

    }
    #endregion

    #region Tools
    protected bool TickTimer(ref float timer, float interval)
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            return true;
        }
        return false;
    }
    protected float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
    protected Vector3 GetVectorFromAngle(float angle)
    {
        float angleRadian = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
    }
    protected float  GetRelativeAzimuth(Transform targetPos)
    {
        Vector2 d = targetPos.position - transform.position;
        float world = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        return  360f - (NormalizeAngle(world - transform.eulerAngles.z) - WorldAngleOffset) % 360;
    }
    protected virtual void OnDisable()
    {
        RemovePingList();
    }

    #endregion
    public enum RadarRange
    {
        Range50Km = 50,
        Range100Km = 100,
        Range150Km = 150,
        Range300Km = 300,
    }


}

