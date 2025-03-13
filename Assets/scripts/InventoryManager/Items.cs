using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string ItemName;
    [SerializeField] private int ItemQuantity;
    [SerializeField] private ItemType ItemType;   
    [SerializeField] private Sprite Sprite;   

    [TextArea]
    [SerializeField] private string ItemDescription;

    public bool CanPick = true;
   

    public void DropItem(ItemDTO itemDTO,int i)
    {
        ItemName = itemDTO.ItemName;
        Sprite = itemDTO.ItemSprite;
        GetComponent<SpriteRenderer>().sprite = Sprite;
        ItemDescription = itemDTO.ItemDescription;
        
        ItemQuantity = i;
        StartCoroutine(Count());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player" && CanPick)
        {
            Debug.Log("접촉");  
            int leftoverItme = ItemSlotController.controll.Add_ver(ItemName,ItemQuantity,Sprite,ItemDescription);
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
