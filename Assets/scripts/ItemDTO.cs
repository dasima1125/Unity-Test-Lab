using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDTO
{
    // Start is called before the first frame update

    public string ItemName;
    public int ItemQuantity;
    public Sprite ItemSprite;
    public string ItemDescription;

    //나중에 아이템의 종류에 따라 최대수량을 정할꺼임
    public int MaxNumberItems = 9;
    public bool ItemFull =false;
    public bool IsFull = false;

    //강낭콩 (일단은 두는곳)
    public Sprite Empty;


    public ItemDTO(string ItemName, int ItemQuantity, Sprite ItemSprite, string ItemDescription,Sprite bin)
    {
        this.ItemName         = ItemName;
        this.ItemQuantity     = ItemQuantity;
        this.ItemSprite       = ItemSprite;
        this.ItemDescription  = ItemDescription;
        Empty = bin;
    }

    // 아이템 추가 메소드
    public int AddItem(string ItemName, int ItemQuantity, Sprite ItemSprite,string ItemDescription)
    {
        if (IsFull) return ItemQuantity;
        
        this.ItemName        =   ItemName;
        this.ItemSprite      =   ItemSprite;
        this.ItemDescription =   ItemDescription;

        this.ItemQuantity    +=  ItemQuantity;
        //Debug.Log("현재 수량 : " + this.ItemQuantity + " 최대수량 : " + MaxNumberItems);
        if (this.ItemQuantity >= MaxNumberItems)
        {
            //Debug.Log("초과");
            int OverQuantity  = this.ItemQuantity - MaxNumberItems;
            this.ItemQuantity = MaxNumberItems;

            IsFull = true;
            return OverQuantity;
        }
        return 0;
        
    }
    public void ResetSlot()
    {
        ItemName = string.Empty;
        ItemQuantity = 0;
        
        ItemSprite = null;
        IsFull = false;
        ItemDescription = string.Empty;
    }
}
