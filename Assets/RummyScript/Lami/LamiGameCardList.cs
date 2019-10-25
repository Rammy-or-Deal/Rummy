using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LamiGameCardList : MonoBehaviour
{
    public List<Card> mGameCardList;    //Game cards list
    public HorizontalLayoutGroup centerLayout;
    public LamiGameCard[] showCards;
    public LamiGameCard[] centerCards;
    
    private float[] spacingList = {-17,-21.2f,-23.53f,-24.7f,-25.53f,-26.25f,-26.69f,-27.11f,-27.35f,-27.58f,-28f,-28.22f,-28.4f};

    public int lineNum=-1;
    void Start()
    {
        
    }

    public void Init()
    {
//        centerLayout = GetComponentInChildren<HorizontalLayoutGroup>();
//        showCards=GetComponentsInChildren<LamiGameCard>(true);
//        centerCards=centerLayout.GetComponentsInChildren<LamiGameCard>(true);
        mGameCardList=new List<Card>();
    }
    
    public void AddGameCard(Card card)
    {
        mGameCardList.Add(card);
    }

    public void AddStartCards(List<Card> cards)
    {
        mGameCardList.InsertRange(0,cards);
        ShowCards();
    }
    
    public void AddEndCards(List<Card> cards)
    {
        mGameCardList.AddRange(cards);
        ShowCards();
    }
    
    public void ShowCards()
    {
        if (mGameCardList.Count > 5)
        {
            centerLayout.gameObject.SetActive(true);
            centerLayout.spacing = spacingList[mGameCardList.Count - 6];
        }
        
        for (int i = 0; i < mGameCardList.Count; i++)
        {
            if (i < 2 || (mGameCardList.Count <= 5))
            {
                showCards[i].UpdateCard(mGameCardList[i]);
                showCards[i].gameObject.SetActive(true);
            }
            else
            {
                if (i == mGameCardList.Count - 1)
                {
                    showCards[4].UpdateCard(mGameCardList[i]);
                    showCards[4].gameObject.SetActive(true);
                }
                else
                {
                    centerCards[i-2].UpdateCard(mGameCardList[i]);
                    centerCards[i-2].gameObject.SetActive(true);    
                }
            }
        }
    }

    public void OnClickLine()
    {
        LogMgr.Inst.Log("Clicked: " + lineNum, (int)LogLevels.PlayerLog2);
        if (lineNum > -1)
        {
            LamiMe.Inst.OnClickLine(lineNum);    
        }
    }
}
