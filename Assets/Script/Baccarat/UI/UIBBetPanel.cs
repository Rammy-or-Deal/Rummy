using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBBetPanel : MonoBehaviour
{
    public Transform[] panels;
    public UIBBetPan[] pans;
    public Image coinImg;
    List<Image> coinList = new List<Image>();
    private string[] coinSpriteNames = new string[] {"simbol_100", "simbol_500", "simbol_1000", "simbol_10000"};
    private const int diff = 40;
    private int coinCnt = -1;

    void Start()
    {
        pans = new UIBBetPan[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            UIBBetPan pan = panels[i].parent.GetComponent<UIBBetPan>();
            pans[i] = pan;
        }

        for (int i = 0; i < 150; i++)
        {
            Image coinObj = Instantiate(coinImg, new Vector3(0, 0, 0), coinImg.transform.rotation, transform);
            coinList.Add(coinObj);
        }
    }

    public void OnPlayerBet(Vector3 originPos, int moneyId, int areaId) //x,y: original position 
    {
        coinCnt++;
        Image coinObj = coinList[coinCnt];
        coinObj.transform.position = originPos;
        coinObj.transform.SetParent(panels[areaId]);
        coinObj.sprite = Resources.Load<Sprite>("baccarat/" + coinSpriteNames[moneyId]);
        coinObj.name = "coin" + coinCnt;
        coinObj.gameObject.SetActive(true);
        Vector3 pos = RandomPos(panels[areaId].gameObject, diff);
        iTween.MoveTo(coinObj.gameObject, iTween.Hash("position", pos, "islocal", true, "time", 0.5));
    }

    public Vector3 RandomPos(GameObject obj, int diff)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        //        Vector2 pos = rectTransform.anchoredPosition;
        Rect rect = rectTransform.rect;
        float xPos = (rect.width - diff) / 2;
        float yPos = (rect.height - diff) / 2;
        return new Vector3(Random.Range(-xPos, xPos), Random.Range(-yPos, yPos), 0);
    }

    internal void Init()
    {
        foreach (var pan in pans)
        {
            pan.Init();
        }

        Debug.LogError(coinCnt);
        for (int i = 0; i <= coinCnt; i++)
        {
            coinList[i].gameObject.SetActive(false);
        }
        coinCnt = -1;
    }
}