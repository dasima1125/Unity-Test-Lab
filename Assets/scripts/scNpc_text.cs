using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scNpc_text : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject trigger;
    [SerializeField] int mode;
    [SerializeField] bool talking;
    [SerializeField] bool life;
    [SerializeField] bool trigger_inside;
  




    
    void Start()
    {
        life    = true;
        talking = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        trigger_inside = Physics2D.OverlapBox(trigger.transform.position, trigger.transform.localScale,0, LayerMask.GetMask("player"));
        if (!trigger_inside)
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

            Debug.Log("대화시작");
            //대화 시작
            GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .gameObject.SetActive(true);
            //할당 패널 활성화
            StartCoroutine(talker(GameObject.Find("UI_veiwer")
                .transform.Find("ChatBox")
                .transform.Find("ChatBoxString")
                .GetComponentInChildren<TextMeshProUGUI>()));

            

        }

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
    
}
