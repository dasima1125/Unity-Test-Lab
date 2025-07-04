
using System;
using UnityEngine;

public class RadarPing : MonoBehaviour
{
    private SearchRadar SRC_2;
    private Transform MiniMapMark;
    private SpriteRenderer spriteRenderer;
    private float disappearTimer;
    private float disappearTimerMax;
    private Color color;
    private Vector3 baseScale;
    //new system
    private float pingAngle;
    private PingType pingType;


    //에라 모르겠다진짜.. 하
    private Collider2D collider;

    private void Awake()
    {
        
        //spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        color = Color.green;

    }
    public void InitSearch(float angle, float life, Collider2D collider, SearchRadar radar) //의존성주입으로 직접 클래스연결
    {
        SRC_2 = radar;
        pingAngle = angle;
        pingType = PingType.Search;

        disappearTimerMax = life;
        disappearTimer = 0f;

        this.collider = collider;
        MiniMapMark = transform.Find("SearchPing");
        if (MiniMapMark != null)
        {
            MiniMapMark.gameObject.SetActive(true);
        }

    }
    public void InitTrack(Collider2D collider) //의존성주입으로 직접 클래스연결
    {
        this.collider = collider;
        pingType = PingType.Track;
        MiniMapMark = transform.Find("TrackPing");
        if (MiniMapMark != null)
        {
            MiniMapMark.gameObject.SetActive(true);
        }
    }
    void Start()
    {
        spriteRenderer = MiniMapMark.GetComponent<SpriteRenderer>();
    }
    public void Init_Regucy(float angle, float life, Collider2D collider)
    {

        pingAngle = angle;

        disappearTimerMax = life;
        disappearTimer = 0f;

        this.collider = collider;

    }

    private void Update()
    {
        if (pingType == PingType.Search)
            SearchPing();
        if (pingType == PingType.Track)
            TrackPing();
        

    }
    void SearchPing()
    {
        disappearTimer += Time.deltaTime;

        color.a = Mathf.Lerp(disappearTimerMax, 0f, disappearTimer / disappearTimerMax);

        spriteRenderer.color = color;

        if (disappearTimer >= disappearTimerMax)
        {
            KillPing();
        }

    }
    void TrackPing()
    {
        if (!collider.GetComponent<Signal>().IsTrack())
        {
            Destroy(gameObject);
        }
        transform.localPosition = collider.gameObject.transform.localPosition;
        
    }
    public void SetSize(float range)
    {
        float baseRange = 150f; // 기준 거리
        float scale = range / baseRange;
        transform.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, 1f); // 2D라면 Z는 1 유지
    }

    public void Searchselect()
    {
        transform.Find("SelectTarget").gameObject.SetActive(true);
    }
    public void Desearchselect()
    {
        transform.Find("SelectTarget").gameObject.SetActive(false);
    }
    public float GetAngle()
    {
        return pingAngle;
    }
    public void KillPing()
    {
        SRC_2.RemovePing(collider, this);
        if (this != null)
            Destroy(gameObject);
    }

}
public enum PingType
{
    Search,
    Track
}
