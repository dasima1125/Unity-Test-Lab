using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;


[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    // 자원 증가
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;
    // 버프 증가
    public AttributesToChange attributesToChange = new AttributesToChange(); 
    public int amountToChangeAttributes;

    public bool UseItem()
    {
        if (statToChange == StatToChange.Health)
        {
            Debug.Log("SO진입");
            var target =GameObject.Find("game Manager").GetComponent<scManager>();
            if(target.player_NowHealth >= target.player_Health)
            {
                return false;
            } 
            else 
            {
                target.player_NowHealth += amountToChangeStat;
                return true;
            }
        }
        return false;
    }
    public enum StatToChange
    {
        None,
        Health,
        Ammo
    }
    public enum AttributesToChange
    {
        None,
        strangth,
        defence
    }

    
}
