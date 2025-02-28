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
    public void MakeGetItem(string [] texts)
    {
        var manager = UImanager.manager;
        foreach (Transform child in manager.canvas.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "info") 
            {
                StartCoroutine(MoveY(child, 45f, 0.2f));
                //child.transform.DOMoveY(child.transform.position.y + 45f, 1f); 닷트윈 맹신 금지
            }
        }

        if (manager.ItemPanelQueue.Count >= 4)//개인상태에서 생성 시도하면
        {
            GameObject oldPanel = manager.ItemPanelQueue.Dequeue();
            //DOTween.Kill(oldPanel); 
            oldPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => Destroy(oldPanel));
        }
       
        GameObject panelInstance = Instantiate(manager.DynamicUIs["infoAlarm"]);//infoAlarm(Clone)
        panelInstance.name = "info";
        panelInstance.transform.SetParent(manager.canvas.transform, false);

        manager.ItemPanelQueue.Enqueue(panelInstance);

        TextMeshProUGUI [] infotitleText = panelInstance.GetComponentsInChildren<TextMeshProUGUI>();
        
        int index = 0;
        if(infotitleText.Length  == 2)
        foreach (var info in infotitleText)
        {
            info.text = texts[index++];
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
       
    }

    public void MakeInfo(string [] texts)
    {
        var manager = UImanager.manager;
        GameObject panelInstance = Instantiate(manager.DynamicUIs["infoAlarm"]);//주 패널 생성
        panelInstance.transform.SetParent(manager.canvas.transform, false);
        manager.InfoUI = panelInstance;
        panelInstance.transform.position = new Vector3(panelInstance.transform.position.x, panelInstance.transform.position.y + 480f, panelInstance.transform.position.z);

        TextMeshProUGUI [] infotitleText = panelInstance.GetComponentsInChildren<TextMeshProUGUI>();
        
        var canvas = panelInstance.GetComponent<CanvasGroup>();
        canvas.alpha = 0f;

        Sequence sequence = DOTween.Sequence(); 
        sequence.Append(canvas.DOFade(1,1f));
        sequence.AppendCallback(() => StartCoroutine(infoString(infotitleText,texts)));
        sequence.AppendInterval(5f);
        sequence.Append(canvas.DOFade(0,1f));
        sequence.AppendCallback(() => Destroy(panelInstance));
    }
    IEnumerator MoveY(Transform child, float n,float i)
    {
        float elapsedTime = 0f;

        while (elapsedTime < i)
        {
            float distanceToMove = (n / i) * Time.deltaTime;
            if(child == null) yield break;
            child.position = new Vector2(child.position.x, child.position.y + distanceToMove); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator infoString(TextMeshProUGUI [] infotitleText,string [] text)
    {
        
        foreach (char letter in text[0])
        {
            infotitleText[0].text += letter; 
            yield return new WaitForSeconds(0.05f); 
        }
        yield return new WaitForSeconds(0.2f); 
        
        foreach (char letter in text[1])
        {
            infotitleText[1].text += letter; 
            yield return new WaitForSeconds(0.05f); 
        }
        yield return new WaitForSeconds(3);
    }

    IEnumerator Clear()
    {
        var manager = UImanager.manager;
        
        GameObject target = manager.ItemPanelQueue.Dequeue();
        if(manager.ItemPanelQueue == null) yield break;

        Sequence sequence = DOTween.Sequence();  
        sequence.Append(target.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        yield return sequence.WaitForCompletion();
        
        Destroy(target);
    }
    //처리방법
    // 패널을 큐에 넣음 
    // 큐에 패널을 넣으면서 카운트 3초를 줌
    // 3초가 지나면 그 큐에삭제명령을 줌

}
