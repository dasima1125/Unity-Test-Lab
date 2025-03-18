using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum ItemType
{
    Null,          // 시험용 빈칸 
    Consumable,    // 소모품 
    Equipment,     // 장비 
    Material,      // 재료 
    QuestItem      // 퀘스트
}

public class ItemDTO
{
    // Start is called before the first frame update

    public string ItemName;
    public int ItemQuantity;
    public Sprite ItemSprite;
    public string ItemDescription;
    public ItemType ItemCategory;

    //나중에 아이템의 종류에 따라 최대수량을 정할꺼임
    public int MaxNumberItems = 9;
    public bool IsFull = false;

    //강낭콩 (일단은 두는곳)
    public Sprite Empty;

    public ItemDTO(string ItemName, int ItemQuantity, Sprite ItemSprite, string ItemDescription,Sprite bin ,ItemType itemType = 0)
    {
        this.ItemName         = ItemName;
        this.ItemQuantity     = ItemQuantity;
        this.ItemSprite       = ItemSprite;
        this.ItemDescription  = ItemDescription;
        Empty = bin;
    }
    private ItemDTO(ItemDTO itemDTO)
    {
        ItemName = itemDTO.ItemName;
        ItemQuantity = itemDTO.ItemQuantity;
        ItemSprite = itemDTO.ItemSprite;  
        ItemDescription = itemDTO.ItemDescription;
        ItemCategory = itemDTO.ItemCategory;
        Empty         = itemDTO.Empty;
        //여긴 나중에쓸때있으면 저장
        
        MaxNumberItems = itemDTO.MaxNumberItems;
        IsFull = itemDTO.IsFull;
       
    }
    public ItemDTO CopyItemDTO()
    {
        return new ItemDTO(this);
    }


    // 아이템 추가 메소드
    public int AddItem(string ItemName, int ItemQuantity, Sprite ItemSprite,string ItemDescription,ItemType ItemCategory)
    {
        if (IsFull) return ItemQuantity;
        
        this.ItemName        =   ItemName;
        this.ItemSprite      =   ItemSprite;
        this.ItemDescription =   ItemDescription;
        this.ItemCategory    =   ItemCategory;

        this.ItemQuantity    +=  ItemQuantity;
        if (this.ItemQuantity >= MaxNumberItems)
        {
            int OverQuantity  = this.ItemQuantity - MaxNumberItems;
            this.ItemQuantity = MaxNumberItems;

            IsFull = true;
            return OverQuantity;
        }
        return 0;
        
    }
    public bool DecreaseItem(int i)
    {
        if(ItemQuantity <= 0 || ItemQuantity < i) return false;

        ItemQuantity -= i;
        IsFull = false;
        
        return true;
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
