using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

public class Items : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string ItemName;
    [SerializeField] private int ItemQuantity;
    [HideInInspector] public ItemType ItemType;
    [HideInInspector] public EquipmentType EquipmentType;      
    [SerializeField] private Sprite Sprite;   

    [TextArea]
    [SerializeField] private string ItemDescription;

    public bool CanPick = true;
    private static DataCommandHandler _data;
    void Awake()
    {
        _data =GameManager.DataSystem.commandHandler;
        
    }
    

    void Start()
    {
        //셀프 업데이트
        if(NewItemSystem_ID != 0 && NewItemSystem_Quantity != 0)
        {
            var data = _data.Execute_GetItemSOID(NewItemSystem_ID);
            GetComponent<SpriteRenderer>().sprite = data.Sprite;
        }
        
    }
    
    public void DropItem(ItemDTO itemDTO,int i)
    {
        ItemName = itemDTO.ItemName;
        Sprite = itemDTO.ItemSprite;
        ItemType = itemDTO.ItemCategory;
        EquipmentType = itemDTO.EquipmentCategory;
        GetComponent<SpriteRenderer>().sprite = Sprite;
        ItemDescription = itemDTO.ItemDescription;
        
        ItemQuantity = i;
        StartCoroutine(Count());
    }
    // SO기반 시스템
    [SerializeField] private int NewItemSystem_ID; 
    [SerializeField] private int NewItemSystem_Quantity;
    
    public void Setup(int id,int quantity)
    {
        var data = _data.Execute_GetItemSOID(id);
        CanPick = false;
        if(data == null) return;

        NewItemSystem_ID = data.ItemID;
        NewItemSystem_Quantity = quantity;
        GetComponent<SpriteRenderer>().sprite = data.Sprite;
        StartCoroutine(Count());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player") && CanPick)
        {
            
            if(NewItemSystem_ID != 0)
            {
                var data = _data.Execute_GetItemSOID(NewItemSystem_ID);
                if(data != null && _data != null)
                {
                    int count = _data.Execute_IncreaseItem(NewItemSystem_ID,NewItemSystem_Quantity);
        
                    //GameManager.NotificationSystem.Port.Send("InventorySystem","UpdateAllSlot",this);
                    GameManager.NotificationSystem.Port.Send("InventorySystem","UpdateAllSlot",this,null,BufferSpeed.Fast);
                    GameManager.NotificationSystem.Port.Send("EquipmentSystem","HandleEquipmentAcquisition",this,NewItemSystem_ID,BufferSpeed.Fast);

                    if(count <= 0) Destroy(gameObject);
                    else NewItemSystem_Quantity = count;

                }
                else
                {
                    Debug.Log("모듈이 존재하지 않습니다. " 
                        + " 데이터 모듈 : " + (data != null ? "True" : "Null") 
                        + " 인벤토리 시스템 모듈 : " + (Inventory_ViewModel.Inventory != null ? "True" : "Null"));

                }
            }
           
        }
        
    }
    public IEnumerator Count()
    {
        yield return new WaitForSeconds(3);
        CanPick = true;
    }
}
