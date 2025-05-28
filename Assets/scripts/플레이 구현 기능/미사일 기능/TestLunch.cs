using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TestLunch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject testBullet;

    public Transform testTargetPos;
    public Transform readTargetPos;
    void Start()
    {

    }

    public void LaunchTest()
    {

        TestBullet test = Instantiate(testBullet, transform.localPosition, quaternion.identity).GetComponent<TestBullet>();
        //test.Init(testTargetPos.localPosition, this);

        Rigidbody2D targetRb = testTargetPos.GetComponent<Rigidbody2D>();
        Vector2 targetVelocity = targetRb != null ? targetRb.velocity : Vector2.zero;

        Vector2 leadPos = CalculateLeadPosition(
            transform.position,
            testTargetPos.position,
            targetVelocity,
            100f // 발사체 속도, testBullet 속도와 동일하게 유지
        );


        test.Init(leadPos, this);

    }

    // Update is called once per frame
    public Vector3 TargetPos()
    {
        if (testTargetPos == null) return Vector2.zero;
        return testTargetPos.position;
    }
    public Vector3 TargetVelocity()
    {
        if (testTargetPos == null) return Vector3.zero;
        return testTargetPos.GetComponent<Rigidbody2D>().velocity;
    }
    /// <summary>
    /// 선형 리드 추적 함수
    /// </summary>
    /// <param name="shooterPos">발사자의 현재 위치</param>
    /// <param name="targetPos">타겟의 현재 위치</param>
    /// <param name="targetVelocity">타겟의 현재 속도</param>
    /// <param name="bulletSpeed">발사체의 속도</param>
    /// <returns></returns>
    public static Vector3 CalculateLeadPosition(Vector3 shooterPos, Vector3 targetPos, Vector3 targetVelocity, float bulletSpeed)
    {
        Vector3 displacement = targetPos - shooterPos;

        float a = Vector3.Dot(targetVelocity, targetVelocity) - bulletSpeed * bulletSpeed;
        // a는 (타겟 속도 크기의 제곱) - (탄속의 제곱)  
        // 이 값은 탄속과 타겟 속도 크기의 상대적인 차이를 나타내며, 이차방정식의 2차항 계수 역할
        float b = Vector3.Dot(displacement, targetVelocity) * 2;
        // b는 타겟과 슈터 간 벡터(변위)와 타겟 속도의 내적을 두 배한 값  
        // 이는 타겟이 슈터를 향해 접근하는 정도(또는 멀어지는 정도)를 나타내며, 1차항 계수 역할
        float c = Vector3.Dot(displacement, displacement);
        // c는 타겟과 슈터 간의 거리의 제곱  
        // 이 값은 초기 거리의 크기를 나타내며, 상수항 역할

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            Debug.Log("도달 불능");
            return Vector3.zero;
        }

        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t = Mathf.Max(t1, t2);
        // if (t < 0)
        // {
        //     Debug.LogWarning("타임스톤이라도 썼나요?.");
        //     return targetPos;
        // }
        return targetPos + targetVelocity * t;
    }
    
    
    void Update()
    {
        
        
    }
}
