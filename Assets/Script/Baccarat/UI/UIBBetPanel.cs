﻿using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class UIBBetPanel : MonoBehaviour
{
    public Transform[] panels;
    public UIBBetPan[] pans;
    public Image coinImg;
    List<Image> coinList = new List<Image>();
    private string[] coinSpriteNames=new string[] {"simbol_100","simbol_500","simbol_1000","simbol_10000"};
    private const int diff = 40;
    void Start()
    {
        pans=new UIBBetPan	[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            UIBBetPan pan = panels[i].parent.GetComponent<UIBBetPan>();
            pans[i] = pan;
        }
    }
    
    public void OnPlayerBet(float x, float y, int moneyId, int areaId)
    {
        Image coinObj = Instantiate(coinImg, new Vector3	(x,y,0), coinImg.transform.rotation, panels[areaId]);
        coinObj.sprite= Resources.Load<Sprite>("baccarat/"+coinSpriteNames	[moneyId]);
        Vector3 pos = RandomPos(panels[areaId].gameObject, diff);
        iTween.MoveTo(coinObj.gameObject, iTween.Hash("position", pos, "islocal", true, "time", 0.5));
        coinList.Add(coinObj);
    }

    public Vector3 RandomPos(GameObject obj,int diff)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
//        Vector2 pos = rectTransform.anchoredPosition;
        Rect rect = rectTransform.rect;
        float xPos = (rect.width - diff) / 2;
        float yPos = (rect.height - diff) / 2;
        return new Vector3(Random.Range(-xPos,xPos), Random.Range(-yPos,yPos), 0);
    }

    internal void Init()
    {
        foreach(var pan in pans)
        {
            pan.Init();
        }
        foreach(var coin in coinList)
        {
            coin.gameObject.SetActive(false);
        }
        coinList.Clear();
    }
}