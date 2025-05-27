using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TestLunch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject testBullet;

    public Transform testTargetPos;
    void Start()
    {
        
    }

    public void LaunchTest()
    {
        Debug.Log("테스트 발사됨");
        TestBullet test = Instantiate(testBullet,transform.localPosition,quaternion.identity).GetComponent<TestBullet>();
        test.init(testTargetPos.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
