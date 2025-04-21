using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Game
    { 
        get => instance ?? (instance = FindObjectOfType<GameManager>()); 
    }

    //각 시스템 접근 인스턴스

    public static DataSystem DataSystem { get; private set; }
    public static SceneSystem SceneSystem { get; private set; }
    public static NotificationSystem NotificationSystem { get; private set; }
    

    void Awake()
    {
        DataSystem ??= new DataSystem();
        SceneSystem ??= new SceneSystem(this);
        NotificationSystem ??= new NotificationSystem();
      
    }
    void Update()
    {
        if(NotificationSystem != null)
        NotificationSystem.Port.ProcessBuffer();
    }

}