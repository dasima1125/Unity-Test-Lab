using System.Collections;
using System.Collections.Generic;
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


    [SerializeField] private int NewItemSystem_ID; 
    [SerializeField] private int NewItemSystem_Quantity;
   

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player") && CanPick)
        {
            if(NewItemSystem_ID != 0)
            {
                var data = DataManager.data.ItemData[NewItemSystem_ID];
                if(data != null)
                {
                    var insert = DataManager.data.InventoryList;
                    insert.Add((data.ItemID,NewItemSystem_Quantity));
                }
                else
                {
                    Debug.Log("존재하지 않는 아이템ID 입니다");
                }
                
            }
            int leftoverItme = ItemSlotController.controll.Add_ver(ItemName,ItemQuantity,Sprite,ItemDescription,ItemType,EquipmentType);
            if(leftoverItme <= 0)
            {
                Destroy(gameObject);
            } 
            else
            {
                ItemQuantity = leftoverItme; //이거 없어될거같은데 << ㄴㄴ있어야함
            }
            
        }
        
    }
    public IEnumerator Count()
    {
        yield return new WaitForSeconds(3);
        CanPick = true;
    }
}
