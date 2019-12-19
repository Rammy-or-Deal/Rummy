using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class UIChangeCardDialog : MonoBehaviour
{
    public Image[] cardGroups;
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

    public void OnExchangeClick()
    {
        for (int i = 0; i < middleCards.Length; i++)
        {
            var card = middleCards[i].GetValue();
            middleCards[i].SetValue(backCards[i].GetValue());
            backCards[i].SetValue(card);
        }
        UpdateHandSuitString();
    }
    public void OnDoubleDownClick()
    {
        DoubleDownRequest();
    }

    private void DoubleDownRequest()
    {
        // var pList = new PlayerInfoContainer();
        // pList.m_playerInfoListString = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonFields.PLAYER_LIST_STRING];
        // if (pList.m_playerList.Count(x => x.m_status == enumPlayerStatus.Fortune_Doubled) > 0)
        // {
        //     return;
        // }

        SendMyCards(enumPlayerStatus.Fortune_Doubled);
    }

    public void OnConfirmClick()
    {
        SendMyCards(enumPlayerStatus.Fortune_dealtCard);
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

        if(CheckIfLuckyCards(frontList, middleList, backList)) return;

        List<Card> resList = new List<Card>();
        frontText.color = Color.green;
        middleText.color = Color.green;
        backText.color = Color.green;
        SetCardGroupColor(0, true);
        SetCardGroupColor(1, true);
        SetCardGroupColor(2, true);

        var frontType = FortuneRuleMgr.GetCardType(frontList, ref frontList);
        var middleType = FortuneRuleMgr.GetCardType(middleList, ref middleList);
        var backType = FortuneRuleMgr.GetCardType(backList, ref backList);

        frontText.text = FortuneRuleMgr.GetCardTypeString(frontType);
        middleText.text = FortuneRuleMgr.GetCardTypeString(middleType);
        backText.text = FortuneRuleMgr.GetCardTypeString(backType);

        LogMgr.Inst.Log("Result calculated. type=(front,middle,back) : " + frontType + ", " + middleType + ", " + backType, (int)LogLevels.PlayerLog1);

        var frontScore = FortuneRuleMgr.GetScore(frontList, frontType);
        var middleScore = FortuneRuleMgr.GetScore(middleList, middleType);
        var backScore = FortuneRuleMgr.GetScore(backList, backType);

        LogMgr.Inst.Log("score=(front,middle,back) : " + frontScore + ", " + middleScore + ", " + backScore, (int)LogLevels.PlayerLog1);
        // Compare front and middle
        if (frontScore > middleScore || middleScore > backScore)
        {
            frontText.color = Color.red;
            middleText.color = Color.red;
            backText.color = Color.red;

            SetCardGroupColor(0, false);
            SetCardGroupColor(1, false);
            SetCardGroupColor(2, false);
        }
        
    }

    private bool CheckIfLuckyCards(List<Card> frontList, List<Card> middleList, List<Card> backList)
    {
        var luck = FortuneRuleMgr.CheckIfLuckyCards(frontList, middleList, backList);
        if(luck == Lucky.None)
            return false;               
        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, (int)enumGameMessage.Fortune_Lucky},
            {Common.LUCKY_NAME, luck}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        return true;
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
    public void SendMyCards(enumPlayerStatus status)
    {
        FortuneMe.Inst.SetMyProperty((int)status);

        var frontList = getCardList(frontCards);
        var middleList = getCardList(middleCards);
        var backList = getCardList(backCards);

        int msgId = (int)enumGameMessage.Fortune_OnPlayerDealCard;

        if (status == enumPlayerStatus.Fortune_Doubled)
            msgId = (int)enumGameMessage.Fortune_DoubleDownRequest;


        Hashtable props = new Hashtable{
            {PhotonFields.GAME_MESSAGE, msgId},
            {Common.PLAYER_ID, PhotonNetwork.LocalPlayer.ActorNumber},
            {Common.FORTUNE_PLAYER_FRONT_CARD, string.Join(",", frontList.Select(x=>x.cardString))},
            {Common.FORTUNE_PLAYER_MIDDLE_CARD, string.Join(",", middleList.Select(x=>x.cardString))},
            {Common.FORTUNE_PLAYER_BACK_CARD, string.Join(",", backList.Select(x=>x.cardString))},
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        this.gameObject.SetActive(false);
    }

    public void SetCardGroupColor(int id, bool flag)
    {
        cardGroups[id].color = flag ? new Color32(43, 43, 43, 255) : new Color32(255, 42, 42, 255);
    }

    public void OnSortButton()
    {

        var backList = new List<Card>();
        var frontList = new List<Card>();
        var middleList = new List<Card>();

        var cardList = new List<Card>();
        foreach (var c in myCards)
        {
            var card = new Card();
            card.num = c.num;
            card.color = c.color;
            cardList.Add(card);
        }

        #region Making backList
        var tmpList = new List<Card>();
        for (int i = 0; i < cardList.Count; i++)
        {
            Card card = new Card(cardList[i].num, cardList[i].color);
            tmpList.Add(card);
        }
        GameMgr.Inst.Log("tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
        FortuneRuleMgr.GetCardType(tmpList, ref tmpList);
        GameMgr.Inst.Log("sorted tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
        for (int i = 0; i < 5; i++)
        {
            Card card = new Card(tmpList[i].num, tmpList[i].color);
            if (tmpList[i].num == 14) card.num = 1;
            backList.Add(card);
            cardList.Remove(cardList.Where(x => x.num ==card.num && x.color ==card.color).First());
        }

        for (int i = 0; i < backList.Count; i++)
        {
            backCards[i].SetValue(backList[i]);
        }

        #endregion

        #region Making middleList
        tmpList.Clear();
        for (int i = 0; i < cardList.Count; i++)
        {
            Card card = new Card(cardList[i].num, cardList[i].color);
            tmpList.Add(card);
        }
        GameMgr.Inst.Log("tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
        FortuneRuleMgr.GetCardType(tmpList, ref tmpList);
        GameMgr.Inst.Log("sorted tmpList:=" + string.Join(", ", tmpList.Select(x => x.cardString)));
        for (int i = 0; i < 5; i++)
        {
            Card card = new Card(tmpList[i].num, tmpList[i].color);
            if (tmpList[i].num == 14) card.num = 1;
            middleList.Add(card);
            cardList.Remove(cardList.Where(x => x.num == card.num && x.color == card.color).First());
        }
        for (int i = 0; i < middleList.Count; i++)
        {
            middleCards[i].SetValue(middleList[i]);
        }
        #endregion
        GameMgr.Inst.Log("last tmpList:=" + string.Join(", ", cardList.Select(x => x.cardString)));
        #region Making frontList
        for (int i = 0; i < cardList.Count; i++)
        {
            Card card = new Card(cardList[i].num, cardList[i].color);
            frontList.Add(card);
        }
        for (int i = 0; i < frontList.Count; i++)
        {
            frontCards[i].SetValue(frontList[i]);
        }
        #endregion

        GameMgr.Inst.Log("tmpList backList:=" + string.Join(", ", backList.Select(x => x.cardString)));
        GameMgr.Inst.Log("tmpList middleList:=" + string.Join(", ", middleList.Select(x => x.cardString)));
        GameMgr.Inst.Log("tmpList frontList:=" + string.Join(", ", frontList.Select(x => x.cardString)));
    }
}
