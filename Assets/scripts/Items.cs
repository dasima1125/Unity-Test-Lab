using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string ItemName;
    [SerializeField] private int ItemQuantity;
    [SerializeField] private Sprite Sprite;   

    [TextArea]
    [SerializeField] private string ItemDescription;

    public bool CanPick = true;
   
    void Start()
    {
        
    }
    public void AddInfo(ItemDTO itemDTO)
    {
        ItemName = itemDTO.ItemName;
        ItemQuantity = itemDTO.ItemQuantity;
        Sprite = itemDTO.ItemSprite;
        ItemDescription = itemDTO.ItemDescription;
        StartCoroutine(Count());
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player" && CanPick)
        {
            Debug.Log("접촉");  
            int leftoverItme = InventoryManager.Inventory.Add_ver2(ItemName,ItemQuantity,Sprite,ItemDescription);
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
