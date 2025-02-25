using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UImanager_ChatBox : MonoBehaviour
{
    private static UImanager_ChatBox instance;

    public static UImanager_ChatBox Chat
    {
        get => instance ?? (instance = FindObjectOfType<UImanager_ChatBox>());
    }

    public IEnumerator PrintDialog(string uiName = null)
    {
     
        var manager = UImanager.manager;
        string testui ="ChatBox";

        //전에 다른패널 페이딩 필요함
       
        GameObject panelInstance = Instantiate(manager.chatBoxUIs[testui]);
        TextMeshProUGUI insert = panelInstance.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault();

        panelInstance.transform.SetParent(manager.canvas.transform, false);
        manager.chatBoxUI = panelInstance;

        

        string testID    = "test";
        string testState = "normal";
        bool   testbool  = false;
        DialogData dialogDTO = new DialogData();
        var dialogArray = dialogDTO.ExtractDialog(testID).pickDialog(testState,testbool);

        int index = 0;
        while (dialogArray.Length > index)
        {
            insert.text = "";
            foreach (var dialog in dialogArray[index])
            {
                insert.text += dialog; 
                yield return new WaitForSeconds(0.05f); 

            }
            yield return new WaitForSeconds(0.5f);
            index++;

        }
        
        yield return new WaitForSeconds(3);
        Destroy(manager.chatBoxUI);
    }
    
}
public class DialogData
{
    public List<string> Types{ get; set; }
    public List<JToken> Dialogs { get; set; }
    public DialogData()
    {
        Types = new List<string>();
        Dialogs = new List<JToken>();
    }

    public void AddData(string type , JToken dialog)
    {
        Types.Add(type);
        Dialogs.Add(dialog);
    }
 
    public DialogData ExtractDialog(string ID) 
    {
        TextAsset jsonData = Resources.Load<TextAsset>("npcText");
        DialogData data = new DialogData();
        JToken extraction_phase1 = JObject.Parse(jsonData.text)["npcs"]?.FirstOrDefault(b => b["id"]?.ToString() == ID);//대상 id기입요망
        
        if (extraction_phase1 == null) return null;
        foreach (var keyName in extraction_phase1["dialogs"] as JObject) data.AddData(keyName.Key,keyName.Value);
        
        return data;
    }

    public string[] pickDialog(string state ,bool visted)
    {
        if(!Types.Contains(state)) return null; 
        int index = Types.IndexOf(state);

        var book = Dialogs[index] as JObject;
        if(book.Count > 1)
        {
            if(!visted)  return book["new"].ToObject<string[]>();
            
            else return book["used"].ToObject<string[]>();
        }
        else return book["new"].ToObject<string[]>();
    }
}

//일단 생각해보자 채팅을 콜할때 먼저 현재 상태를 분별해 타입에 몇번째 순서인지 가져옴
//타입인덱스 순서를 기반으로 Dialogs리스트에 접근 만약 end상태라면 Dialogs리스트에[1]임
//그걸 가져와서 분기를 또 추적함 분기를 받아오면 일단 토큰을 돌리고 해당키를 찾음 
//그 키에 해당되면 문자열을 가져옴
//즉 추적할때 상태의 인덱스와 분기 인덱스를 가져와서 문자열 추출
//즉 상태 인티저 분기 스트링 받고 대사배열 추출 
//ㅇㅋ
//
