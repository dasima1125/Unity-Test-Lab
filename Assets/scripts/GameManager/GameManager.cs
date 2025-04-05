using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameManager instance;
    public static GameManager Game
    { 
        get => instance ?? (instance = FindObjectOfType<GameManager>()); 
    }

    //각 시스템 접근 인스턴스
    public static DataSystem Data { get; private set; }
    
    void Awake()
    {
        Debug.Log("작동중");
        Data ??= new DataSystem();
        Inventory_Model.Inventory.Init(Data);
    }

}
