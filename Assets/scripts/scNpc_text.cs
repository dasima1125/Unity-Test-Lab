using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
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
    [SerializeField]private string id;
    [SerializeField]private string dialogs;
    [SerializeField]private bool eventCleared;
    [SerializeField]private bool Mission_Activation = false;

  




    
    void Start()
    {
        life    = true;
        talking = false;
        eventCleared = false;
   
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
        
            //StopCoroutine("talkertest3");// <<=코루틴 중첩문제 발생가능성 있음 StopAllCoroutines로 걍 이 스크립트내 코루틴 싹다 미는게 안정적임
            StopAllCoroutines(); 
            //여기서부터 땜빵
            if(talking)
            {
                if(life)
                //life = false;
                talking = false;
            }
            scCamera camera = GameObject.Find("Main Camera").GetComponent<scCamera>();
            camera.zoomOut();
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
            StartCoroutine(talkertest3(GameObject.Find("UI_veiwer")
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
    List<(string,string[])> jsonReadertest2()//가변형 //개선좀하자 굳이 튜플로
    {
        TextAsset jsonData = Resources.Load<TextAsset>("npcText"); //경로 중요함 하위면 루트 지정해줘야함
        if (jsonData != null)
        {
            JToken extractionJson_phase_1 = JObject.Parse(jsonData.text)["npcs"]?.FirstOrDefault(b => b["id"]?.ToString() == id)?? "null";
            if (extractionJson_phase_1.ToString() == "null") return null;

            List<(string,string[])> output = new List<(string,string[])>(); //딕셔너리는 중복된 값을 제거하므로 저장에 좋진않음 튜플식으로 전환
            var name = extractionJson_phase_1["dialogs"];
            foreach (var keyName in (JObject)name) // 키 이름 추출용 토큰에선 키 추출이 안됨 고로 오브젝트로 변환 필요
            {
                //Debug.Log("검색된 행동별 대사 타입 : " + keyName.Key); // "start", "end" 출력 >> 상황별 id를 저장할 예정
                var dialogsValue = (JObject)keyName.Value;
                foreach (var v in dialogsValue)// 해당키의 json 내부 저장된 배열 순회
                { 
                    output.Add((keyName.Key, v.Value.ToObject<string[]>()));
                }
                
            }
            //Debug.Log(String.Join("\n", output.Select(item => String.Join(", ", item.Item2))));//내부 배열만 출력 

            // 저장 방식 
            //    ㄴ 배열 이벤트 시작이랑 끝으로 두개 받음 
            //           ㄴ 문자열 키  문자열 배열 쌍 값으로 저장.
            return output;

        }
        return null;
    }
    void infoAlram(List<string> text ,int type)
    {
        GameObject panel = GameObject.Find("UI_veiwer").transform.Find("infoAlarm").gameObject;//패널 접근
        mainmenu_play coroutinplayer = GameObject.Find("UI_veiwer").GetComponent<mainmenu_play>();
        
        //TextMeshProUGUI infoText = panel.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI infotitleText    = panel.transform.Find("infoTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI infocontentText  = panel.transform.Find("infoContent").GetComponent<TextMeshProUGUI>();
        CanvasGroup panelImage = panel.GetComponent<CanvasGroup>();
        
        if(panelImage.alpha != 0) return;

        infotitleText.text   ="";//초기화
        infocontentText.text ="";
        
        //infoText.DOText("hello", 2f, true);//유료버전만된다
        panelImage.DOFade(1,1f).OnComplete(() =>
        {
            Sequence sequence = DOTween.Sequence();        
            sequence.AppendCallback(() => coroutinplayer.infoWrite(infotitleText,infocontentText, text,type));   
            sequence.AppendInterval(4f).Append(panelImage.DOFade(0, 1f).SetUpdate(true));//지연
            sequence.Play();

        });

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
        var output = jsonReadertest2();
        if (output == null)
        {
            insert.text = "잘못된 대사 접근중. 종료합니다";
            Debug.Log("json 검색 실패.");
            yield break;
        }
        
        List<(string, string[])> start_dialogs = output.Where(item => item.Item1 == "start").ToList();
        List<(string, string[])> end_dialogs   = output.Where(item => item.Item1 == "end").ToList();
        
        List<string> infosender = output.Where(item => item.Item1 == "Quest").SelectMany(item => item.Item2).ToList();

        //Debug.Log(String.Join(" ",start_dialogs.Select(item => item.Item1)));
        
        string[] sentences = {"데이터 불러오기 실패."};
        /////////////////////// 분기점 설정 ///////////////////
        // 퀘스트 처리 부분 나중에 옴길예정 아마 상속구조로 변경할꺼임
        GameObject gameManagerObject = GameObject.Find("game Manager");//직접 끌어와야지
        
        if(!life && gameManagerObject.GetComponent<scManager>().key_count >= 1)
        {
            gameManagerObject.GetComponent<scManager>().key_count -= 1;
            eventCleared = true;
            life = true;
        }
        // 행동트리마냥 지정해줘야할려나?
        if(!Mission_Activation)
        {
            sentences = start_dialogs[0].Item2;
        }
        else
        {
            if(!eventCleared) // 이벤트 트리거 작동전
            {   
                sentences = life ? start_dialogs[1].Item2 : start_dialogs[2].Item2;
            } 
            else // 작동후
            {
                sentences = life ? end_dialogs[0].Item2 : end_dialogs[1].Item2;
            }
        }
        
        int currentSentenceIndex = 0;
        //이벤트 트리거에 따라 다이얼로그 투입이 달라지는 분기 설정
        //    ㄴ  리스트가 2개 이상일시 첫번째 조우와 다음 조우가 대사가 다르다는걸 의미
        //         ㄴ life 변수를 통해 첫번째 조우시 life 변수를 거짓으로 바꾸고 다음부터는 life변수가 거짓인 경로로 이동시킴
        //              ㄴ 첫조우시 리스트 첫번째 값 문자열 추출 , 아닐시 두번째 분해 
        //                   ㄴ 나중에 구조 변경을 쉽게 만들려면 스위치문으로 리스트 개수를 기준으로 행동양식별 구조를 생성 즉 구조를 새로짜야함
        
        //줌인 기능 추가 
        scCamera camera = GameObject.Find("Main Camera").GetComponent<scCamera>();
        camera.zoomIn();
        while (currentSentenceIndex < sentences.Length)
        {
            insert.text = "";
            skip = false;

            foreach (char letter in sentences[currentSentenceIndex])
            {   
                if(skip)
                {
                    skip   = false;
                    insert.text = sentences[currentSentenceIndex];
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
            currentSentenceIndex++; 
        }
        // 마무리단계
        if(life && Mission_Activation)
        {   
            int infoType = !eventCleared ? 1 : 2; 
            infoAlram(infosender,infoType);
        }
        if(Mission_Activation) life = false;
        talking = false; 
        camera.zoomOut();
        
        GameObject.Find("UI_veiwer")
            .transform.Find("ChatBox")
            .gameObject.SetActive(false);
    }
   
}


