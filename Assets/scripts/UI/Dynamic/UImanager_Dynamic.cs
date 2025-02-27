using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UImanager_Dynamic : MonoBehaviour
{
    private static UImanager_Dynamic instance;
    public static UImanager_Dynamic Dynamic
    { 
        get => instance ?? (instance = FindObjectOfType<UImanager_Dynamic>());
    }

    #nullable enable
    public IEnumerator MakeInfo(string []? texts = null ,bool ? clear = null)
    {
        var manager = UImanager.manager;
        
        Transform[] children = manager.canvas.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in manager.canvas.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "info") // 이름이 "info"인지 확인
            {
                child.transform.DOMoveY(child.transform.position.y + 45f, 0.1f);
                //child.transform.position += new Vector3(0, 45f, 0);
            }
        }

        if (manager.panelQueue.Count >= 5)//5개인상태에서 생성 시도하면
        {
            GameObject oldPanel = manager.panelQueue.Dequeue();
        
            //DOTween.Kill(oldPanel); 일단 안써도 버그는 안생김
            oldPanel.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() => Destroy(oldPanel));
        }
       
        GameObject panelInstance = Instantiate(manager.DynamicUIs["infoAlarm"]);//infoAlarm(Clone)
        panelInstance.name = "info";
        panelInstance.transform.SetParent(manager.canvas.transform, false);

        manager.panelQueue.Enqueue(panelInstance);

        TextMeshProUGUI [] infotitleText = panelInstance.GetComponentsInChildren<TextMeshProUGUI>();
        
        int index = 0;
        if(infotitleText.Length  == 2)
        foreach (var info in infotitleText)
        {
            info.text = manager.sender[index++];
        }

        var canvas = panelInstance.GetComponent<CanvasGroup>();
        
        canvas.alpha = 0f;
        
        canvas.DOFade(1, 1f).OnComplete(() =>  //또는 시퀸스로 지연 구현하는법도있는데 시퀸스는 오브젝트가아니라 참조 주소가 필요함
        {                                      // 즉 큐 구조가 개판이됨
            canvas.DOFade(1, 5f).OnComplete(() =>
            {
                StartCoroutine(Clear());
            });
        });

        //추가방싱 >> 엘든링식
        //밑에추가 
        //만약 큐에있는 애면 기존에있던에 트랜스폼 자기높이만큼 +10정도 위로
        //지금 추가할애가 거기 추가
  
        yield return null;
       
    }
    IEnumerator Clear()
    {
        var manager = UImanager.manager;
        
        GameObject target = manager.panelQueue.Dequeue();
        if(manager.panelQueue == null) yield break;

        Sequence sequence = DOTween.Sequence();  
        sequence.Append(target.GetComponent<CanvasGroup>().DOFade(0, 1f));
        yield return sequence.WaitForCompletion();
        
        Destroy(target);
    }
    //처리방법
    // 패널을 큐에 넣음 
    // 큐에 패널을 넣으면서 카운트 3초를 줌
    // 3초가 지나면 그 큐에삭제명령을 줌

}
