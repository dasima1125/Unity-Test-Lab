using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemData" , menuName ="ScriptableObject/Item Data" , order = int.MaxValue)]
public class ItemData_SO : ScriptableObject
{
    [SerializeField]
    private int itemID;
    public int ItemID => itemID; 


    [SerializeField]
    private string itemName;
    public string ItemName => itemName;
    

    [SerializeField] 
    private Sprite sprite;   
    public Sprite Sprite => sprite;


    [SerializeField] 
    private int maxNumberItems;  
    public int MaxNumberItems => maxNumberItems;
    
    
    [SerializeField]
    private string itemDescription;
    public string ItemDescription => itemDescription;
    

    [SerializeField] 
    private ItemTypeEnums itemType;
    public ItemTypeEnums ItemType => itemType;


    [HideInInspector,SerializeField] 
    private EquipmentTypeEnums equipmentType;
    public EquipmentTypeEnums EquipmentType => equipmentType;
     
}
