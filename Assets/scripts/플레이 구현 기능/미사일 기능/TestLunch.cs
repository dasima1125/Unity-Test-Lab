using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Search;
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



        Vector3 leadPos = Calculate_LeadPosition(transform.position, TargetPos(), TargetVelocity(), 100f);
        if (leadPos == Vector3.zero) leadPos = testTargetPos.localPosition;

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

    public static Vector3 Calculate_LeadPosition(Vector3 shooterPos, Vector3 targetPos, Vector3 targetVelocity, float bulletSpeed)
    {
        Vector3 displacement = targetPos - shooterPos; //방향, 거리 백터

        float a = Vector3.Dot(targetVelocity, targetVelocity) - bulletSpeed * bulletSpeed;// a는 (타겟 속도 크기의 제곱) - (탄속의 제곱)   == 상대속도
        float b = Vector3.Dot(displacement, targetVelocity) * 2;// b는 타겟과 슈터 간 벡터(변위)와 타겟 속도의 내적을 두 배한 값  == 접근방향
        float c = Vector3.Dot(displacement, displacement);// c는 타겟과 슈터 간의 거리의 제곱  == 상대거리

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || Mathf.Abs(a) < 0.0001f)
        {
            //Debug.Log("도달 불가 : 순수추적으로 대체" );
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
        //Debug.Log("연산 성공 : 추적");
        return targetPos + targetVelocity * t;
    }
    public static Vector3 Calculate_PNAcceleration(Vector3 targetPos, Vector3 targetLastVector, Vector3 missilePos, Vector3 missilevelocity, float N)
    {
        Vector3 LOS = targetPos - missilePos;
        Vector3 targetVel = (targetPos - targetLastVector) / Time.deltaTime;
        Vector3 relativeVel = targetVel - missilevelocity;

        Vector3 O = Vector3.Cross(LOS, relativeVel) / LOS.sqrMagnitude;

        Vector3 a = -N * relativeVel.magnitude * Vector3.Cross(missilevelocity.normalized, O);

        return a;
        
    }
    
    


}
