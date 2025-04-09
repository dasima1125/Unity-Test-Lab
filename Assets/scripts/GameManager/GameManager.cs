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

    //데이터 시스템
    public static DataSystem DataSystem { get; private set; }

    public InventoryNotifier InventoryNotify { get; private set; }
    public EquipmentNotifier EquipmentNotify { get; private set; }
    
    void Awake()
    {
        Debug.Log("작동중");
        DataSystem ??= new DataSystem();
        InventoryNotify ??= new InventoryNotifier();
        EquipmentNotify ??= new EquipmentNotifier();
        //Inventory_ViewModel ??= FindObjectOfType<Inventory_ViewModel>();

        //Inventory_Model.Inventory.Init(Data);
    }

}