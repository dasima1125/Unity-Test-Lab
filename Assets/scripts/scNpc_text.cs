using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;


public class scNpc_text : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject trigger;

    [SerializeField] bool talking;
    [SerializeField] bool skip;
    [SerializeField] bool life;
    [SerializeField] bool trigger_inside;
    //테스트 라인 
    [SerializeField]private string dialogs;
    [SerializeField]private string id;
  




    
    void Start()
    {
        life    = true;
        talking = false;
        jsonReadertest();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        trigger_inside = Physics2D.OverlapBox(trigger.transform.position, trigger.transform.localScale,0, LayerMask.GetMask("player"));
        if (talking && Input.GetKeyDown(KeyCode.E))
        {
            skip = true;
        }
        
        
        if (!trigger_inside) //도중에 나갈시
        {
            StopAllCoroutines(); 
            //여기서부터 땜빵
            if(talking)
            {
                if(life)
                life = false;
                
                talking = false;
            }
            GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .gameObject.SetActive(false);
        }
        
        talktrigger();
    }

    void talktrigger()
    {
        if (talking || !trigger_inside)
        return;
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            talking = true;

            //Debug.Log("대화시작");
            //대화 시작
            GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .gameObject.SetActive(true);
            //할당 패널 활성화
            /**
            StartCoroutine(talker(GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .transform.Find("ChatBoxString")
                .GetComponentInChildren<TextMeshProUGUI>()));
            **/
            StartCoroutine(talkertest2(GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .transform.Find("ChatBoxString")
                .GetComponentInChildren<TextMeshProUGUI>()));

            

        }

    }

    void jsonReader()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("npcText"); //경로 중요함 하위면 루트 지정해줘야함
        if (jsonData != null)
        {
            //Debug.Log("접속 성공");
            
            JObject json = JObject.Parse(jsonData.text); 

            //var insertText = json["npcs"]?.FirstOrDefault(b => b["id"]?.ToString() == id)?["dialogs"]?[dialogs]?.ToString() ?? string.Empty;
            var insertText = json["npcs"]?.FirstOrDefault(b => b["id"]?.ToString() == id)?["dialogs"]?[dialogs]?.ToObject<string[]>();
            // 출력
            //Debug.Log("배열 수" + insertText.Count().ToString());
            Debug.Log(string.Join("\n", insertText));

            
   
        }

    }
    string [] jsonReadertest()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("npcText"); //경로 중요함 하위면 루트 지정해줘야함
        if (jsonData != null)
        {
            JObject json = JObject.Parse(jsonData.text); 
            var insertText = json["npcs"]?.FirstOrDefault(b => b["id"]?.ToString() == id)?["dialogs"]?[dialogs]?.ToObject<string[]>();
            // 출력
            Debug.Log(string.Join("\n", insertText));

            return insertText;
        }
        return null;

    }


    IEnumerator talker(TextMeshProUGUI insert)
    {
        string[] sentences = { "hello!", "This is hard coding.", "And need more money." };
        string[] sentences_end = { "It’s done now", "Go away."};
        int currentSentenceIndex = 0;

        if(life)
        {
            while (currentSentenceIndex < sentences.Length)
            {
                insert.text = "";
                foreach (char letter in sentences[currentSentenceIndex])
                {
                    insert.text += letter; 
                    yield return new WaitForSeconds(0.03f); 

                }

                while (!Input.GetKeyDown(KeyCode.E))// 다음 문장 출력 대기
                {
                    yield return null; 
                }
                currentSentenceIndex++; 
            }
            life = false;
        }
        
        if(!life)
        {
            while (currentSentenceIndex < sentences_end.Length)
            {
                insert.text = "";
                foreach (char letter in sentences_end[currentSentenceIndex])
                {
                    insert.text += letter; 
                    yield return new WaitForSeconds(0.03f); 
                }

                while (!Input.GetKeyDown(KeyCode.E)) 
                {
                        yield return null; 
                }
                currentSentenceIndex++; 
            }
        }
        // 마무리단계
   
        talking = false; 
        GameObject.Find("UI_veiwer")
            .transform.Find("ChatBox")
            .gameObject.SetActive(false);

       
    }
    IEnumerator talkertest2(TextMeshProUGUI insert)
    {
        Debug.Log("테스트 시작");
        string[] sentences = jsonReadertest();
        int currentSentenceIndex = 0;
        
        

        while (currentSentenceIndex < sentences.Length)
        {
            insert.text = "";
            skip   = false;
            Debug.Log(currentSentenceIndex + " : 번째");
            
            
            foreach (char letter in sentences[currentSentenceIndex])
            {   
                if(skip)
                {
                    skip   = false;
                    insert.text = sentences[currentSentenceIndex];
                    Debug.Log("skiped");
                    break;
                }  
                
                insert.text += letter; 
                yield return new WaitForSeconds(0.05f); 

            }
            yield return new WaitForSeconds(0.2f);//안정화용 딜레이 

            while (!Input.GetKeyDown(KeyCode.E))// 다음 문장 출력 대기
            {
                yield return null; 
            }
            Debug.Log("next");
            currentSentenceIndex++; 
        }
        // 마무리단계
   
        talking = false; 
        GameObject.Find("UI_veiwer")
            .transform.Find("ChatBox")
            .gameObject.SetActive(false);
    }
    IEnumerator talkertest3(TextMeshProUGUI insert)
    {
        Debug.Log("테스트 시작");
        string[] sentences = jsonReadertest();
        int currentSentenceIndex = 0;
        
        

        while (currentSentenceIndex < sentences.Length)
        {
            insert.text = "";
            skip   = false;
            Debug.Log(currentSentenceIndex + " : 번째");
            
            
            foreach (char letter in sentences[currentSentenceIndex])
            {   
                if(skip)
                {
                    skip   = false;
                    insert.text = sentences[currentSentenceIndex];
                    Debug.Log("skiped");
                    break;
                }  
                
                insert.text += letter; 
                yield return new WaitForSeconds(0.05f); 

            }
            yield return new WaitForSeconds(0.2f);//안정화용 딜레이 

            while (!Input.GetKeyDown(KeyCode.E))// 다음 문장 출력 대기
            {
                yield return null; 
            }
            Debug.Log("next");
            currentSentenceIndex++; 
        }
        // 마무리단계
   
        talking = false; 
        GameObject.Find("UI_veiwer")
            .transform.Find("ChatBox")
            .gameObject.SetActive(false);
    }
    
}


