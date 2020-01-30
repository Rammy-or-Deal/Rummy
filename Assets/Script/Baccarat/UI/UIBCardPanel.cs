using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBCardPanel : MonoBehaviour
{
    public UIBCard[][] cards;
    public UIBCard[] leftCards;
    public UIBCard[] rightCards;
    public Vector3[][] cardOrgPos;
    
    
    // Start is called before the first frame update
    void Start()
    {
        cardOrgPos=new Vector3[2][];
        cards=new UIBCard[2][];
        cards[0]=new UIBCard[3];
        cards[1]=new UIBCard[3];
        for (int j = 0; j < 3; j++)
        {
            cards[0][j]=leftCards[j];
            cards[1][j]=rightCards[j];
        }

        StartCoroutine(SetOriPos());
    }

    IEnumerator SetOriPos()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 2; i++)
        {
            cardOrgPos[i]=new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                cardOrgPos[i][j] = cards[i][j].transform.position;
            }
        }
    }
  
    internal void Init()
    {
        foreach(var card in leftCards)
        {
            card.Init();
        }
        leftCards[leftCards.Length-1].Init(false);
        foreach(var card in rightCards)
        {
            card.Init();
        }
        rightCards[rightCards.Length-1].Init(false);
    }

    public void TweenOriginalPos()
    {
//        GameMgr.Inst.Log("TweenOriginalPos=" + cards[0][0].transform.position+":"+cardOrgPos[0][0], LogLevel.BaccaratDistributeCardLog);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                iTween.MoveTo(cards[i][j].gameObject, cardOrgPos[i][j], Constants.BTweenTime);
            }
        }
    }
}
