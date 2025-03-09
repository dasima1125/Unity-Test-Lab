using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string itemName;
    [SerializeField] private int itemQuantity;
    [SerializeField] private Sprite sprite;   

    [TextArea]
    [SerializeField] private string itemDescription;

    public bool CanPick = true;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player" && CanPick)
        {
            Debug.Log("접촉");
            
            int leftoverItme = InventoryManager.Inventory.Add_ver2(itemName,itemQuantity,sprite,itemDescription);
            if(leftoverItme <= 0)
            {
                Destroy(gameObject);
            } 
            else
            {
                itemQuantity = leftoverItme; //이거 없어될거같은데 << ㄴㄴ있어야함
            }
          
        }
        
    }
    public IEnumerator Count()
    {
        yield return new WaitForSeconds(1);
        CanPick = true;
    }
}
