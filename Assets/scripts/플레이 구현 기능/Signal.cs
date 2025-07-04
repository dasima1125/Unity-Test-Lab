using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    private bool Tracked = false;
    

    public bool IsTrack()
    {
        return Tracked; 
    }
    public void Lock()
    {
        Tracked = true; 
    }
    public void UnLock()
    {
        Tracked = false;
    }

    // Update is called once per frame
    void Update()
    {
        Lock_Tracked();
    }
    public void Lock_Tracked()
    {
        if (!Tracked) return;
        //Debug.Log("락온됨 : " + gameObject.name + ", 위치 : " +gameObject.transform.position);
    }
    void OnDrawGizmos()
    {
        if (!Tracked) return;

        float width = 10f;
        float height = 10f;

        // 게임 오브젝트의 위치를 기준으로 네모 그리기
        Vector3 topLeft = transform.position;
        Vector3 topRight = new Vector3(topLeft.x + width, topLeft.y, topLeft.z);
        Vector3 bottomLeft = new Vector3(topLeft.x, topLeft.y - height, topLeft.z);
        Vector3 bottomRight = new Vector3(topLeft.x + width, topLeft.y - height, topLeft.z);

        Gizmos.color = Color.red; // Gizmos 색 설정

        // 네모 그리기
        Gizmos.DrawLine(topLeft, topRight);   // 상단
        Gizmos.DrawLine(topRight, bottomRight); // 우측
        Gizmos.DrawLine(bottomRight, bottomLeft); // 하단
        Gizmos.DrawLine(bottomLeft, topLeft);  // 좌측
    }

}
