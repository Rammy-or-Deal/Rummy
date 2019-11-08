using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeCardDialog : MonoBehaviour
{
    //cards
    public FortuneCard[] frontCards;
    public FortuneCard[] middleCards;
    public FortuneCard[] backCards;
    public FortuneCard[] myCards;

    public FortuneHandMission[] handMissions;

    public Text frontText;
    public Text middleText;
    public Text backText;
    public GameObject mClock;
    public Text mClockText;

    //
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        for (int i = 0; i < myCards.Length; i++)
        {
            myCards[i].Init(true);
        }
        for (int i = 0; i < handMissions.Length; i++)
            handMissions[i].gameObject.SetActive(false);
    }

    public void OnTipClick()
    {

    }
    public void OnDoubleDownClick()
    {

    }

    public void OnConfirmClick()
    {

    }

    internal void SetMission(FortuneMissionCard mission)
    {
        handMissions[mission.missionLine].SetMission(mission);
    }

    internal void UpdateHandSuitString()
    {
        var frontList = getCardList(frontCards);
        var middleList = getCardList(middleCards);
        var backList = getCardList(backCards);

        List<Card> resList = new List<Card>();
        frontText.color = Color.green;
        middleText.color = Color.green;
        backText.color = Color.green;

        var frontType = FortuneRuleMgr.GetCardType(frontList, ref frontList);
        var middleType = FortuneRuleMgr.GetCardType(middleList, ref middleList);
        var backType = FortuneRuleMgr.GetCardType(backList, ref backList);

        frontText.text = FortuneRuleMgr.GetCardTypeString(frontType);
        middleText.text = FortuneRuleMgr.GetCardTypeString(middleType);
        backText.text = FortuneRuleMgr.GetCardTypeString(backType);

        LogMgr.Inst.Log("Result calculated. score=(front,middle,back) : " + frontType + ", " + middleType + ", " + backType);

        // Compare front and middle
        if (frontType > middleType)
        {
            frontText.color = Color.red; middleText.color = Color.red;
        }
        if (frontType == middleType)
        {
            if (frontList[0].num > middleList[0].num)
            { frontText.color = Color.red; middleText.color = Color.red; }
            if (frontList[0].num == middleList[0].num && frontList[0].color < middleCards[0].color)
            { frontText.color = Color.red; middleText.color = Color.red; }
        }


        if (middleType > backType)
        {
            middleText.color = Color.red; backText.color = Color.red;
        }
        if (middleType == backType)
        {
            if (middleList[0].num > backList[0].num)
            { middleText.color = Color.red; backText.color = Color.red; }
            if (middleList[0].num == backList[0].num && middleList[0].color < backCards[0].color)
            { middleText.color = Color.red; backText.color = Color.red; }
        }
    }
    List<Card> getCardList(FortuneCard[] cards)
    {
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            cardList.Add(cards[i].GetValue());
        }
        return cardList;
    }

}