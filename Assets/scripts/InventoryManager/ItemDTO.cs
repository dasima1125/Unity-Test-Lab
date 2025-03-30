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
public enum EquipmentType
{
    Null,
    MainWeapon,
    SubWeapon,
    MeleeWeapon,
    Tools,
    Head,    
    Body,     
    Leg,      
    Boots      
}

public class ItemDTO
{
    // Start is called before the first frame update

    public string ItemName;
    public int ItemQuantity;
    public Sprite ItemSprite;
    public string ItemDescription;

    public ItemType ItemCategory;    
    public EquipmentType EquipmentCategory;
    
    //나중에 아이템의 종류에 따라 최대수량을 정할꺼임
    public int MaxNumberItems = 9;
    public bool IsFull = false;

    //강낭콩 (일단은 두는곳)
    public Sprite Empty;

    public ItemDTO(string ItemName, int ItemQuantity, Sprite ItemSprite, string ItemDescription,Sprite bin ,ItemType itemType = 0 ,EquipmentType EquipmentType = 0)//0은 자동 null 타입
    {
        this.ItemName         = ItemName;
        this.ItemQuantity     = ItemQuantity;
        this.ItemSprite       = ItemSprite;
        this.ItemDescription  = ItemDescription;
        
        ItemCategory = itemType;
        EquipmentCategory = (ItemCategory == ItemType.Equipment) ? EquipmentType : EquipmentType.Null;
        Empty = bin;
    }

    public ItemDTO CopyItemDTO()
    {
        return (ItemDTO)MemberwiseClone();
    }

    // 아이템 추가 메소드
    public int AddItem(string ItemName, int ItemQuantity, Sprite ItemSprite,string ItemDescription,ItemType ItemCategory, EquipmentType EquipmentCategory)
    {
        if (IsFull) return ItemQuantity;
        
        this.ItemName        =   ItemName;
        this.ItemSprite      =   ItemSprite;
        this.ItemDescription =   ItemDescription;
        this.ItemCategory    =   ItemCategory;

        if(this.ItemCategory == ItemType.Equipment)
        {
            this.EquipmentCategory = EquipmentCategory;
        }
            
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

        ItemCategory = 0;
        EquipmentCategory = 0;

    }
}
