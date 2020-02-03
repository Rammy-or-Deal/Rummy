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
    List<Image>[] coinList;
    private string[] coinSpriteNames = new string[] {"simbol_100", "simbol_500", "simbol_1000", "simbol_10000"};
    private const int diff = 40;
    private int[] coinCnt;

    void Start()
    {
        pans = new UIBBetPan[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            UIBBetPan pan = panels[i].parent.GetComponent<UIBBetPan>();
            pans[i] = pan;
        }

        coinCnt = new int[4];
        coinList = new List<Image>[4];
        for (int moneyId = 0; moneyId < 4; moneyId++)
        {
            coinList[moneyId]=new List<Image>();
            for (int i = 0; i < 50; i++)
            {
                Image coinObj = Instantiate(coinImg, new Vector3(0, 0, 0), coinImg.transform.rotation, transform);
                coinObj.sprite = Resources.Load<Sprite>("baccarat/" + coinSpriteNames[moneyId]);
                coinList[moneyId].Add(coinObj);
            }
        }
    }

    public void OnPlayerBet(Vector3 originPos, int moneyId, int areaId) //x,y: original position 
    {
//        Debug.LogWarning(coinCnt[areaId]);
        Image coinObj = coinList[moneyId][coinCnt[moneyId]];
//        Debug.Log("k1");
        coinObj.transform.position = originPos;
//        Debug.Log("k2");
        coinObj.transform.SetParent(panels[areaId]);
//        Debug.Log("k3");
        Vector3 pos = RandomPos(panels[areaId].gameObject, diff);
//        Debug.Log("k5");
        iTween.MoveTo(coinObj.gameObject, iTween.Hash("position", pos, "islocal", true, "time", 0.5));
//        Debug.Log("k6");
//        coinObj.gameObject.SetActive(true);
        coinObj.name = "coin" + coinCnt[moneyId];
//        Debug.LogWarning(coinObj.name);
        coinCnt[moneyId]++;
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
        try
        {
            foreach (var pan in pans)
            {
                pan.Init();
            }

            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i <= coinCnt[k]; i++)
                {
                    coinList[k][i].transform.position = new Vector3(-1000, 0, 0);
                }

                coinCnt[k] = 0;
            }
        }
        catch
        {
            GameMgr.Inst.Log("There's an error in UIBBetPanel->Init()");
        }
    }
}